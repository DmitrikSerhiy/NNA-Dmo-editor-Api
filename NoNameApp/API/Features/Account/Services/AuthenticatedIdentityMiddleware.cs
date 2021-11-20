using System;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Model.Enums;
using Model.Interfaces;
using Model.Interfaces.Repositories;

namespace API.Features.Account.Services {
    public class AuthenticatedIdentityMiddleware {
        private readonly RequestDelegate _next;
        private readonly IUserRepository _repository;

        public AuthenticatedIdentityMiddleware(
            RequestDelegate next,
            IUserRepository repository) {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }
        
        // todo: cover with unit tests
        // todo: add rate-limit for non secured end-points
        // todo: return 403 if token is invalid. Create special token.
        public async Task InvokeAsync(HttpContext context, IAuthenticatedIdentityProvider authenticatedIdentityProvider) {
            if (!(context.User.Claims.Any(claim => claim.Type.Equals(ClaimTypes.Email)) && 
                context.User.Claims.Any(claim => claim.Type.Equals(ClaimTypes.NameIdentifier)) &&
                context.User.Claims.Any(claim => claim.Type.Equals(NnaCustomTokenClaimsDictionary.GetValue(NnaCustomTokenClaims.oid))))) {
                // for non secured end-points
                await _next.Invoke(context);
                return;
            }
            

            if (context.User.Claims.Any(claim => claim.Type == nameof(NnaCustomTokenClaims.gtyp))) {
                throw new AuthenticationException($"Invalid token. Refresh token should not be used as authentication key.");
            }
            
            var userId = context.User.Claims.First(claim => claim.Type.Equals(ClaimTypes.NameIdentifier)).Value;
            var userEmail = context.User.Claims.First(claim => claim.Type.Equals(ClaimTypes.Email)).Value;
            var tokenId = context.User.Claims.First(claim => 
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
            
            if (authData.Email != userEmail ||
                authData.UserId.ToString() != userId ||
                authData.AccessTokenId != tokenId) {
                throw new AuthenticationException($"Inconsistent auth data for user: '{userEmail}'");
            }
            
            authenticatedIdentityProvider.SetAuthenticatedUser(authData);
            await _next.Invoke(context);
        }
    }
}
