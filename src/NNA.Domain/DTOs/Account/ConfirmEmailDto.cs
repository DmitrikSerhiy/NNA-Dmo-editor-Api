namespace NNA.Domain.DTOs.Account;

public sealed class ConfirmEmailDto : BaseDto {
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
}