using System.Net.Mime;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization.Policy;
using NNA.Api.Helpers;
using NNA.Domain.Enums;

namespace NNA.Api.Features.Account.Services; 

public sealed class NnaAuthorizationMiddleware : IAuthorizationMiddlewareResultHandler {
    
    public async Task HandleAsync(
        RequestDelegate next, 
        HttpContext context, 
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult) {

        var userRoles =
            context.User.Claims.FirstOrDefault(claim => claim.Type.Equals(Enum.GetName(NnaCustomTokenClaims.rls)))?.Value ?? null;

        if (userRoles is null) {
            await NnaForbidAsync(context, "User role is missing.");
            return;
        }

        var highestRole = GetHighestRole(userRoles);
        if (string.IsNullOrEmpty(highestRole)) {
            await NnaForbidAsync(context, "Invalid user role claim.");
            return;
        }

        var shouldForbid = false;
        foreach (var requirement in policy.Requirements.Where(requirement => requirement is ClaimsAuthorizationRequirement)) {
            var containsAllowedRole = (requirement as ClaimsAuthorizationRequirement)!.AllowedValues!.Contains(highestRole);
            if (containsAllowedRole == false) {
                shouldForbid = true;
                break;
            }
        }

        if (shouldForbid) {
            await NnaForbidAsync(context, "Not authorized attempt to access resource");
            return;
        }
        
        await next(context);    
    }
    
    private async Task NnaForbidAsync(HttpContext context, string reason) {
        await context.ForbidAsync();
        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.Headers.Add(NnaHeaders.Get(NnaHeaderNames.RedirectToLogin));

        await context.Response.WriteAsJsonAsync(new
        {
            fromExceptionMiddleware = true,
            title = "Authorization error",
            message = reason
        });
    }
    
    private string GetHighestRole(string userRoles) {
        var roles = userRoles.Split(",");
        if (roles.Contains(Enum.GetName(NnaRoles.SuperUser))) {
            return Enum.GetName(NnaRoles.SuperUser)!;
        } 
        
        if (roles.Contains(Enum.GetName(NnaRoles.ActiveUser))) {
            return Enum.GetName(NnaRoles.ActiveUser)!;
        } 
        
        if (roles.Contains(Enum.GetName(NnaRoles.NotActiveUser))) {
            return Enum.GetName(NnaRoles.NotActiveUser)!;
        }

        return "";
    }
    
}