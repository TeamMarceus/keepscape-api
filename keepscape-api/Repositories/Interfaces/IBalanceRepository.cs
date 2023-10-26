using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Generics;
namespace keepscape_api.Repositories.Interfaces
{
    public interface IBalanceRepository : IBaseRepository<Balance>
    {
        Task<Balance?> GetByUserId(Guid userId);
        Task<(IEnumerable<BalanceLog> BalanceLogs, int PageCount)> GetLogsByBalanceId(Guid balanceId, PaginatorQuery paginatorQuery);
    }
}
