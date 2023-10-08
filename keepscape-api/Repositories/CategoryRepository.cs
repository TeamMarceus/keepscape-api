using keepscape_api.Data;
using keepscape_api.Enums;
using keepscape_api.Models.Categories;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class CategoryRepository : BaseRepository<BaseCategory>, ICategoryRepository
    {
        public CategoryRepository(APIDbContext context) : base(context)
        {
        }
        public override async Task<IEnumerable<BaseCategory>> GetAllAsync()
        {
            return await _dbSet
                .Include(t => t.BaseImage)
                .ToListAsync();
        }
        public override async Task<BaseCategory?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(t => t.BaseImage)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<BaseCategory?> GetCategoriesByTypeName(CategoryType type)
        {
            return await _dbSet.Include(t => t.BaseImage)
                .FirstOrDefaultAsync(t => t.Type == type);
        }

        public async Task<BaseCategory?> GetCategoryByNameAsync(string name)
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
