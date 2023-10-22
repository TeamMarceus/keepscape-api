using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Generics;

namespace keepscape_api.Repositories.Interfaces
{
    public interface IBalanceWithdrawalRepository : IBaseRepository<BalanceWithdrawal>
    {
        Task<BalanceWithdrawal?> GetByBalanceId(Guid balanceId);
        Task<(IEnumerable<BalanceWithdrawal> BalanceWithdrawals, int PageCount)> Get(BalanceWithdrawalQuery balanceWithdrawalQuery);
    }
}
