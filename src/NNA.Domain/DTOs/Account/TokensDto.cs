namespace NNA.Domain.DTOs.Account; 
public class TokensDto : BaseDto {
    public string AccessToken { get; set; }
    public string AccessTokenKeyId { get; set; }
    public string RefreshToken { get; set; }
    public string RefreshTokenKeyId { get; set; }
}