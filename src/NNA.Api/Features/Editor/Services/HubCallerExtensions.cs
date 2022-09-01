using Microsoft.AspNetCore.SignalR;
using NNA.Api.Features.Account.Services;
using NNA.Domain.Entities;

namespace NNA.Api.Features.Editor.Services;

public static class HubCallerExtensions {
    public static Guid? GetCurrentUserId(this HubCallerContext context) {
        if (!context.Items.TryGetValue("user", out var authProvider)) {
            return null;
        }

        // ReSharper disable once PossibleNullReferenceException
        return (authProvider as AuthenticatedIdentityProvider)!.AuthenticatedUserId;
    }

    public static string? GetCurrentUserEmail(this HubCallerContext context) {
        return context.Items.TryGetValue("user", out var authProvider)
            // ReSharper disable once PossibleNullReferenceException
            ? (authProvider as AuthenticatedIdentityProvider)!.AuthenticatedUserEmail
            : null;
    }

    public static bool ContainsUser(this HubCallerContext context) {
        return context.Items.Keys.Any(key => key.Equals("user"));
    }

    public static void AuthenticateUser(this HubCallerContext context, UsersTokens authData) {
        var authenticatedIdentityProvider = new AuthenticatedIdentityProvider();
        authenticatedIdentityProvider.SetAuthenticatedUser(authData);
        context.Items.Add("user", authenticatedIdentityProvider);
    }

    public static void LogoutUser(this HubCallerContext context) {
        context.Items.Remove("user");
    }
}