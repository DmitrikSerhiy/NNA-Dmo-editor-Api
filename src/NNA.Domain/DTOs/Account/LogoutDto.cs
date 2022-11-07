namespace NNA.Domain.DTOs.Account;

public sealed class LogoutDto : BaseDto {
    public string Email { get; set; } = null!;
}