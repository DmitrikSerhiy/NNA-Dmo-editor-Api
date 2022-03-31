using System;

namespace Model.DTOs.Account {
    public class ChangePasswordDto : BaseDto {
        public Guid UserId { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}