using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace API.Helpers {
    public class NnaHeaders {
        private static readonly Dictionary<NnaHeaderNames, string> Dictionary = new() {
            { NnaHeaderNames.ExpiredToken, "NNA-Token-Expired" },
            { NnaHeaderNames.RedirectToLogin, "true" }
        };

        public static KeyValuePair<string, StringValues> Get(NnaHeaderNames key) {
            return new KeyValuePair<string, StringValues>(key.ToString(), Dictionary[key]);
        }
    }
}