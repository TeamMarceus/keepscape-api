using keepscape_api.Models;
using keepscape_api.Repositories.Interfaces;
using keepscape_api.Services.Emails;

namespace keepscape_api.Services.ConfirmationCodes
{
    public class ConfirmationCodeService : IConfirmationCodeService
    {
        private readonly IConfirmationCodeRepository _confirmationCodeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly Random _random = new Random();

        public ConfirmationCodeService(
            IConfirmationCodeRepository confirmationCodeRepository,
            IUserRepository userRepository,
            IEmailService emailService)
        {
            _confirmationCodeRepository = confirmationCodeRepository;
            _userRepository = userRepository;
            _emailService = emailService;
        }

        public async Task<bool> Send(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                return false;
            }

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

            await _emailService.SendEmailAsync(email, subject, body);

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
