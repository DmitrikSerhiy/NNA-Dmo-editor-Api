using System;
using System.Collections.Generic;
using API.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace API.Features.Account.Services {
    public class TokenDescriptorProvider {

        private readonly JwtOptions _jwtOptions;
        public TokenDescriptorProvider(IOptions<JwtOptions> jwtOptions) {
            _jwtOptions = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions));
        }

        public SecurityTokenDescriptor ProvideForAccessToken() {
            return BuildDescriptor();
        }

        public SecurityTokenDescriptor ProvideForRefreshToken() {
            var descriptor = BuildDescriptor();
            descriptor.Expires = DateTime.UtcNow + TimeSpan.FromHours(_jwtOptions.TokenLifetimeInHours * 2);
            descriptor.Claims = new Dictionary<string, object> {
                { "gtyp", "refresh_token" }
            };

            return descriptor;
        }

        private SecurityTokenDescriptor BuildDescriptor() {
            return new SecurityTokenDescriptor {
                Audience = _jwtOptions.Audience,
                Expires = DateTime.UtcNow + TimeSpan.FromHours(_jwtOptions.TokenLifetimeInHours),
                Issuer = _jwtOptions.Issuer,
                NotBefore = DateTime.UtcNow,
                IssuedAt = DateTime.UtcNow,
                TokenType = "JWT",
                AdditionalHeaderClaims = new Dictionary<string, object> {
                    { JwtHeaderParameterNames.Kid, Guid.NewGuid().ToString() }
                }
            };
        }
    }
}