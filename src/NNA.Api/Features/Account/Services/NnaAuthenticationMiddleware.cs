﻿using System.Net;
using System.Net.Mime;
using System.Security.Authentication;
using System.Security.Claims;
using NNA.Api.Helpers;
using NNA.Domain.Enums;
using NNA.Domain.Interfaces;

namespace NNA.Api.Features.Account.Services;

public sealed class NnaAuthenticationMiddleware {
    private readonly RequestDelegate _next;

    public NnaAuthenticationMiddleware(RequestDelegate next) {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(
        HttpContext context, 
        IAuthenticatedIdentityProvider authenticatedIdentityProvider,
        ClaimsValidator claimsValidator) {
        if (!(
                context.User.Claims.Any(claim => claim.Type.Equals(ClaimTypes.Email)) &&
                context.User.Claims.Any(claim => claim.Type.Equals(ClaimTypes.NameIdentifier)) &&
                context.User.Claims.Any(claim =>claim.Type.Equals(NnaCustomTokenClaimsDictionary.GetValue(NnaCustomTokenClaims.oid)))
            )) {
            // for non-secured end-points
            await _next.Invoke(context);
            return;
        }

        try {
            var authData =
                await claimsValidator.ValidateAndGetAuthDataAsync(context.User.Claims.ToList(), CancellationToken.None);
            authenticatedIdentityProvider.SetAuthenticatedUser(authData);
        }
        catch (AuthenticationException ex) {
            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.Headers.Add(NnaHeaders.Get(NnaHeaderNames.RedirectToLogin));

            await context.Response.WriteAsJsonAsync(new
            {
                fromExceptionMiddleware = true,
                title = "Authentication error",
                message = ex.Message
            });
            return;
        }

        await _next.Invoke(context);
    }
}