using Microsoft.Extensions.Primitives;
using NNA.Domain.Enums;

namespace NNA.Api.Helpers;

public class NnaHeaders {
    private static readonly Dictionary<NnaHeaderNames, string> Dictionary = new() {
        { NnaHeaderNames.ExpiredToken, "NNA-Token-Expired" },
        { NnaHeaderNames.RedirectToLogin, "true" }
    };

    public static KeyValuePair<string, StringValues> Get(NnaHeaderNames key) {
        return new KeyValuePair<string, StringValues>(key.ToString(), Dictionary[key]);
    }
}