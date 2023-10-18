using keepscape_api.Data;
using keepscape_api.Models.Categories;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories.Interfaces
{
    public class PlaceRepository : BaseRepository<Place>, IPlaceRepository
    {
        public PlaceRepository(APIDbContext context) : base(context)
        {
        }

        public Task<int> GetPlaceCountAsync()
        {
            return _dbSet.CountAsync();
        }

        public override async Task<IEnumerable<Place>> GetAllAsync()
        {
            return await _dbSet
                .ToListAsync();
        }

        public override async Task<Place?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
