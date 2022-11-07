namespace NNA.Domain.DTOs.Account;

public sealed class RefreshDto : BaseDto {
    public string RefreshToken { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
}