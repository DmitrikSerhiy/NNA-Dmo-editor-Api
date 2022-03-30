namespace Model.DTOs.Account {
    public class PersonalInfoDto: BaseDto {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserId { get; set; }
        public string[] AuthProviders { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool HasPassword { get; set; }
    }
}