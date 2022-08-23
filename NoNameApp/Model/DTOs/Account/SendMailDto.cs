using Model.Enums;

namespace Model.DTOs.Account; 
public class SendMailDto: BaseDto {
    public string Email { get; set; }
    public SendMailReason Reason { get; set; }
}