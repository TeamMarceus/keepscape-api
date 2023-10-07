using keepscape_api.Data;
using keepscape_api.Models;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(APIDbContext context) : base(context)
        {
        }
        public override async Task<IEnumerable<User>> GetAllAsync()
        {
            return await base.GetAllAsync();
        }
        public override async Task<User?> GetByIdAsync(Guid id)
        {
            return await base.GetByIdAsync(id);
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<User?> GetUserByPhoneNumberAsync(string phoneNumber)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        public Task<int> GetUserCountAsync()
        {
            return _dbSet.CountAsync();
        }
    }
}
