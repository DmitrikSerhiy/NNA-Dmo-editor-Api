using Microsoft.AspNetCore.Identity;

namespace NNA.Domain.Entities; 
public sealed class NnaToken : IdentityUserToken<Guid> {
    public string TokenKeyId { get; set; } = null!;
}