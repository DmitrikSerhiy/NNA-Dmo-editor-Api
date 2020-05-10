using Model;
using Model.Entities;
using System;

namespace API.Infrastructure.Authentication {
    public class AuthenticatedIdentityProvider : IAuthenticatedIdentityProvider {
        public Guid AuthenticatedUserId { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public String AuthenticatedUserEmail { get; private set; }
        public void SetAuthenticatedUser(NoNameUser user) {
            if (user == null) throw new ArgumentNullException(nameof(user));

            IsAuthenticated = true;
            AuthenticatedUserId = user.Id;
            AuthenticatedUserEmail = user.Email;
        }

        public void ClearAuthenticatedUserInfo() {
            AuthenticatedUserId = Guid.Empty;
            IsAuthenticated = false;
            AuthenticatedUserEmail = String.Empty;
        }
    }
}
