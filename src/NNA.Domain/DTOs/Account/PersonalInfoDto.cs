namespace NNA.Domain.DTOs.Account;

public sealed class PersonalInfoDto : BaseDto {
    public PersonalInfoDto(string userName, string userEmail, string userId, string[] authProviders) {
        UserName = userName;
        UserEmail = userEmail;
        UserId = userId;
        AuthProviders = authProviders;
    }

    public string UserName { get; set; }
    public string UserEmail { get; set; }
    public string UserId { get; set; }
    public string[] AuthProviders { get; set; }
    public bool IsEmailVerified { get; set; }
    public bool IsEmailSent { get; set; }
    public DateTimeOffset? LastTimeEmailSent { get; set; }
    public bool HasPassword { get; set; }
}