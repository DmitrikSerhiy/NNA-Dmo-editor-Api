namespace Model.DTOs.Account; 
public class RefreshDto : BaseDto {
    public string RefreshToken { get; set; }
    public string AccessToken { get; set; }
}