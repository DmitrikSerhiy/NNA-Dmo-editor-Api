using NNA.Domain.Enums;

namespace NNA.Domain.DTOs.Account; 
public class ValidateNnaTokenForSetOrResetPasswordDto: BaseDto {
    public string Email { get; set; }
    public string Token { get; set; }        
    public SendMailReason Reason { get; set; }
}