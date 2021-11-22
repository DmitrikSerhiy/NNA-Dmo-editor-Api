using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Model.Entities;
using Model.Enums;
using Model.Interfaces.Repositories;

namespace API.Helpers {
    public class ClaimsValidator {

        private readonly IUserRepository _repository;

        public ClaimsValidator() { }
        
        public ClaimsValidator(IUserRepository repository) {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        
        public async Task<UsersTokens> ValidateAndGetAuthDataAsync(List<Claim> claims) {
            if (claims.Any(claim => claim.Type == nameof(NnaCustomTokenClaims.gtyp))) {
                throw new AuthenticationException($"Invalid token. Refresh token should not be used as authentication key.");
            }
            
            var userId = claims.First(claim => claim.Type.Equals(ClaimTypes.NameIdentifier)).Value;
            var userEmail = claims.First(claim => claim.Type.Equals(ClaimTypes.Email)).Value;
            var tokenId = claims.First(claim => 
                claim.Type.Equals(NnaCustomTokenClaimsDictionary.GetValue(NnaCustomTokenClaims.oid))).Value;
            
            if (string.IsNullOrWhiteSpace(userId)) {
                throw new AuthenticationException($"Invalid user id claim: '{userId}'");
            }
            
            if (string.IsNullOrWhiteSpace(userEmail)) {
                throw new AuthenticationException($"Invalid user email claim: '{userEmail}'");
            }

            if (string.IsNullOrWhiteSpace(tokenId)) {
                throw new AuthenticationException($"Invalid user oid claim: '{tokenId}'");
            }
            
            var authData = await _repository.GetAuthenticatedUserDataAsync(userEmail);
            if (authData is null) {
                throw new AuthenticationException($"Missing auth data for user: '{userEmail}'");
            }
            
            if (authData is null) {
                throw new AuthenticationException($"Missing auth data for user: '{userEmail}'");
            }
            
            if (authData.Email != userEmail ||
                authData.UserId.ToString() != userId ||
                authData.AccessTokenId != tokenId) {
                throw new AuthenticationException($"Inconsistent auth data for user: '{userEmail}'");
            }

            return authData;
        }
    }
}