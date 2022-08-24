namespace NNA.Domain.DTOs.Account; 
public class UpdateUserNameDto: BaseDto {
    public string Email { get; set; }
    public string UserName { get; set; }
}