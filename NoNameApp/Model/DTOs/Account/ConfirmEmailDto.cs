namespace Model.DTOs.Account; 
public class ConfirmEmailDto: BaseDto {
    public string Email { get; set; }
    public string Token { get; set; }
}