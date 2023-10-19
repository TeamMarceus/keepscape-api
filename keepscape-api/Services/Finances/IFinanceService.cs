using keepscape_api.Dtos.Finances;
using keepscape_api.Models;

namespace keepscape_api.Services.Finances
{
    public interface IFinanceService
    {
        Task<IEnumerable<BalanceWithdrawalResponseDto>> GetBalanceWithdrawals();
        Task<bool> UpdateBalanceWithdrawal(Guid balanceWithdrawalId, string paymentStatus);
    }
}
