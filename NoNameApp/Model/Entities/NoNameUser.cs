using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Model.Entities {
    public sealed class NoNameUser : IdentityUser<Guid> {
        public NoNameUser(String email, String userName) {
            if (String.IsNullOrWhiteSpace(email)) {
                throw new ArgumentNullException(nameof(email));
            }

            if (String.IsNullOrWhiteSpace(userName)) {
                throw new ArgumentNullException(nameof(userName));
            }

            Email = email;
            UserName = userName;
        }

        public ICollection<Dmo> DecomposedMovieObjects { get; set; }
    }
}
