using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using NNA.Api.Features.Account.Services;

namespace NNA.Api.Helpers; 

public class NnaUserTelemetryInitializer : ITelemetryInitializer {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public NnaUserTelemetryInitializer(IHttpContextAccessor httpContextAccessor) {
        _httpContextAccessor = httpContextAccessor;
    }

    public void Initialize(ITelemetry telemetry) {
        var requestTelemetry = telemetry as RequestTelemetry;

        if (requestTelemetry == null) {
            return;
        }

        if (_httpContextAccessor.HttpContext?.Request.Method == "OPTIONS") {
            return;
        }

        if (_httpContextAccessor.HttpContext?.User is null) {
            return;
        }

        var userId = ClaimsValidator.GetUserIdFromClaims(_httpContextAccessor.HttpContext?.User.Claims);
        var userEmail = ClaimsValidator.GetUserEmailFromClaims(_httpContextAccessor.HttpContext?.User.Claims);
        var userRoles = ClaimsValidator.GetUserRoleFromClaims(_httpContextAccessor.HttpContext?.User.Claims);
        
        if (userId != null) {
            requestTelemetry.Context.User.Id = userId;
        }

        if (userEmail != null) {
            requestTelemetry.Context.User.AccountId = userEmail;
        }

        if (userRoles != null) {
            var highestRole = NnaAuthorizationMiddleware.GetHighestRole(userRoles);
            requestTelemetry.Context.User.AuthenticatedUserId = highestRole;
        }
    }
}