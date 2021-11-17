using System;
using System.Threading.Tasks;
using API.Helpers;
using API.Helpers.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model.DTOs.Account;
using Model.Entities;
using Model.Interfaces.Repositories;

namespace API.Features.Account.Services {
    public sealed class IdentityService {
        
        private readonly NnaUserManager _userManager;
        private readonly IUserRepository _userRepository;
        private readonly NnaTokenHandler _tokenHandler;
        private readonly JwtOptions _jwtOptions;

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

        public IdentityService(
            NnaUserManager userManager, 
            TokenDescriptorProvider descriptorProvider,
            IOptions<JwtOptions> jwtOptions, 
            IUserRepository userRepository) {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _descriptorProvider = descriptorProvider ?? throw new ArgumentNullException(nameof(descriptorProvider));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
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
        /// Load existing tokens or generate new if tokens are missing. If tokens exist but are invalid
        /// </summary>
        public async Task<TokensDto> GetOrCreateTokensAsync(string email) {
            if (email == null) throw new ArgumentNullException(nameof(email));
            
            var user = await _userManager.FindByEmailAsync(email);
            var tokens = await _userRepository.GetTokens(user.Id);
            
             if (tokens is null) {
                 var newTokens = GenerateNewTokenPair(user);
                 await _userRepository.SaveTokens(                  
                     DbTokenDescriptor.MapToAccessTokenByPasswordAuth(user.Id, newTokens),
                     DbTokenDescriptor.MapToRefreshTokenByPasswordAuth(user.Id, newTokens));
            
                 return newTokens;
             }
            
            var accessTokenValidation = _tokenHandler.ValidateToken(tokens.Value.accessToken.Value,
                TokenValidationParametersProvider.Provide(AccessTokenDescriptor.Invoke(_jwtOptions, user)));
            
            var refreshTokenValidation = _tokenHandler.ValidateToken(tokens.Value.refreshToken.Value,
                TokenValidationParametersProvider.Provide(RefreshTokenDescriptor.Invoke(_jwtOptions, user)));
            
            if (!accessTokenValidation.IsValid || !refreshTokenValidation.IsValid) {
                var newTokens = GenerateNewTokenPair(user);
                _userRepository.UpdateTokens( 
                    DbTokenDescriptor.MapToAccessTokenByPasswordAuth(user.Id, newTokens),
                    DbTokenDescriptor.MapToRefreshTokenByPasswordAuth(user.Id, newTokens));
                
                return newTokens;
            } 
            
            return new TokensDto {
                AccessToken = tokens.Value.accessToken.Value,
                RefreshToken = tokens.Value.refreshToken.Value,
                AccessTokenKeyId = _tokenHandler.GetTokenKeyId(tokens.Value.accessToken.Value),
                RefreshTokenKeyId = _tokenHandler.GetTokenKeyId(tokens.Value.refreshToken.Value)
            };
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