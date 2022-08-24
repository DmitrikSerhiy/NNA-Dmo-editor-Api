using Microsoft.AspNetCore.Identity;

namespace NNA.Domain.Entities; 
public sealed class NnaUser : IdentityUser<Guid> {
    public NnaUser() { }
    public NnaUser(string email, string userName) {
        if (string.IsNullOrWhiteSpace(email)) {
            throw new ArgumentNullException(nameof(email));
        }

        if (string.IsNullOrWhiteSpace(userName)) {
            throw new ArgumentNullException(nameof(userName));
        }

        // ReSharper disable VirtualMemberCallInConstructor
        Email = email;
        UserName = userName;
    }

    public string? AuthProviders { get; set; }
    public ICollection<Dmo> Dmos { get; set; }
    public ICollection<DmoCollection> DmoCollections { get; set; }
    public ICollection<Beat> Beats { get; set; }
}