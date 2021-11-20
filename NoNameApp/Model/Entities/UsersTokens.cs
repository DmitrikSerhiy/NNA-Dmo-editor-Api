using System;

namespace Model.Entities {
    public sealed class UsersTokens {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string AccessTokenId { get; set; }
        public string RefreshTokenId { get; set; }
    }
}