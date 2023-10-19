using keepscape_api.Dtos.Finances;
using keepscape_api.Services.Finances;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("balance/withdrawals")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetBalanceWithdrawals()
        {
            try
            {
                var balanceWithdrawals = await _financeService.GetBalanceWithdrawals();

                return Ok(balanceWithdrawals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(_financeService.GetBalanceWithdrawals)} threw an exception");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("balance/withdrawals/{balanceWithdrawalId}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> UpdateBalanceWithdrawal(Guid balanceWithdrawalId, [FromBody] BalanceWithdrawalUpdateDto paymentStatus)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var isUpdated = await _financeService.UpdateBalanceWithdrawal(balanceWithdrawalId, paymentStatus.Status);

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
