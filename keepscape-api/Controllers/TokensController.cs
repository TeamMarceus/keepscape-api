using keepscape_api.Dtos.Tokens;
using keepscape_api.Enums;
using keepscape_api.Services.Tokens;
using keepscape_api.Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace keepscape_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokensController : ControllerBase
    {
        private readonly ILogger<TokenService> _logger;
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public TokensController(ILogger<TokenService> logger, ITokenService tokenService, IUserService userService)
        {
            _logger = logger;
            _tokenService = tokenService;
            _userService = userService;
        }

        [HttpPost("acquire")]
        public async Task<IActionResult> Acquire([FromBody] TokenCreateDto tokenCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userStatus = await _userService.GetStatus(tokenCreateDto.Email);

                if (userStatus == UserStatus.Banned)
                {
                    return Forbid("User is banned.");
                }

                if (userStatus == UserStatus.NotFound)
                {
                    return BadRequest("User not found.");
                }

                if (userStatus == UserStatus.Pending)
                {
                    return Unauthorized("User is pending.");
                }

                var tokenResponseDto = await _tokenService.Create(tokenCreateDto);

                if (tokenResponseDto == null)
                {
                    return BadRequest("Invalid credentials.");
                }

                return Ok(tokenResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_tokenService.Create)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] TokenVerifyDto tokenVerifyDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var isVerified = await _tokenService.Verify(tokenVerifyDto);

                if (!isVerified)
                {
                    return BadRequest(isVerified);
                }

                return Ok(isVerified);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_tokenService.Verify)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromQuery] string refreshToken)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var isRevoked = await _tokenService.Revoke(refreshToken);

                if (!isRevoked)
                {
                    return BadRequest(isRevoked);
                }

                return Ok(isRevoked);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_tokenService.Revoke)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenRefreshDto tokenRefreshDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userStatus = await _userService.GetStatus(tokenRefreshDto.UserId);

                if (userStatus == UserStatus.Banned)
                {
                    return Forbid("User is banned.");
                }

                if (userStatus == UserStatus.NotFound)
                {
                    return BadRequest("User not found.");
                }

                if (userStatus == UserStatus.Pending)
                {
                    return Unauthorized("User is pending.");
                }

                var tokenResponseDto = await _tokenService.Refresh(tokenRefreshDto);

                if (tokenResponseDto == null)
                {
                    return BadRequest("Cannot refresh token.");
                }

                return Ok(tokenResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_tokenService.Refresh)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
