using NNA.Domain.Entities;

namespace NNA.Domain.Extensions;

public static class NnaUserExtensions {
    private const char ProvidersSeparator = ',';

    public static string[] GetAuthProviders(this NnaUser? user) {
        if (user == null) {
            return Array.Empty<string>();
        }

        return user.AuthProviders == null
            ? Array.Empty<string>()
            : user.AuthProviders.Split(ProvidersSeparator).ToArray();
    }

    public static void AddAuthProvider(this NnaUser? user, string? providerName) {
        if (user == null || providerName == null) {
            return;
        }

        if (user.AuthProviders == null) {
            user.AuthProviders = providerName;
            return;
        }

        if (user.AuthProviders.Contains(providerName)) {
            return;
        }

        user.AuthProviders += $"{ProvidersSeparator}{providerName}";
    }

    public static void RemoveAuthProvider(this NnaUser? user, string providerName) {
        if (user?.AuthProviders == null) {
            return;
        }

        if (!user.AuthProviders.Contains(providerName)) {
            return;
        }

        if (user.AuthProviders == providerName) {
            user.AuthProviders = null;
            return;
        }

        user.AuthProviders = user.AuthProviders.Replace($"{ProvidersSeparator}{providerName}", "");
    }
}