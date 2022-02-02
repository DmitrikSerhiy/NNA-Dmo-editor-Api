using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using API.Helpers;
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
            var plainTextContent = GenerateMessageWithLink(token, reason);
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

        private string GenerateMessageWithLink(string token, SendMailReason reason) {
            var link = new StringBuilder();
            link.Append(reason == SendMailReason.NnaSetPassword
                ? "Follow the link to set new password: "
                : "Follow the link to reset your password: ");
            link.Append(_sendGridConfiguration.FormUrl);
            link.Append($"?token={HttpUtility.UrlEncode(token)}");
            link.Append($"&reason={(int)reason}");
            return link.ToString();
        }
    }
}