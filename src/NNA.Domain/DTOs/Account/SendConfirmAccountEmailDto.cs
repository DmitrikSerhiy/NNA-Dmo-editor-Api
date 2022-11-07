namespace NNA.Domain.DTOs.Account;

public sealed class SendConfirmAccountEmailDto : BaseDto {
    public string Email { get; set; } = null!;
}