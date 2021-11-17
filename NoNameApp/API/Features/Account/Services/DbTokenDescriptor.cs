using System;
using Model.DTOs.Account;
using Model.Entities;
using Model.Enums;

namespace API.Features.Account.Services {
    public class DbTokenDescriptor {
        private const string PasswordLoginProvider = "password";

        public static NnaToken MapToAccessTokenByPasswordAuth(Guid userId, TokensDto tokensDto) {
            return new NnaToken {
                UserId = userId,
                Name = nameof(TokenName.Access),
                LoginProvider = PasswordLoginProvider,
                Value = tokensDto.AccessToken,
                TokenKeyId = tokensDto.AccessTokenKeyId
            };
        }

        public static NnaToken MapToRefreshTokenByPasswordAuth(Guid userId, TokensDto tokensDto) {
            return new NnaToken {
                UserId = userId,
                Name = nameof(TokenName.Refresh),
                LoginProvider = PasswordLoginProvider,
                Value = tokensDto.RefreshToken,
                TokenKeyId = tokensDto.RefreshTokenKeyId
            };
        }
        
    }
}