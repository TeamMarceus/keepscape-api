using keepscape_api.Enums;
using keepscape_api.Models.Categories;
using keepscape_api.Repositories.Generics;

namespace keepscape_api.Repositories.Interfaces
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<int> GetCategoryCountAsync();
    }
}
