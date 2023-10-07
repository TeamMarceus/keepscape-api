using keepscape_api.Models;
using keepscape_api.Repositories.Generics;

namespace keepscape_api.Repositories.Interfaces
{
    public interface ISellerApplicationRepository : IBaseRepository<SellerApplication>
    {
        Task<SellerApplication?> GetSellerApplicationByUserId(Guid userId);
        Task<SellerApplication?> GetSellerApplicationBySellerProfileId(Guid sellerProfileId);
    }
}
