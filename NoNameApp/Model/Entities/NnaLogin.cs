using System;
using Microsoft.AspNetCore.Identity;

namespace Model.Entities {
    public class NnaLogin : IdentityUserLogin<Guid> {
        public string RefreshTokenId { get; set; }
        public string AccessTokenId { get; set; }
    }
}