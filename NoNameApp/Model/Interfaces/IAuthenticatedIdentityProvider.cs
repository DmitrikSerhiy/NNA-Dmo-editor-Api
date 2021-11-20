using System;
using Model.Entities;

namespace Model.Interfaces {
    public interface IAuthenticatedIdentityProvider {
        Guid AuthenticatedUserId { get; }
        bool IsAuthenticated { get; }
        // ReSharper disable once UnusedMemberInSuper.Global
        string AuthenticatedUserEmail { get; }
        string AuthenticatedTokenId { get; }


        void SetAuthenticatedUser(UsersTokens authData);
        // ReSharper disable once UnusedMemberInSuper.Global

        void ClearAuthenticatedUserInfo();
    }
}
