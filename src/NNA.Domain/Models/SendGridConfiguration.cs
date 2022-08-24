namespace NNA.Domain.Models;
public class SendGridConfiguration {
    public string SenderEmail { get; set; }
    public string PasswordFormUrl { get; set; }
    public string ConfirmAccountUrl { get; set; }
    public string ApiKey { get; set; }
}
