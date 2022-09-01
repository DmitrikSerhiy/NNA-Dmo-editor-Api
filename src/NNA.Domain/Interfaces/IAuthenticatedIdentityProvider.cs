using NNA.Domain.Entities;

namespace NNA.Domain.Interfaces;

public interface IAuthenticatedIdentityProvider {
    Guid AuthenticatedUserId { get; }
    bool IsAuthenticated { get; }
    string AuthenticatedUserEmail { get; }
    string AuthenticatedTokenId { get; }

    void SetAuthenticatedUser(UsersTokens authData);
    void ClearAuthenticatedUserInfo();
}