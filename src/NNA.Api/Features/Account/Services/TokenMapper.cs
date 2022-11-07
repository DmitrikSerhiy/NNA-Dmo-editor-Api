using NNA.Domain.DTOs.Account;
using NNA.Domain.Entities;
using NNA.Domain.Enums;

namespace NNA.Api.Features.Account.Services;

public sealed class TokenMapper {
    public static NnaToken MapToAccessTokenByPasswordAuth(
        Guid userId,
        TokensDto tokensDto,
        LoginProviderName loginProviderName = LoginProviderName.password) {
        return new NnaToken {
            UserId = userId,
            Name = nameof(TokenName.Access),
            LoginProvider = Enum.GetName(loginProviderName),
            Value = tokensDto.AccessToken,
            TokenKeyId = tokensDto.AccessTokenKeyId
        };
    }

    public static NnaToken MapToRefreshTokenByPasswordAuth(
        Guid userId,
        TokensDto tokensDto,
        LoginProviderName loginProviderName = LoginProviderName.password) {
        return new NnaToken {
            UserId = userId,
            Name = nameof(TokenName.Refresh),
            LoginProvider = Enum.GetName(loginProviderName),
            Value = tokensDto.RefreshToken,
            TokenKeyId = tokensDto.RefreshTokenKeyId
        };
    }
}