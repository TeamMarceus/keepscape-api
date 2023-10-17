using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Generics;

namespace keepscape_api.Repositories.Interfaces
{
    public interface ISellerApplicationRepository : IBaseRepository<SellerApplication>
    {
        Task<SellerApplication?> GetByUserId(Guid userId);
        Task<SellerApplication?> GetBySellerProfileId(Guid sellerProfileId);
        Task<(IEnumerable<SellerApplication> SellerApplications, int PageCount)> Get(SellerApplicationQuery sellerApplicationQuery);
    }
}
