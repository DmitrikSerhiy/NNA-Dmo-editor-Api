using System;
using Model.Entities;

namespace Model {
    public interface IAuthenticatedIdentityProvider {
        Guid AuthenticatedUserId { get; }
        Boolean IsAuthenticated { get; }
        String AuthenticatedUserEmail { get; }

        void SetAuthenticatedUser(NoNameUser user);
        void ClearAuthenticatedUserInfo();
    }
}
