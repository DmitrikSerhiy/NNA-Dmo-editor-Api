namespace NNA.Domain.Entities; 
public class UsersTokens {
    public virtual Guid UserId { get; set; }
    public virtual string Email { get; set; } = null!;
    public virtual string AccessTokenId { get; set; } = null!;
    public virtual string RefreshTokenId { get; set; } = null!;
    public virtual string LoginProvider { get; set; } = null!;
}