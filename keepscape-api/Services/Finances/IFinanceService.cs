﻿using keepscape_api.Dtos.Finances;

namespace keepscape_api.Services.Finances
{
    public interface IFinanceService
    {
        Task<BalanceResponseDto?> GetBalance(Guid userId);
        Task<BalanceWithdrawalResponseDto?> CreateBalanceWithdrawal(Guid userId, BalanceWithdrawalCreateDto balanceWithdrawalCreateDto);
        Task<IEnumerable<BalanceWithdrawalResponseDto>> GetBalanceWithdrawals();
        Task<bool> UpdateBalanceWithdrawal(Guid balanceWithdrawalId, BalanceWithdrawalUpdateDto balanceWithdrawalUpdateDto);
    }
}
