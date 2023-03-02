using Microsoft.AspNetCore.Authorization;
using NNA.Domain;

namespace NNA.Api.Attributes; 

public sealed class SuperUserAuthorizeAttribute : AuthorizeAttribute {
    public SuperUserAuthorizeAttribute(): base(ApplicationConstants.SuperUserPolicy) { }
}