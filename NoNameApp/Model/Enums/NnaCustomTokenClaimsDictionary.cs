using System.Collections.Generic;

namespace Model.Enums {
    public class NnaCustomTokenClaimsDictionary {

        private static readonly Dictionary<NnaCustomTokenClaims, string> Dictionary = new() {
                { NnaCustomTokenClaims.gTyp, "refresh_token" },
                { NnaCustomTokenClaims.oid, "" }
            };

        public static string GetValue(NnaCustomTokenClaims name) {
            return !Dictionary.TryGetValue(name, out var claimValue) ? "" : claimValue;
        }
    }
}