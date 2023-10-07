using keepscape_api.Models.Categories;

namespace keepscape_api.Repositories.Generics
{
    public interface ICategoryRepository<T> where T : BaseCategory
    {
        Task<T?> GetCategoryByNameAsync(string name);
        Task<int> GetCategoryCountAsync();
    }
}
