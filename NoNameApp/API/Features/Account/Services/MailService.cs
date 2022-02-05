using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using API.Helpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using Model.Entities;
using Model.Enums;
using SendGrid;
using SendGrid.Helpers.Mail;
using Serilog;

namespace API.Features.Account.Services {
    public class MailService {

        private readonly ISendGridClient _sendGridClient;
        private readonly SendGridConfiguration _sendGridConfiguration;
        private readonly NnaUserManager _nnaUserManager;
        
        public MailService(
            ISendGridClient sendGridClient,
            IOptions<SendGridConfiguration> sendGridConfiguration, 
            NnaUserManager nnaUserManager) {
            _sendGridClient = sendGridClient ?? throw new ArgumentNullException(nameof(sendGridClient));
            _sendGridConfiguration = sendGridConfiguration?.Value ?? throw new ArgumentNullException(nameof(sendGridConfiguration));
            _nnaUserManager = nnaUserManager ?? throw new ArgumentNullException(nameof(nnaUserManager));
        }

        public async Task<bool> SendSetOrResetPasswordEmailAsync(NnaUser user, SendMailReason reason) {
            var from = new EmailAddress(_sendGridConfiguration.SenderEmail, "Dmo Editor");
            var subject = reason == SendMailReason.NnaSetPassword ? "Set new password" : "Reset your password";
            var to = new EmailAddress(user.Email, user.UserName);
            var token = await _nnaUserManager.GenerateNnaTokenForSetOrResetPasswordAsync(user, reason);
            var plainTextContent = GenerateMessageWithLink(user, token, reason);
            var message = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, "");
            
            Response response = null;
            try {
                response = await _sendGridClient.SendEmailAsync(message);
            }
            catch (Exception ex) {
                var responseString = await response?.Body?.ReadAsStringAsync() ?? "Response is null";
                Log.Error(ex, responseString);
                return false;
            }
            return response.StatusCode == HttpStatusCode.Accepted;
        }

        private string GenerateMessageWithLink(NnaUser user, string token, SendMailReason reason) {
            var reasonString = reason == SendMailReason.NnaSetPassword ? "set" : "reset";
            
            var welcomeMessage = new StringBuilder();
            welcomeMessage.AppendLine($"Hi {user.UserName},");
            welcomeMessage.AppendLine($"You requested an email to {reasonString} your password");
            welcomeMessage.AppendLine("If this was you, then further action is required");
            welcomeMessage.AppendLine("");
            
            var link = new StringBuilder();
            link.Append($"To {reasonString} your password follow the link :");
            link.AppendLine("");
            link.Append(_sendGridConfiguration.FormUrl);
            link.Append($"?token={HttpUtility.UrlEncode(token)}");
            link.Append($"&reason={(int)reason}");
            link.Append($"&user={user.Email}");

            var goodbyeMessage = new StringBuilder();
            goodbyeMessage.AppendLine("");
            goodbyeMessage.AppendLine("If it was not you ignore this email!");
            goodbyeMessage.AppendLine("And please, reset your account password immediately.");
            goodbyeMessage.AppendLine("Remember to use a password that is both strong and unique.");
            goodbyeMessage.AppendLine("");
            goodbyeMessage.AppendLine("Or use one of your social accounts to login in Dmo Editor.");
            goodbyeMessage.AppendLine("This is the best approach from security view point");
            goodbyeMessage.AppendLine("");

            goodbyeMessage.AppendLine("Thanks,");
            goodbyeMessage.AppendLine("Dmo Editor Support Team");
            
            return $"{welcomeMessage}{link}{goodbyeMessage}";
        }
    }
}