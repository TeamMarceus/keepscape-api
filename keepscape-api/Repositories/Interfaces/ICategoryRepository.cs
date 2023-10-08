using keepscape_api.Enums;
using keepscape_api.Models.Categories;
using keepscape_api.Repositories.Generics;

namespace keepscape_api.Repositories.Interfaces
{
    public interface ICategoryRepository : IBaseRepository<BaseCategory>
    {
        Task<BaseCategory?> GetCategoriesByTypeName(CategoryType type);
        Task<int> GetCategoryCountAsync();
    }
}
