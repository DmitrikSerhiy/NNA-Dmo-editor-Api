namespace NNA.Domain.DTOs.Account;

public sealed class RegisterDto : BaseDto {
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
}