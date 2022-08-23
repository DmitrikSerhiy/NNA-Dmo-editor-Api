using System;

namespace Model.Entities; 
public class UsersTokens {
    public virtual Guid UserId { get; set; }
    public virtual string Email { get; set; }
    public virtual string AccessTokenId { get; set; }
    public virtual string RefreshTokenId { get; set; }
    public virtual string LoginProvider { get; set; }
}