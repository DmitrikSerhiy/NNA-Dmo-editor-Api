namespace NNA.Domain.DTOs.Account; 
public class SendConfirmAccountEmailDto: BaseDto {
    public string Email { get; set; } = null!;
}