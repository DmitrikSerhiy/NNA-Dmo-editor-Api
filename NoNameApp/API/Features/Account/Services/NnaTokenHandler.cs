using System;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Model.Entities;
using Model.Enums;

namespace API.Features.Account.Services {
    public class NnaTokenHandler : JsonWebTokenHandler {
        private readonly TokenDescriptorProvider _descriptorProvider;

        public NnaTokenHandler() { }
        
        public NnaTokenHandler(IOptions<JwtOptions> jwtOptions) {
            if (jwtOptions?.Value is null) throw new ArgumentNullException(nameof(jwtOptions));
            _descriptorProvider = new TokenDescriptorProvider(jwtOptions.Value);
        }

        public virtual string CreateNnaAccessToken(NnaUser user) {
            var accessTokenDescriptor = _descriptorProvider.ProvideForAccessTokenWithCredentialsAndSubject(user);
            return CreateToken(accessTokenDescriptor);
        }

        public virtual string CreateNnaRefreshToken(NnaUser user) {
            var refreshTokenDescriptor = _descriptorProvider.ProvideForRefreshTokenWithCredentialsAndSubject(user);
            return CreateToken(refreshTokenDescriptor);
        }

        public virtual TokenValidationResult ValidateAccessToken(string token, NnaUser user) {
            var accessTokenDescriptor = _descriptorProvider.ProvideForAccessTokenWithCredentialsAndSubject(user);
            return ValidateToken(token, TokenValidationParametersProvider.Provide(accessTokenDescriptor));
        }

        public virtual TokenValidationResult ValidateRefreshToken(string token, NnaUser user) {
            var refreshTokenDescriptor = _descriptorProvider.ProvideForRefreshTokenWithCredentialsAndSubject(user);
            return ValidateToken(token, TokenValidationParametersProvider.Provide(refreshTokenDescriptor));
        }

        public virtual string GetTokenKeyId(string token) {
            if (!CanReadToken(token)) {
                return null;
            }
            
            return ReadJsonWebToken(token).Claims
                .FirstOrDefault(claim => claim.Type == nameof(NnaCustomTokenClaims.oid))?.Value;
        }
        
        public virtual string GetUserEmail(string token) {
            if (!CanReadToken(token)) {
                return null;
            }
            
            return ReadJsonWebToken(token).Claims
                .FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Email)?.Value;
        }
        
        public virtual async Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string token) {
            return await GoogleJsonWebSignature.ValidateAsync(token);
        }
    }
}