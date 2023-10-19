using keepscape_api.Models;

using keepscape_api.Repositories.Generics;
namespace keepscape_api.Repositories.Interfaces
{
    public interface IBalanceRepository : IBaseRepository<Balance>
    {
        Task<Balance?> GetBalanceByUserId(Guid userId);
    }
}
