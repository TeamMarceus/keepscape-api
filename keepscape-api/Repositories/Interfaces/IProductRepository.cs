using keepscape_api.Models;
using keepscape_api.QueryModels;
using keepscape_api.Repositories.Generics;

namespace keepscape_api.Repositories.Interfaces
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<IEnumerable<Product>> Get(ProductQuery productQueryParameters);
        Task<int> GetTotalProductCount();
    }
}
