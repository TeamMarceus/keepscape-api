using keepscape_api.Configurations;
using keepscape_api.Dtos.Tokens;
using keepscape_api.Models;
using keepscape_api.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace keepscape_api.Services.Tokens
{
    public class TokenService : ITokenService
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly JwtConfig _jwtConfig;
       
        public TokenService(ITokenRepository tokenRepository, IUserRepository userRepository, IOptionsSnapshot<JwtConfig> jwtConfig)
        {
            _tokenRepository = tokenRepository;
            _userRepository = userRepository;
            _jwtConfig = jwtConfig.Value;
        }

        public async Task<TokenResponseDto?> Create(TokenCreateDto tokenCreateDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(tokenCreateDto.Email);

            if (user == null)
            {
                return null;
            }

            var passwordHasher = new PasswordHasher<User>();
            var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, tokenCreateDto.Password);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var latestToken = await _tokenRepository.GetLatestTokenByUserGuid(user.Id);

            if (latestToken != null)
            {
                latestToken.IsRevoked = true;
                await _tokenRepository.UpdateAsync(latestToken);
            }

            return GenerateToken(user);
        }

        public async Task<bool> Revoke(string refreshToken)
        {
            var token = await _tokenRepository.GetTokenByRefreshToken(refreshToken);

            if (token == null)
            {
                return false;
            }

            token.IsRevoked = true;

            return await _tokenRepository.UpdateAsync(token);
        }

        public Task<bool> Verify(TokenVerifyDto tokenVerifyDto)
        {
            throw new NotImplementedException();
        }
        private TokenResponseDto GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("Email", user.Email),
                    new Claim("FirstName", user.FirstName),
                    new Claim("LastName", user.LastName),
                    new Claim("Role", user.UserType.ToString()),
                    new Claim(JwtRegisteredClaimNames.Aud, _jwtConfig.Audience!),
                    new Claim(JwtRegisteredClaimNames.Iss, _jwtConfig.Issuer!)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtConfig.AccessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var acessToken = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
            var refreshToken = GenerateRefreshToken();

            return new TokenResponseDto
            {
                AccessToken = acessToken,
                RefreshToken = refreshToken
            };
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
