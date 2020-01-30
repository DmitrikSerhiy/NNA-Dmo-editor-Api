using System;
using Model.Entities;

namespace Model {
    public interface IAuthenticatedIdentityProvider {
        Guid AuthenticatedUserId { get; }
        Boolean IsAuthenticated { get; }

        void SetAuthenticatedUser(NoNameUser user);
    }
}
