namespace NNA.Domain.DTOs.Account;

public sealed class SsoCheckDto : BaseDto {
    public string Email { get; set; } = null!;
}