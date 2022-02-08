using System;
using System.Collections.Generic;
using API.Helpers;
using API.Helpers.Extensions;
using Microsoft.IdentityModel.Tokens;
using Model.Entities;
using Model.Enums;

namespace API.Features.Account.Services {
    public class TokenDescriptorProvider {

        private readonly JwtOptions _jwtOptions;
        public TokenDescriptorProvider(JwtOptions jwtOptions) {
            _jwtOptions = jwtOptions ?? throw new ArgumentNullException(nameof(jwtOptions));
        }

        public SecurityTokenDescriptor ProvideForAccessTokenWithCredentialsAndSubject(NnaUser user) {
            var accessTokenDescriptor = ProvideForAccessToken();
            accessTokenDescriptor.AddSigningCredentials(_jwtOptions);
            accessTokenDescriptor.AddSubjectClaims(user.Email, user.Id);

            return accessTokenDescriptor;
        }
        
        public SecurityTokenDescriptor ProvideForRefreshTokenWithCredentialsAndSubject(NnaUser user) {
            var refreshTokenDescriptor = ProvideForRefreshToken();
            refreshTokenDescriptor.AddSigningCredentials(_jwtOptions);
            refreshTokenDescriptor.AddSubjectClaims(user.Email, user.Id);

            return refreshTokenDescriptor;
        }

        public SecurityTokenDescriptor ProvideForAccessToken() {
            return BuildDescriptor();
        }

        public SecurityTokenDescriptor ProvideForRefreshToken() {
            var descriptor = BuildDescriptor();
            descriptor.Expires = DateTime.UtcNow + TimeSpan.FromHours(_jwtOptions.TokenLifetimeInHours * 2);
            descriptor.Claims.Add(
                nameof(NnaCustomTokenClaims.gtyp),
                NnaCustomTokenClaimsDictionary.GetValue(NnaCustomTokenClaims.gtyp));
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
                Claims = new Dictionary<string, object> {
                    { nameof(NnaCustomTokenClaims.oid), Guid.NewGuid() }
                }
            };
        }
    }
}