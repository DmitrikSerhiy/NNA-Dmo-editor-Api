namespace NNA.Domain.Models;

public sealed class SendGridConfiguration {
    public string SenderEmail { get; set; } = null!;
    public string PasswordFormUrl { get; set; } = null!;
    public string ConfirmAccountUrl { get; set; } = null!;
}