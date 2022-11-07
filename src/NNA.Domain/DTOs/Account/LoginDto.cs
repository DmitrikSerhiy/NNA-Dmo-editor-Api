namespace NNA.Domain.DTOs.Account;

public sealed class LoginDto : BaseDto {
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}