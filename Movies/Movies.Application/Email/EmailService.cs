﻿using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Movies.Application.Settings;

namespace Movies.Application.Email;

internal class EmailService(IOptions<EmailSettings> options) : IEmailService
{
    private readonly EmailSettings _emailSettings = options.Value;

    public async Task SendAsync(string recipient, string subject, string htmlBody)
    {
        var builder = new BodyBuilder
        {
            HtmlBody = htmlBody
        };
        
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(MailboxAddress.Parse(_emailSettings.From));
        emailMessage.To.Add(MailboxAddress.Parse(recipient));
        emailMessage.Subject = subject;
        emailMessage.Body = builder.ToMessageBody();

        using var emailClient = new SmtpClient();
        await emailClient.ConnectAsync(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
        await emailClient.AuthenticateAsync(_emailSettings.Username,_emailSettings.Password);
        await emailClient.SendAsync(emailMessage);
        await emailClient.DisconnectAsync(true);

    }
    
}