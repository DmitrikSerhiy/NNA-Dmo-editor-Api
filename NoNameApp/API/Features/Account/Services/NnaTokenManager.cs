using System;
using System.Threading.Tasks;
using API.Helpers;
using API.Helpers.Extensions;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model.DTOs.Account;
using Model.Entities;
using Model.Interfaces;
using Model.Interfaces.Repositories;

namespace API.Features.Account.Services {
    public sealed class NnaTokenManager {
        
        private readonly NnaUserManager _userManager;
        private readonly IUserRepository _userRepository;
        private readonly NnaTokenHandler _tokenHandler;
        private readonly JwtOptions _jwtOptions;
        private readonly IAuthenticatedIdentityProvider _identityProvider; 

        private static TokenDescriptorProvider _descriptorProvider;
        
        #region Descriptors
        
        private static readonly Func<JwtOptions, NnaUser, SecurityTokenDescriptor> AccessTokenDescriptor = (jwtOptions, user) => {
            var accessTokenDescriptor = _descriptorProvider.ProvideForAccessToken();
            accessTokenDescriptor.AddSigningCredentials(jwtOptions);
            accessTokenDescriptor.AddSubjectClaims(user.Email, user.Id);

            return accessTokenDescriptor;
        };
        
        private static readonly Func<JwtOptions, NnaUser, SecurityTokenDescriptor> RefreshTokenDescriptor = (jwtOptions, user) => {
            var refreshTokenDescriptor = _descriptorProvider.ProvideForRefreshToken();
            refreshTokenDescriptor.AddSigningCredentials(jwtOptions);
            refreshTokenDescriptor.AddSubjectClaims(user.Email, user.Id);

            return refreshTokenDescriptor;
        };

        #endregion

        public NnaTokenManager(
            NnaUserManager userManager, 
            TokenDescriptorProvider descriptorProvider,
            IOptions<JwtOptions> jwtOptions, 
            IUserRepository userRepository, 
            IAuthenticatedIdentityProvider identityProvider) {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _descriptorProvider = descriptorProvider ?? throw new ArgumentNullException(nameof(descriptorProvider));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _identityProvider = identityProvider ?? throw new ArgumentNullException(nameof(identityProvider));
            _jwtOptions = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
            _tokenHandler = new NnaTokenHandler();
        }
        
        public string CreateAccessJwt(NnaUser user) {
            if (user == null) throw new ArgumentNullException(nameof(user));
            
            return _tokenHandler.CreateToken(AccessTokenDescriptor.Invoke(_jwtOptions, user));
        }
        
        public string CreateRefreshJwt(NnaUser user) {
            if (user == null) throw new ArgumentNullException(nameof(user));
            
            return _tokenHandler.CreateToken(RefreshTokenDescriptor.Invoke(_jwtOptions, user));
        }

        /// <summary>
        /// Creates new valid pair of access/refresh tokens for user with specified email.
        /// All previous tokens for that user become invalid.
        /// </summary>
        public async Task<TokensDto> CreateTokens(string email) {
            if (email == null) throw new ArgumentNullException(nameof(email));

            var user = await _userManager.FindByEmailAsync(email);
            return await GenerateAndSaveTokensAsync(user);
        }

