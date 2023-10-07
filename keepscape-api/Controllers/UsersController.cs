using keepscape_api.Dtos.Users;
using keepscape_api.Enums;
using keepscape_api.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace keepscape_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<IUserService> _logger;
        private readonly IUserService _userService;

        public UsersController(ILogger<IUserService> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }


        [HttpPost("register/buyer")]
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

                return Ok(userResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_userService.Register)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("register/seller")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserCreateSellerDto userRegisterDto)
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
                    return Unauthorized("User is banned.");
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
    }
}
