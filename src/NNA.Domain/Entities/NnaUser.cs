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

        Email = email;
        UserName = userName;
    }

    public string? AuthProviders { get; set; }
    // ReSharper disable CollectionNeverUpdated.Global
    public ICollection<Dmo> Dmos { get; set; } = new List<Dmo>();
    public ICollection<DmoCollection> DmoCollections { get; set; } = new List<DmoCollection>();
    public ICollection<Beat> Beats { get; set; } = new List<Beat>();
}