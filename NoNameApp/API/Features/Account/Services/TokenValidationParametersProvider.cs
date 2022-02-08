using System;
using Microsoft.IdentityModel.Tokens;

namespace API.Features.Account.Services {
    internal sealed class TokenValidationParametersProvider {

        internal static TokenValidationParameters Provide(SecurityTokenDescriptor tokenDescriptor) {
            return new TokenValidationParameters {
                ValidAlgorithms = new[] { tokenDescriptor.SigningCredentials.Algorithm },
                ValidAudience = tokenDescriptor.Audience,
                ValidIssuer = tokenDescriptor.Issuer,

                ValidateIssuerSigningKey = true,
                RequireSignedTokens = true,
                IssuerSigningKey = tokenDescriptor.SigningCredentials.Key,

                ValidateLifetime = true,
                RequireExpirationTime = true,

                ValidTypes = new[] { tokenDescriptor.TokenType },
                ClockSkew = TimeSpan.Zero
            };
        }
    }
}