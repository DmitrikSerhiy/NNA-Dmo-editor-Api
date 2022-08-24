namespace NNA.Domain.Models;
public class SendGridConfiguration
{
    public string SenderEmail { get; set; } = null!;
    public string PasswordFormUrl { get; set; } = null!;
    public string ConfirmAccountUrl { get; set; } = null!;
}
