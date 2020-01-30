using Model;
using Model.Entities;
using System;

namespace API.Infrastructure.Authentication
{
    public class AuthenticatedIdentityProvider : IAuthenticatedIdentityProvider {
        public Guid AuthenticatedUserId { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public void SetAuthenticatedUser(NoNameUser user) {
            if (user == null) throw new ArgumentNullException(nameof(user));

            IsAuthenticated = true;
            AuthenticatedUserId = user.Id;
        }
    }
}
