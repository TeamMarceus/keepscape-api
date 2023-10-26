using keepscape_api.Dtos.Finances;
using keepscape_api.QueryModels;
using keepscape_api.Services.Finances;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace keepscape_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinancesController : ControllerBase
    {
        private ILogger<FinanceService> _logger;
        private readonly IFinanceService _financeService;

        public FinancesController(
                ILogger<FinanceService> logger,
                IFinanceService financeService)
        {
            _logger = logger;
            _financeService = financeService;
        }

        // Sellers
        [HttpGet("seller/balance")]
        [Authorize(Policy = "Seller")]
        public async Task<IActionResult> GetBalance()
        {
            try
            {
                var userId = Guid.TryParse(User.FindFirstValue("UserId"), out var id) ? id : Guid.Empty;

                if (userId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var balance = await _financeService.GetBalance(userId);

                if (balance == null)
                {
                    return BadRequest("Invalud credentials.");
                }

                return Ok(balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_financeService.GetBalance)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("seller/balance/withdrawals")]
        [Authorize(Policy = "Seller")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateBalanceWithdrawal([FromForm] BalanceWithdrawalCreateDto balanceWithdrawalCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = Guid.TryParse(User.FindFirstValue("UserId"), out var id) ? id : Guid.Empty;

                if (userId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var balanceWithdrawal = await _financeService.CreateBalanceWithdrawal(userId, balanceWithdrawalCreateDto);

                if (balanceWithdrawal == null)
                {
                    return BadRequest("Failed to create balance withdrawal");
                }

                return Ok(balanceWithdrawal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_financeService.CreateBalanceWithdrawal)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("seller/balance/logs")]
        [Authorize(Policy = "Seller")]
        public async Task<IActionResult> GetBalanceLogs([FromQuery] PaginatorQuery paginatorQuery)
        {
            try
            {
                var userId = Guid.TryParse(User.FindFirstValue("UserId"), out var id) ? id : Guid.Empty;

                if (userId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var balanceLogs = await _financeService.GetBalanceLogs(userId, paginatorQuery);

                if (balanceLogs == null)
                {
                    return NoContent();
                }

                return Ok(balanceLogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_financeService.GetBalanceLogs)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("seller/balance/withdrawals")]
        [Authorize(Policy = "Seller")]
        public async Task<IActionResult> GetBalanceWithdrawals([FromQuery] PaginatorQuery paginatorQuery)
        {
            try
            {
                var userId = Guid.TryParse(User.FindFirstValue("UserId"), out var id) ? id : Guid.Empty;

                if (userId == Guid.Empty)
                {
                    return BadRequest("Invalid credentials.");
                }

                var balanceWithdrawals = await _financeService.GetBalanceWithdrawals(userId, paginatorQuery);

                if (balanceWithdrawals == null)
                {
                    return NoContent();
                }

                return Ok(balanceWithdrawals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_financeService.GetBalanceWithdrawals)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        // Admin
        [HttpGet("admin/balance/withdrawals")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetBalanceWithdrawals([FromQuery] BalanceWithdrawalQuery balanceWithdrawalQuery)
        {
            try
            {
                var balanceWithdrawals = await _financeService.GetBalanceWithdrawals(balanceWithdrawalQuery);

                if (balanceWithdrawals == null)
                {
                    return NoContent();
                }

                return Ok(balanceWithdrawals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_financeService.GetBalanceWithdrawals)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("admin/balance/withdrawals/{balanceWithdrawalId}")]
        [Authorize(Policy = "Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateBalanceWithdrawal(Guid balanceWithdrawalId, [FromForm] BalanceWithdrawalUpdateDto balanceWithdrawalUpdateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var isUpdated = await _financeService.UpdateBalanceWithdrawal(balanceWithdrawalId, balanceWithdrawalUpdateDto);

                if (!isUpdated)
                {
                    return BadRequest("Failed to update balance withdrawal");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_financeService.UpdateBalanceWithdrawal)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
