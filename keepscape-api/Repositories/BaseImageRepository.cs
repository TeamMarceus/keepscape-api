using keepscape_api.Data;
using keepscape_api.Models;
using keepscape_api.Repositories.Interfaces;

namespace keepscape_api.Repositories
{
    public class BaseImageRepository : BaseRepository<BaseImage>, IBaseImageRepository
    {
        public BaseImageRepository(APIDbContext context) : base(context)
        {
        }
    }
}
