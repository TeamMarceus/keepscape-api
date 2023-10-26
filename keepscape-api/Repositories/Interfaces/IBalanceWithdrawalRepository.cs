using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Generics;

namespace keepscape_api.Repositories.Interfaces
{
    public interface IBalanceWithdrawalRepository : IBaseRepository<BalanceWithdrawal>
    {
        Task<(IEnumerable<BalanceWithdrawal> BalanceWithdrawals, int PageCount)> GetByBalanceId(Guid balanceId, PaginatorQuery paginatorQuery);
        Task<(IEnumerable<BalanceWithdrawal> BalanceWithdrawals, int PageCount)> Get(BalanceWithdrawalQuery balanceWithdrawalQuery);
    }
}
