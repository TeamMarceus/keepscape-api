using keepscape_api.Data;
using keepscape_api.Enums;
using keepscape_api.Models.Categories;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(APIDbContext context) : base(context)
        {
        }
        public override async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _dbSet
                .ToListAsync();
        }
        public override async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<int> GetCategoryCountAsync()
        {
            return await _dbSet.CountAsync();
        }
    }
}
