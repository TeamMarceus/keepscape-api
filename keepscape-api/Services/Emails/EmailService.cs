using keepscape_api.Configurations;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace keepscape_api.Services.Emails
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfig _config;
        private readonly MailAddress _from;
        private readonly SmtpClient _smtp;

        public EmailService(IOptionsSnapshot<EmailConfig> config)
        {
            _config = config.Value;
            _from = new MailAddress(_config.Email, "Keepscape Ecommerce");
            _smtp = new SmtpClient
            {
                Host = _config.Host,
                Port = _config.Port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_config.Email, _config.Password)
            };
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var to = new MailAddress(email);

            var mailMessage = new MailMessage(_from, to)
            {
                Subject = subject,
                Body = message,
                IsBodyHtml = true
            };

            await _smtp.SendMailAsync(mailMessage);

        }
    }
}
