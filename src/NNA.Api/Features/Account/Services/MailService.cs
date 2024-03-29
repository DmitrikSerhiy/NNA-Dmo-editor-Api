﻿using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using NNA.Domain.Entities;
using NNA.Domain.Enums;
using NNA.Domain.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace NNA.Api.Features.Account.Services;

public sealed class MailService {
    private readonly SendGridConfiguration _sendGridConfiguration;
    private readonly string _sendGridApiKey;
    private readonly NnaUserManager _nnaUserManager;
    private readonly EmailAddress _nnaFromEmail;

    public MailService(
        IOptions<SendGridConfiguration> sendGridConfiguration,
        NnaUserManager nnaUserManager,
        IConfiguration configuration) {
        _sendGridConfiguration =
            sendGridConfiguration.Value ?? throw new ArgumentNullException(nameof(sendGridConfiguration));
        _nnaUserManager = nnaUserManager ?? throw new ArgumentNullException(nameof(nnaUserManager));
        _nnaFromEmail = new EmailAddress(_sendGridConfiguration.SenderEmail, "Dmo Editor");
        _sendGridApiKey = configuration.GetValue<string>($"{nameof(SendGridConfiguration)}:ApiKey");
    }

    public async Task<bool> SendConfirmAccountEmailAsync(NnaUser user, CancellationToken cancellationToken) {
        var to = new EmailAddress(user.Email, user.UserName);
        var token = await _nnaUserManager.GenerateEmailConfirmationTokenAsync(user);
        var plainTextContent = GenerateMessageForConfirmEmailActionWithLink(token);
        var message = MailHelper.CreateSingleEmail(_nnaFromEmail, to, "Confirm your account", plainTextContent, "");

        Response? response;
        try {
            var sendGridClient = new SendGridClient(new SendGridClientOptions { ApiKey = _sendGridApiKey });
            response = await sendGridClient.SendEmailAsync(message, cancellationToken);
        }
        catch (Exception) {
            return false;
        }

        return response.StatusCode == HttpStatusCode.Accepted;
    }

    public async Task<bool> SendSetOrResetPasswordEmailAsync(NnaUser user, SendMailReason reason, CancellationToken cancellationToken) {
        var subject = reason == SendMailReason.NnaSetPassword ? "Set new password" : "Reset your password";
        var to = new EmailAddress(user.Email, user.UserName);
        var token = await _nnaUserManager.GenerateNnaTokenForSetOrResetPasswordAsync(user, reason);
        var plainTextContent = GenerateMessageForPasswordActionWithLink(user, token, reason);
        var message = MailHelper.CreateSingleEmail(_nnaFromEmail, to, subject, plainTextContent, "");

        Response? response;
        try {
            var sendGridClient = new SendGridClient(new SendGridClientOptions { ApiKey = _sendGridApiKey });
            response = await sendGridClient.SendEmailAsync(message, cancellationToken);
        }
        catch (Exception) {
            return false;
        }

        return response.StatusCode == HttpStatusCode.Accepted;
    }

    private string GenerateMessageForConfirmEmailActionWithLink(string token) {
        var welcomeMessage = new StringBuilder();
        welcomeMessage.AppendLine("Welcome to Dmo Editor!");
        welcomeMessage.AppendLine("To confirm your account please follow the link:");
        welcomeMessage.AppendLine("");

        var link = new StringBuilder();
        link.Append(_sendGridConfiguration.ConfirmAccountUrl);
        link.Append($"?token={token}");
        link.AppendLine("");

        var goodbyeMessage = new StringBuilder();
        goodbyeMessage.AppendLine("");
        goodbyeMessage.AppendLine("Dmo Editor Support Team");

        return $"{welcomeMessage}{link}{goodbyeMessage}";
    }

    private string GenerateMessageForPasswordActionWithLink(NnaUser user, string token, SendMailReason reason) {
        var reasonString = reason == SendMailReason.NnaSetPassword ? "set" : "reset";

        var welcomeMessage = new StringBuilder();
        welcomeMessage.AppendLine($"Hi {user.UserName},");
        welcomeMessage.AppendLine($"You requested an email to {reasonString} your password");
        welcomeMessage.AppendLine("If this was you, then further action is required");
        welcomeMessage.AppendLine("");

        var link = new StringBuilder();
        link.Append($"To {reasonString} your password follow the link:");
        link.AppendLine("");
        link.AppendLine("");
        link.Append(_sendGridConfiguration.PasswordFormUrl);
        link.Append($"?token={token}");
        link.Append($"&reason={(int)reason}");
        link.Append($"&user={user.Email}");

        var goodbyeMessage = new StringBuilder();
        goodbyeMessage.AppendLine("");
        goodbyeMessage.AppendLine("");
        goodbyeMessage.AppendLine("If it was not you ignore this email and reset your account password immediately.");
        goodbyeMessage.AppendLine("Remember to use a password that is both strong and unique.");
        goodbyeMessage.AppendLine("Or use one of your social accounts to login in Dmo Editor (recommended approach).");
        goodbyeMessage.AppendLine("");
        goodbyeMessage.AppendLine("Dmo Editor Support Team");

        return $"{welcomeMessage}{link}{goodbyeMessage}";
    }
}