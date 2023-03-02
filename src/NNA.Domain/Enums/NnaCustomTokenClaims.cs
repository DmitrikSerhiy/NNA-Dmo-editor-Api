namespace NNA.Domain.Enums;

public enum NnaCustomTokenClaims {
    // ReSharper disable twice InconsistentNaming
    // ReSharper disable once IdentifierTypo
    gtyp, // grand type
    oid, // unique token id for all user sessions // type by asp.net: "http://schemas.microsoft.com/identity/claims/objectidentifier"
    rls // used instead of http://schemas.microsoft.com/ws/2008/06/identity/claims/role to reduce general token length
}