        /// <summary>
        /// Check whether current user has tokens.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> VerifyTokenAsync() {
            var tokens = await _userRepository.GetTokens(_identityProvider.AuthenticatedUserId);
            if (tokens is null) {
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Load existing tokens or generate new if tokens are missing.
        /// If tokens exist but are invalid then new tokens are created 
        /// </summary>
        public async Task<TokensDto> GetOrCreateTokensAsync(NnaUser user) {
            if (user == null) throw new ArgumentNullException(nameof(user));
            var tokens = await _userRepository.GetTokens(user.Id);
            
             if (tokens is null) {
                 return await GenerateAndSaveTokensAsync(user);
             }
            
            var accessTokenValidation = _tokenHandler.ValidateToken(tokens.Value.accessToken.Value,
                TokenValidationParametersProvider.Provide(AccessTokenDescriptor.Invoke(_jwtOptions, user)));
            
            var refreshTokenValidation = _tokenHandler.ValidateToken(tokens.Value.refreshToken.Value,
                TokenValidationParametersProvider.Provide(RefreshTokenDescriptor.Invoke(_jwtOptions, user)));
            
            if (!accessTokenValidation.IsValid || !refreshTokenValidation.IsValid) {
                return GenerateAndUpdateTokens(user);
            } 
            
            return new TokensDto {
                AccessToken = tokens.Value.accessToken.Value,
                RefreshToken = tokens.Value.refreshToken.Value,
                AccessTokenKeyId = _tokenHandler.GetTokenKeyId(tokens.Value.accessToken.Value),
                RefreshTokenKeyId = _tokenHandler.GetTokenKeyId(tokens.Value.refreshToken.Value)
            };
        }
        
        /// <summary>
        /// Creates or Updates new access/refresh tokens pair if old access/refresh pair corresponds to each other.
        /// </summary>
        public async Task<TokensDto> RefreshTokens(RefreshDto refreshDto) {
            if (string.IsNullOrWhiteSpace(refreshDto.AccessToken)) throw new ArgumentNullException(nameof(refreshDto.AccessToken));
            if (string.IsNullOrWhiteSpace(refreshDto.RefreshToken)) throw new ArgumentNullException(nameof(refreshDto.RefreshToken));
            if (refreshDto.AccessToken == refreshDto.RefreshToken) throw new ArgumentException();

            var userEmailFromRefreshToken = _tokenHandler.GetUserEmail(refreshDto.RefreshToken);
            var userEmailFromAccessToken = _tokenHandler.GetUserEmail(refreshDto.AccessToken);
            if (userEmailFromRefreshToken is null || userEmailFromAccessToken is null) {
                return null;
            }
            if (userEmailFromRefreshToken != userEmailFromAccessToken) {
                return null;
            }

            var authData = await _userRepository.GetAuthenticatedUserDataAsync(userEmailFromRefreshToken);
            if (authData is null) {
                return null;
            }
            
            if (_tokenHandler.GetTokenKeyId(refreshDto.AccessToken) != authData.AccessTokenId ||
                _tokenHandler.GetTokenKeyId(refreshDto.RefreshToken) != authData.RefreshTokenId) {
                return null;
            }

            var user = new NnaUser {
                Email = authData.Email,
                Id = authData.UserId
            };

            var refreshValidationResult = _tokenHandler.ValidateToken(
                refreshDto.RefreshToken, 
                TokenValidationParametersProvider.Provide(RefreshTokenDescriptor.Invoke(_jwtOptions, user)));

            if (!refreshValidationResult.IsValid) {
                return null;
            }
            
            return GenerateAndUpdateTokens(user);
        }

        public async Task ClearTokens(string email) {
            if (email == null) throw new ArgumentNullException(nameof(email));
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null) {
                return;
            }
            
            if (_identityProvider.AuthenticatedUserEmail != user.Email) {
                return;
            }
            
            await _userRepository.ClearTokens(user);
            _identityProvider.ClearAuthenticatedUserInfo();
        }

        public async Task<bool> ValidateGoogleTokenAsync(AuthGoogleDto authGoogleDto) {
            if (authGoogleDto is null) throw new ArgumentNullException(nameof(authGoogleDto));
            if (authGoogleDto.Email is null) throw new ArgumentNullException(nameof(authGoogleDto.Email));
            
            try {
                var payload = await GoogleJsonWebSignature.ValidateAsync(authGoogleDto.GoogleToken);
                if (!payload.EmailVerified || payload.Email != authGoogleDto.Email) {
                    return false;
                }
            }
            catch (InvalidJwtException) {
                return false;
            }

            return true;
        }

        private TokensDto GenerateAndUpdateTokens(NnaUser user) {
            var newTokens = GenerateNewTokenPair(user);
            
            _userRepository.UpdateTokens( 
                TokenMapper.MapToAccessTokenByPasswordAuth(user.Id, newTokens),
                TokenMapper.MapToRefreshTokenByPasswordAuth(user.Id, newTokens));
            
            return newTokens;
        }

        private async Task<TokensDto> GenerateAndSaveTokensAsync(NnaUser user) {
            var newTokens = GenerateNewTokenPair(user);
            
            await _userRepository.SaveTokens(                  
                TokenMapper.MapToAccessTokenByPasswordAuth(user.Id, newTokens),
                TokenMapper.MapToRefreshTokenByPasswordAuth(user.Id, newTokens));

            return newTokens;
        }
        
        private TokensDto GenerateNewTokenPair(NnaUser user) {
            var accessToken = CreateAccessJwt(user);
            var refreshToken = CreateRefreshJwt(user);
                
            return new TokensDto {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenKeyId = _tokenHandler.GetTokenKeyId(accessToken),
                RefreshTokenKeyId = _tokenHandler.GetTokenKeyId(refreshToken)
            };
        }
    }
}