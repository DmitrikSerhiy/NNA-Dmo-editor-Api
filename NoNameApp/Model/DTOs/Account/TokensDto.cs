namespace Model.DTOs.Account {
    public class TokensDto : BaseDto {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}