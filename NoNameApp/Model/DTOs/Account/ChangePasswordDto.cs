namespace Model.DTOs.Account {
    public class ChangePasswordDto : BaseDto {
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}