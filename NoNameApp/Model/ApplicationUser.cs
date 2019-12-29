using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Model {
    public sealed class ApplicationUser : IdentityUser<Guid> {
        public ApplicationUser(String email) {
            Email = email;
        }
    }
}
