namespace NNA.Domain.Enums;

public sealed class NnaCustomTokenClaimsDictionary {
    private static readonly Dictionary<NnaCustomTokenClaims, string> Dictionary = new() {
        { NnaCustomTokenClaims.gtyp, "refresh_token" },
        { NnaCustomTokenClaims.oid, "http://schemas.microsoft.com/identity/claims/objectidentifier" },
        { NnaCustomTokenClaims.rls, "roles" }
    };

    public static string GetValue(NnaCustomTokenClaims name) {
        return !Dictionary.TryGetValue(name, out var claimValue) ? "" : claimValue;
    }
}