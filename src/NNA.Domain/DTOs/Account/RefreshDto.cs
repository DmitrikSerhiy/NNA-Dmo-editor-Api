namespace NNA.Domain.DTOs.Account;

public class RefreshDto : BaseDto {
    public string RefreshToken { get; set; } = null!;
    public string AccessToken { get; set; } = null!;
}