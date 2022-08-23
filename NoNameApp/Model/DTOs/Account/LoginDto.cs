namespace Model.DTOs.Account; 
public class LoginDto : BaseDto {
    public string Email { get; set; }
    public string Password { get; set; }
}