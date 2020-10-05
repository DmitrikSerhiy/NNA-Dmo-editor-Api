using System;
using Model.Entities;

namespace Model {
    public interface IAuthenticatedIdentityProvider {
        Guid AuthenticatedUserId { get; }
        bool IsAuthenticated { get; }
        // ReSharper disable once UnusedMemberInSuper.Global
        string AuthenticatedUserEmail { get; }

        void SetAuthenticatedUser(NoNameUser user);
        // ReSharper disable once UnusedMemberInSuper.Global

        void ClearAuthenticatedUserInfo();
    }
}
