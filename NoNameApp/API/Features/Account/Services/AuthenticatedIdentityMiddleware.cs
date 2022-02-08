using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Helpers;
using Microsoft.AspNetCore.Http;
using Model.Enums;
using Model.Interfaces;

namespace API.Features.Account.Services {
    public class AuthenticatedIdentityMiddleware {
        private readonly RequestDelegate _next;
        private readonly ClaimsValidator _claimsValidator;

        public AuthenticatedIdentityMiddleware(
            RequestDelegate next, 
            ClaimsValidator claimsValidator) {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _claimsValidator = claimsValidator ?? throw new ArgumentNullException(nameof(claimsValidator));
        }
        
        public async Task InvokeAsync(HttpContext context, IAuthenticatedIdentityProvider authenticatedIdentityProvider) {
            if (!(context.User.Claims.Any(claim => claim.Type.Equals(ClaimTypes.Email)) && 
                context.User.Claims.Any(claim => claim.Type.Equals(ClaimTypes.NameIdentifier)) &&
                context.User.Claims.Any(claim => claim.Type.Equals(NnaCustomTokenClaimsDictionary.GetValue(NnaCustomTokenClaims.oid))))) {
                // for non-secured end-points
                await _next.Invoke(context);
                return;
            }

            var authData = await _claimsValidator.ValidateAndGetAuthDataAsync(context.User.Claims.ToList());
            authenticatedIdentityProvider.SetAuthenticatedUser(authData);
            await _next.Invoke(context);
        }
    }
}
