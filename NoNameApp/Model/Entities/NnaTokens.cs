using System;
using Microsoft.AspNetCore.Identity;

namespace Model.Entities {
    public sealed class NnaToken : IdentityUserToken<Guid> {
        public string TokenKeyId { get; set; }
    }
}