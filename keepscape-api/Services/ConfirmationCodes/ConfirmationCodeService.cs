using keepscape_api.Configurations;
using keepscape_api.Models;
using keepscape_api.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace keepscape_api.Services.ConfirmationCodes
{
    public class ConfirmationCodeService : IConfirmationCodeService
    {
        private readonly IConfirmationCodeRepository _confirmationCodeRepository;
        private readonly IUserRepository _userRepository;
        private readonly CodeConfig _config;
        private readonly Random _random = new Random();

        public ConfirmationCodeService(
            IConfirmationCodeRepository confirmationCodeRepository,
            IUserRepository userRepository,
            IOptionsSnapshot<CodeConfig> config)
        {
            _confirmationCodeRepository = confirmationCodeRepository;
            _userRepository = userRepository;
            _config = config.Value;
        }

        public async Task<bool> Send(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                return false;
            }

            var fromAddress = new MailAddress(_config.Email, "Keepscape Ecommerce");
            var toAddress = new MailAddress(email);
            string password = _config.Password;
            string subject = "Password Reset Code - Keepscape Ecommerce";
            string code = GenerateCode();

            string body = $@"
                <html>
                    <body style='font-family: Arial, sans-serif; color: #333;'>
                        <h1 style='color: #E7173B;'>Password Reset Code</h1>
                        <p>Hello,</p>
                        <p>Here is your password reset code: </p>
                        <h1 style='font-size: 40px;'><strong>{code}</strong></h1>
                        <p>This will only last 10 minutes</p> 
                        <p>If you didn't request this, please ignore this email.</p>
                        <p>Regards,<br />Keepscape Team</p>
                    </body>
                </html>
            ";

            var smtp = new SmtpClient
            {
                Host = _config.Host,
                Port = _config.Port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, password)
            };

            var mailMessage = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            await smtp.SendMailAsync(mailMessage);

            var latestCode = await _confirmationCodeRepository.GetLatestConfirmationCodeByUserGuid(user.Id);

            if (latestCode != null)
            {
                await _confirmationCodeRepository.DeleteAsync(latestCode);
            }

            await _confirmationCodeRepository.AddAsync(new ConfirmationCode
            {
                Code = code,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            });

            return true;
        }

        private string GenerateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public async Task<bool> Verify(string email, string code)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                return false;
            }

            var latestCode = await _confirmationCodeRepository.GetLatestConfirmationCodeByUserGuid(user.Id);

            if (latestCode == null)
            {
                return false;
            }

            if (latestCode.IsRevoked ||
                latestCode.IsConfirmed ||
                latestCode.ExpiresAt < DateTime.UtcNow ||
                latestCode.Code != code)
            {
                return false;
            }

            return true;
        }
    }
}
