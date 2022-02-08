using System;
using System.Linq;
using API.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Model.Entities;
using Model.Enums;

namespace API.Features.Account.Services {
    public sealed class NnaTokenHandler : JsonWebTokenHandler {
        private readonly TokenDescriptorProvider _descriptorProvider;
        
        public NnaTokenHandler(IOptions<JwtOptions> jwtOptions) {
            if (jwtOptions?.Value is null) throw new ArgumentNullException(nameof(jwtOptions));
            _descriptorProvider = new TokenDescriptorProvider(jwtOptions.Value);
        }

        internal string CreateNnaAccessToken(NnaUser user) {
            var accessTokenDescriptor = _descriptorProvider.ProvideForAccessTokenWithCredentialsAndSubject(user);
            return CreateToken(accessTokenDescriptor);
        }

        internal string CreateNnaRefreshToken(NnaUser user) {
            var refreshTokenDescriptor = _descriptorProvider.ProvideForRefreshTokenWithCredentialsAndSubject(user);
            return CreateToken(refreshTokenDescriptor);
        }

        internal TokenValidationResult ValidateAccessToken(string token, NnaUser user) {
            var accessTokenDescriptor = _descriptorProvider.ProvideForAccessTokenWithCredentialsAndSubject(user);
            return ValidateToken(token, TokenValidationParametersProvider.Provide(accessTokenDescriptor));
        }

        internal TokenValidationResult ValidateRefreshToken(string token, NnaUser user) {
            var refreshTokenDescriptor = _descriptorProvider.ProvideForRefreshTokenWithCredentialsAndSubject(user);
            return ValidateToken(token, TokenValidationParametersProvider.Provide(refreshTokenDescriptor));
        }

        internal string GetTokenKeyId(string token) {
            if (!CanReadToken(token)) {
                return null;
            }
            
            return ReadJsonWebToken(token).Claims
                .FirstOrDefault(claim => claim.Type == nameof(NnaCustomTokenClaims.oid))?.Value;
        }
        
        internal string GetUserEmail(string token) {
            if (!CanReadToken(token)) {
                return null;
            }
            
            return ReadJsonWebToken(token).Claims
                .FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Email)?.Value;
        }
    }
}