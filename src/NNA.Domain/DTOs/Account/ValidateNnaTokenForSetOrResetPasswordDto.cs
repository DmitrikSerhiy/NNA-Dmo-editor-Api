using NNA.Domain.Enums;

namespace NNA.Domain.DTOs.Account; 
public class ValidateNnaTokenForSetOrResetPasswordDto: BaseDto {
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
    public SendMailReason Reason { get; set; }
}