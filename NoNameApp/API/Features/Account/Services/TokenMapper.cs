using System;
using Model.DTOs.Account;
using Model.Entities;
using Model.Enums;

namespace API.Features.Account.Services {
    public class TokenMapper {
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
}