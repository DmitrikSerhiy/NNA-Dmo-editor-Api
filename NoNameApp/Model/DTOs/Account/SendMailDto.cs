using Model.Enums;

namespace Model.DTOs.Account {
    public class SendMailDto {
        public string Email { get; set; }
        public SendMailReason Reason { get; set; }
    }
}