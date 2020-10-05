using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Model.Entities {
    public sealed class NoNameUser : IdentityUser<Guid> {
        public NoNameUser(string email, string userName) {
            if (string.IsNullOrWhiteSpace(email)) {
                throw new ArgumentNullException(nameof(email));
            }

            if (string.IsNullOrWhiteSpace(userName)) {
                throw new ArgumentNullException(nameof(userName));
            }

            Email = email;
            UserName = userName;
        }

        public ICollection<Dmo> Dmos { get; set; }
        public ICollection<UserDmoCollection> UserDmoCollections { get; set; }
    }
}
