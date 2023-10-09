using Azure.Security.KeyVault.Secrets;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using PlanePal.Services.Interfaces;
using PlanePal.Services.Shared;

namespace PlanePal.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly KeyVaultSecret _emailAddress;
        private readonly KeyVaultSecret _password;
        private readonly SecretClient _secretClient;

        public EmailService()
        {
            _secretClient = AzureKeyVaultClientProvider.GetClient();
            _emailAddress = _secretClient.GetSecret("email-service");
            _password = _secretClient.GetSecret("email-service-password");
        }

        public async Task SendMail(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Recipient email is empty or whitespace.");
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Email subject is empty or whitespace.");
            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Email body is empty or whitespace.");

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_emailAddress.Value));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            message.Body = new TextPart(TextFormat.Plain)
            {
                Text = body
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailAddress.Value, _password.Value);
            await smtp.SendAsync(message);
        }
    }
}