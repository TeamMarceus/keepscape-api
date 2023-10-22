﻿using keepscape_api.Dtos.Finances;
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

        [HttpGet("balance")]
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

        [HttpPost("balance/withdrawals")]
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
