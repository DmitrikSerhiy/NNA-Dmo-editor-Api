using NNA.Domain.Entities;
using NNA.Domain.Interfaces;

namespace NNA.Api.Features.Account.Services;
public sealed class AuthenticatedIdentityProvider : IAuthenticatedIdentityProvider {
    public Guid AuthenticatedUserId { get; private set; }
    public bool IsAuthenticated { get; private set; }
    public string AuthenticatedUserEmail { get; private set; } = null!;
    public string AuthenticatedTokenId { get; private set; } = null!;
        
    public void SetAuthenticatedUser(UsersTokens authData) {
        if (authData == null) throw new ArgumentNullException(nameof(authData));

        IsAuthenticated = true;
        AuthenticatedUserId = authData.UserId;
        AuthenticatedUserEmail = authData.Email;
        AuthenticatedTokenId = authData.AccessTokenId;
    }

    public void ClearAuthenticatedUserInfo() {
        AuthenticatedUserId = Guid.Empty;
        IsAuthenticated = false;
        AuthenticatedUserEmail = string.Empty;
        AuthenticatedTokenId = string.Empty;
    }
}