using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using GardenGroup.Models;
using GardenGroup.Services.interfaces;

namespace GardenGroup.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpOptions _options;

        public EmailService(SmtpOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Recipient email address is required.", nameof(toEmail));

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("GardenGroup", _options.FromAddress));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = Regex.Replace(htmlBody, "<.*?>", string.Empty)
            };
            message.Body = bodyBuilder.ToMessageBody();

            try
            {
                using var client = new SmtpClient();

                // Connect
                await client.ConnectAsync(_options.Host, _options.Port,
                    _options.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                // Authenticate (if username/password provided)
                if (!string.IsNullOrEmpty(_options.Username))
                {
                    await client.AuthenticateAsync(_options.Username, _options.Password);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                Console.WriteLine($" Email successfully sent to {toEmail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email to {toEmail}: {ex.Message}");
                throw; // Let higher layer handle/log if necessary
            }
        }
    }
}
