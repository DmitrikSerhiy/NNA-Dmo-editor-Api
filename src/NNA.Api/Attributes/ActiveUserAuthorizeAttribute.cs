using Microsoft.AspNetCore.Authorization;
using NNA.Domain;

namespace NNA.Api.Attributes; 

public sealed class ActiveUserAuthorizeAttribute : AuthorizeAttribute {
    public ActiveUserAuthorizeAttribute() : base(ApplicationConstants.ActiveUserPolicy) { }
}