namespace Model.DTOs.Account; 
public sealed class RegisterDto : BaseDto {
    public string Email { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}