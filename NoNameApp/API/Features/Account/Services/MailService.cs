using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Model.Entities;
using SendGrid;
using SendGrid.Helpers.Mail;
using Serilog;

namespace API.Features.Account.Services {
    public class MailService {

        private readonly ISendGridClient _sendGridClient;
        private readonly IConfiguration _configuration;
        
        public MailService(ISendGridClient sendGridClient,
            IConfiguration configuration) {
            _sendGridClient = sendGridClient ?? throw new ArgumentNullException(nameof(sendGridClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<bool> SendSetPasswordEmailAsync(NnaUser user) {
            var from = new EmailAddress(_configuration.GetValue<string>("SendGridConfiguration:SenderEmail"), "Dmo Editor");
            var subject = "Set new password";
            var to = new EmailAddress(user.Email, user.UserName);
            var plainTextContent = "Follow the link to set new password";
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

        public async Task<bool> SendResetPasswordEmailAsync(NnaUser user) {
            return true;
        }
    }
}