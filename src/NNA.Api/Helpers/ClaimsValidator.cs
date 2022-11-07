using System.Security.Authentication;
using System.Security.Claims;
using NNA.Domain.Entities;
using NNA.Domain.Enums;
using NNA.Domain.Interfaces.Repositories;

namespace NNA.Api.Helpers;

public sealed class ClaimsValidator {
    private readonly IUserRepository _repository = null!;

    public ClaimsValidator() { }

    public ClaimsValidator(IUserRepository repository) {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<UsersTokens> ValidateAndGetAuthDataAsync(List<Claim> claims, CancellationToken cancellationToken) {
        if (claims.Any(claim => claim.Type == nameof(NnaCustomTokenClaims.gtyp))) {
            throw new AuthenticationException("Invalid token. Refresh token should not be used as authentication key");
        }

        var userId = claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.NameIdentifier))?.Value;
        if (string.IsNullOrWhiteSpace(userId)) {
            throw new AuthenticationException("User id claim is missing");
        }

        var userEmail = claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.Email))?.Value;
        if (string.IsNullOrWhiteSpace(userEmail)) {
            throw new AuthenticationException("User email claim is missing");
        }

        var tokenId = claims.FirstOrDefault(claim =>
            claim.Type.Equals(NnaCustomTokenClaimsDictionary.GetValue(NnaCustomTokenClaims.oid)))?.Value;
        if (string.IsNullOrWhiteSpace(tokenId)) {
            throw new AuthenticationException("User oid claim is missing");
        }

        var authData = await _repository.GetAuthenticatedUserDataAsync(userEmail, cancellationToken);
        if (authData is null) {
            throw new AuthenticationException($"Authentication data for '{userEmail}' is not saved");
        }

        if (authData.Email != userEmail ||
            authData.UserId.ToString() != userId ||
            authData.AccessTokenId != tokenId) {
            throw new AuthenticationException($"Inconsistent auth data for user: '{userEmail}'");
        }

        return authData;
    }
}