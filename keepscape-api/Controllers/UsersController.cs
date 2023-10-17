using keepscape_api.Dtos.Users;
using keepscape_api.Enums;
using keepscape_api.QueryModels;
using keepscape_api.Services.ConfirmationCodes;
using keepscape_api.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace keepscape_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<IUserService> _logger;
        private readonly IUserService _userService;
        private readonly IConfirmationCodeService _confirmationCodeService;

        public UsersController(ILogger<IUserService> logger, IUserService userService, IConfirmationCodeService confirmationCodeService)
        {
            _logger = logger;
            _userService = userService;
            _confirmationCodeService = confirmationCodeService;
        }

        // Generic Users
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userStatus = await _userService.GetStatus(userLoginDto.Email);

                if (userStatus == UserStatus.Banned)
                {
                    return BadRequest("User is banned.");
                }

                if (userStatus == UserStatus.NotFound)
                {
                    return BadRequest("User not found.");
                }

                if (userStatus == UserStatus.Pending)
                {
                    return Unauthorized("User is pending.");
                }

                var userResponseDto = await _userService.Login(userLoginDto);

                if (userResponseDto == null)
                {
                    return BadRequest("Invalid email or password.");
                }

                return Ok(userResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_userService.Login)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = Guid.TryParse(User.FindFirstValue("UserId"), out var parsedUserId) ? parsedUserId : Guid.Empty;

                if (userId == Guid.Empty)
                {
                    return BadRequest("Invalid user id.");
                }

                await _userService.Logout(userId);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_userService.Logout)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("passwords/codes")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPasswordResetCode(string email)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userStatus = await _userService.GetStatus(email);

                if (userStatus == UserStatus.Banned)
                {
                    return BadRequest("User is banned.");
                }

                if (userStatus == UserStatus.NotFound)
                {
                    return BadRequest("User not found.");
                }

                if (userStatus == UserStatus.Pending)
                {
                    return Unauthorized("User is pending.");
                }

                var codeSent = await _confirmationCodeService.Send(email);

                if (!codeSent)
                {
                    return BadRequest("Invalid email.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_confirmationCodeService.Send)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("passwords/codes/verify")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyPasswordResetCode([FromBody] UserVerifyCode userVerifyCode)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userStatus = await _userService.GetStatus(userVerifyCode.Email);

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

                var codeVerified = await _confirmationCodeService.Verify(userVerifyCode.Email, userVerifyCode.Code);

                if (!codeVerified)
                {
                    return BadRequest("Invalid email or code.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_confirmationCodeService.Verify)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("passwords/reset")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(UserUpdatePasswordWithCodeDto userUpdatePasswordWithCodeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userStatus = await _userService.GetStatus(userUpdatePasswordWithCodeDto.Email);

                if (userStatus == UserStatus.Banned)
                {
                    return BadRequest("User is banned.");
                }

                if (userStatus == UserStatus.NotFound)
                {
                    return BadRequest("User not found.");
                }

                if (userStatus == UserStatus.Pending)
                {
                    return Unauthorized("User is pending.");
                }

                var userResponseDto = await _userService.UpdatePasswordWithCode(userUpdatePasswordWithCodeDto);

                if (!userResponseDto)
                {
                    return BadRequest("Invalid email.");
                }

                return Ok(userResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_userService.UpdatePasswordWithCode)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("passwords/update")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword(UserUpdatePasswordDto userUpdatePasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = Guid.TryParse(User.FindFirstValue("UserId"), out var parsedUserId) ? parsedUserId : Guid.Empty;

                if (userId == Guid.Empty)
                {
                    return BadRequest("Invalid user id.");
                }

                var userResponseDto = await _userService.UpdatePassword(userId, userUpdatePasswordDto);

                if (!userResponseDto)
                {
                    return BadRequest("Invalid email.");
                }

                return Ok(userResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_userService.UpdatePassword)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        // Buyers
        [HttpPost("buyers/register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserCreateBuyerDto userRegisterDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userResponseDto = await _userService.Register(userRegisterDto);

                if (userResponseDto == null)
                {
                    return BadRequest("Email already exists or user profile cannot be created.");
                }

                userResponseDto = (UserResponseBuyerDto)userResponseDto;

                return Created("", userResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_userService.Register)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        // Sellers
        [HttpPost("sellers/register")]
        [Consumes("multipart/form-data")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromForm]UserCreateSellerDto userRegisterDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userResponseDto = await _userService.Register(userRegisterDto);

                if (userResponseDto == null)
                {
                    return BadRequest("Email already exists or user profile cannot be created.");
                }

                userResponseDto = (UserResponseSellerDto)userResponseDto;

                return Ok(userResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_userService.Register)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        // Admin
        [HttpGet("sellers/applications")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetSellerApplications([FromQuery] SellerApplicationQuery sellerApplicationQuery)
        {
            try
            {
                var sellerApplications = await _userService.GetApplications(sellerApplicationQuery);

                return Ok(sellerApplications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_userService.GetApplications)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("sellers/applications/{applicationId}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> UpdateSellerApplication(Guid applicationId, [FromBody] UserSellerApplicationStatusUpdateDto statusUpdate)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var applicationUpdated = await _userService.UpdateApplication(applicationId, statusUpdate);

                if (!applicationUpdated)
                {
                    return BadRequest("Invalid application id.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_userService.UpdateApplication)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{userId}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Update(Guid userId, [FromBody] UserStatusUpdateDto userStatusUpdateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userResponseDto = await _userService.Update(userId, userStatusUpdateDto);

                if (!userResponseDto)
                {
                    return BadRequest("Invalid user id.");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_userService.Update)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
