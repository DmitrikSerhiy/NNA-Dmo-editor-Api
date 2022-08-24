using NNA.Domain.Enums;

namespace NNA.Domain.DTOs.Account; 
public class SendMailDto: BaseDto {
    public string Email { get; set; }
    public SendMailReason Reason { get; set; }
}