using System;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Helpers.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Model.Entities;
using Model.Interfaces;

namespace API.Features.Account.Services.Local {
    public class AuthenticatedIdentityMiddleware {
        private readonly RequestDelegate _next;
        private readonly UserManager<NnaUser> _userManager;

        public AuthenticatedIdentityMiddleware(RequestDelegate next, IServiceProvider serviceProvider, IWebHostEnvironment environment ) {
            _next = next ?? throw new ArgumentNullException(nameof(next));

            _userManager = environment.IsLocal()
                ? serviceProvider.GetService<NnaLocalUserManager>()
                : serviceProvider.GetService<NnaUserManager>();
        }
        // todo: cover with unit tests
        public async Task InvokeAsync(HttpContext context, IAuthenticatedIdentityProvider authenticatedIdentityProvider) {
            if (!(context.User.Claims.Any(claim => claim.Type.Equals(ClaimTypes.Email)) && 
                context.User.Claims.Any(claim => claim.Type.Equals(ClaimTypes.NameIdentifier)))) {
                // for not secured end-points
                await _next.Invoke(context);
                return;
            }

            var userId = context.User.Claims.First(claim => claim.Type.Equals(ClaimTypes.NameIdentifier)).Value;
            var userEmail = context.User.Claims.First(claim => claim.Type.Equals(ClaimTypes.Email)).Value;

            if (string.IsNullOrWhiteSpace(userId)) {
                throw new AuthenticationException($"Invalid user id: '{userId}'");
            }
            
            if (string.IsNullOrWhiteSpace(userEmail)) {
                throw new AuthenticationException($"Invalid user email: '{userEmail}'");
            }
            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) {
                throw new AuthenticationException($"Unknown user with id: '{userId}'");
            }

            if (!user.Email.Equals(userEmail, StringComparison.InvariantCultureIgnoreCase)) {
                throw new AuthenticationException($"User email '{userEmail}' does not correspond to user id: '{userId}'");
            } 
            
            authenticatedIdentityProvider.SetAuthenticatedUser(user);
            await _next.Invoke(context);
        }
    }
}
