using keepscape_api.Models;
using keepscape_api.Repositories.Generics;

namespace keepscape_api.Repositories.Interfaces
{
    public interface ISellerProfileRepository : IBaseRepository<SellerProfile>, IProfileRepository<SellerProfile>
    {
        public Task<int> TotalSold(Guid sellerProfileId);
    }
}
