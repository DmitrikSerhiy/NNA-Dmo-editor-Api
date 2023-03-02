using Microsoft.AspNetCore.Authorization;
using NNA.Domain;

namespace NNA.Api.Attributes; 

public sealed class NotActiveUserAuthorizeAttribute : AuthorizeAttribute {
    public NotActiveUserAuthorizeAttribute(): base(ApplicationConstants.NotActiveUserPolicy) { }
}