namespace NNA.Domain.DTOs.Account;

public sealed class TokensDto : BaseDto {
    public string AccessToken { get; set; } = null!;
    public string AccessTokenKeyId { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public string RefreshTokenKeyId { get; set; } = null!;
}