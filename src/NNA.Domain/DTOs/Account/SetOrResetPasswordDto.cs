using NNA.Domain.Enums;

namespace NNA.Domain.DTOs.Account;

public sealed class SetOrResetPasswordDto : BaseDto {
    public string Token { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
    public SendMailReason Reason { get; set; }
}