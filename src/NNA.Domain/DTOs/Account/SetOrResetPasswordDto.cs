using NNA.Domain.Enums;

namespace NNA.Domain.DTOs.Account; 
public class SetOrResetPasswordDto: BaseDto {
    public string Token { get; set; }
    public string Email { get; set; }
    public string NewPassword { get; set; }
    public SendMailReason Reason { get; set; }
}