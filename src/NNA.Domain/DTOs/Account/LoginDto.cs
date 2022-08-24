namespace NNA.Domain.DTOs.Account; 
public class LoginDto : BaseDto {
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}