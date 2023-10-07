using keepscape_api.Data;
using keepscape_api.Models.Categories;
using keepscape_api.Repositories.Generics;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class CategoryRepository<T> : BaseRepository<T>, ICategoryRepository<T> where T : BaseCategory
    {
        public CategoryRepository(APIDbContext context) : base(context)
        {
        }
        public override async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet
                .Include(t => t.BaseImage)
                .ToListAsync();
        }
        public override async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(t => t.BaseImage)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
        public async Task<T?> GetCategoryByNameAsync(string name)
        {
            return await _dbSet
                .Include(t => t.BaseImage)
                .FirstOrDefaultAsync(t => t.Name == name);
        }

        public async Task<int> GetCategoryCountAsync()
        {
            return await _dbSet.CountAsync();
        }
    }
}
