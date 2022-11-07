namespace NNA.Domain.DTOs.Account;

public sealed class UpdateUserNameDto : BaseDto {
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
}