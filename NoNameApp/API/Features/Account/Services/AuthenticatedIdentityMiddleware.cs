using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Model.Interfaces;

namespace API.Features.Account.Services {
    public class AuthenticatedIdentityMiddleware {
        private readonly RequestDelegate _next;
        private readonly NnaUserManager _userManager;

        public AuthenticatedIdentityMiddleware(RequestDelegate next, NnaUserManager userManager) {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        // ReSharper disable once UnusedMember.Global
        public async Task InvokeAsync(HttpContext context, IAuthenticatedIdentityProvider authenticatedIdentityProvider) {
            var userName = context.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
            //todo: change it to get nameIdentifier instead of name
            if (userName == null) {
                await _next.Invoke(context);
                return;
            }

            //todo: isn't it overhead? To get user by userManager and then by repository. Should think of it later. Should profile db calls.
            var user = await _userManager.FindByNameAsync(userName.Value);
            if (user == null) {
                throw new AuthenticationException($"There is no user with name {userName}");
            }

            authenticatedIdentityProvider.SetAuthenticatedUser(user);
            await _next.Invoke(context);
        }
    }
}
