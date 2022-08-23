using Google.Apis.Auth;
using Model.DTOs.Account;
using Model.Entities;
using Model.Enums;
using Model.Interfaces;
using Model.Interfaces.Repositories;

namespace NNA.Api.Features.Account.Services;
public sealed class NnaTokenManager {
        
    private readonly IUserRepository _userRepository;
    private readonly NnaTokenHandler _nnaTokenHandler;
    private readonly IAuthenticatedIdentityProvider _identityProvider;

    public NnaTokenManager(
        IUserRepository userRepository, 
        NnaTokenHandler nnaTokenHandler,
        IAuthenticatedIdentityProvider identityProvider) {

        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _identityProvider = identityProvider ?? throw new ArgumentNullException(nameof(identityProvider));
        _nnaTokenHandler = nnaTokenHandler ?? throw new ArgumentNullException(nameof(nnaTokenHandler));
    }

    /// <summary>
    /// Creates new valid pair of access/refresh tokens for user with specified email.
    /// All previous tokens for that user become invalid.
    /// </summary>
    public async Task<TokensDto> CreateTokensAsync(NnaUser user, LoginProviderName loginProviderName = LoginProviderName.password) {
        if (user == null) throw new ArgumentNullException(nameof(user));
            
        return await GenerateAndSaveTokensAsync(user, loginProviderName);
    }

    /// <summary>
    /// Check whether current user has tokens.
    /// </summary>
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
    public async Task<TokensDto> GetOrCreateTokensAsync(NnaUser user, LoginProviderName loginProviderName = LoginProviderName.password) {
        if (user == null) throw new ArgumentNullException(nameof(user));
        var tokens = await _userRepository.GetTokens(user.Id);
            
        if (tokens is null) {
            return await GenerateAndSaveTokensAsync(user, loginProviderName);
        }

        var accessTokenValidation = _nnaTokenHandler.ValidateAccessToken(tokens.Value.accessToken.Value, user);
        var refreshTokenValidation = _nnaTokenHandler.ValidateRefreshToken(tokens.Value.refreshToken.Value, user);
            
        if (!accessTokenValidation.IsValid || !refreshTokenValidation.IsValid) {
            return GenerateAndUpdateTokens(user, loginProviderName);
        } 
            
        return new TokensDto {
            AccessToken = tokens.Value.accessToken.Value,
            RefreshToken = tokens.Value.refreshToken.Value,
            AccessTokenKeyId = _nnaTokenHandler.GetTokenKeyId(tokens.Value.accessToken.Value),
            RefreshTokenKeyId = _nnaTokenHandler.GetTokenKeyId(tokens.Value.refreshToken.Value)
        };
    }
        
    /// <summary>
    /// Creates or Updates new access/refresh tokens pair if old access/refresh pair corresponds to each other.
    /// </summary>
    public async Task<TokensDto?> RefreshTokensAsync(RefreshDto refreshDto) {
        if (string.IsNullOrWhiteSpace(refreshDto.AccessToken)) throw new ArgumentNullException(nameof(refreshDto.AccessToken));
        if (string.IsNullOrWhiteSpace(refreshDto.RefreshToken)) throw new ArgumentNullException(nameof(refreshDto.RefreshToken));
        if (refreshDto.AccessToken == refreshDto.RefreshToken) throw new ArgumentException();

        var userEmailFromAccessToken = _nnaTokenHandler.GetUserEmail(refreshDto.AccessToken);
        if (userEmailFromAccessToken is null) {
            return null;
        }
            
        var userEmailFromRefreshToken = _nnaTokenHandler.GetUserEmail(refreshDto.RefreshToken);
        if (userEmailFromRefreshToken is null) {
            return null;
        }
            
        if (userEmailFromRefreshToken != userEmailFromAccessToken) {
            return null;
        }

        var authData = await _userRepository.GetAuthenticatedUserDataAsync(userEmailFromRefreshToken);
        if (authData is null) {
            return null;
        }

        if (_nnaTokenHandler.GetTokenKeyId(refreshDto.AccessToken) != authData.AccessTokenId) {
            return null;
        }
                
        if (_nnaTokenHandler.GetTokenKeyId(refreshDto.RefreshToken) != authData.RefreshTokenId) {
            return null;
        }

        var user = new NnaUser {
            Email = authData.Email,
            Id = authData.UserId
        };

        var refreshValidationResult = _nnaTokenHandler.ValidateRefreshToken(refreshDto.RefreshToken, user);
        if (!refreshValidationResult.IsValid) {
            return null;
        }
            
        return GenerateAndUpdateTokens(user, Enum.Parse<LoginProviderName>(authData.LoginProvider));
    }

    /// <summary>
    /// Remove all user tokens. Clear authenticated data.
    /// </summary>
    public async Task ClearTokensAsync(NnaUser user) {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (_identityProvider.AuthenticatedUserEmail != user.Email) {
            return;
        }
            
        await _userRepository.ClearTokens(user);
        _identityProvider.ClearAuthenticatedUserInfo();
    }

    /// <summary>
    /// Validate token provided by Google
    /// </summary>
    public async Task<bool> ValidateGoogleTokenAsync(AuthGoogleDto authGoogleDto) {
        if (authGoogleDto.Email is null) throw new ArgumentNullException(nameof(authGoogleDto.Email));
            
        try {
            var payload = await _nnaTokenHandler.ValidateGoogleTokenAsync(authGoogleDto.GoogleToken);
            if (!payload.EmailVerified || payload.Email != authGoogleDto.Email) {
                return false;
            }
        } catch (InvalidJwtException) {
            return false;
        }

        return true;
    }


        

    private TokensDto GenerateAndUpdateTokens(NnaUser user, LoginProviderName loginProviderName = LoginProviderName.password) {
        var newTokens = GenerateNewTokenPair(user);
            
        _userRepository.UpdateTokens( 
            TokenMapper.MapToAccessTokenByPasswordAuth(user.Id, newTokens, loginProviderName),
            TokenMapper.MapToRefreshTokenByPasswordAuth(user.Id, newTokens, loginProviderName));
            
        return newTokens;
    }

    private async Task<TokensDto> GenerateAndSaveTokensAsync(NnaUser user, LoginProviderName loginProviderName = LoginProviderName.password) {
        var newTokens = GenerateNewTokenPair(user);
            
        await _userRepository.SaveTokens(                  
            TokenMapper.MapToAccessTokenByPasswordAuth(user.Id, newTokens, loginProviderName),
            TokenMapper.MapToRefreshTokenByPasswordAuth(user.Id, newTokens, loginProviderName));

        return newTokens;
    }
        
    private TokensDto GenerateNewTokenPair(NnaUser user) {
        var accessToken = _nnaTokenHandler.CreateNnaAccessToken(user);
        var refreshToken = _nnaTokenHandler.CreateNnaRefreshToken(user);
                
        return new TokensDto {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenKeyId = _nnaTokenHandler.GetTokenKeyId(accessToken),
            RefreshTokenKeyId = _nnaTokenHandler.GetTokenKeyId(refreshToken)
        };
            
            
    }
}