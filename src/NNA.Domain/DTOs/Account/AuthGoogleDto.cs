namespace NNA.Domain.DTOs.Account; 
public class AuthGoogleDto: BaseDto {
    public string Email { get; set; }
    public string Name { get; set; }
    public string GoogleToken { get; set; }
}