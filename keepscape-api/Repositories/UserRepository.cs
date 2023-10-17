using keepscape_api.Data;
using keepscape_api.Enums;
using keepscape_api.Models;
using keepscape_api.QueryModels;
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

        public async Task<(IEnumerable<User> Buyers, int PageCount)> GetBuyers(PaginatorQuery paginatorQuery)
        {
            var query = _dbSet
                .Include(u => u.BuyerProfile)
                .Where(u => u.UserType == UserType.Buyer);

            int pageCount = 1;

            if (paginatorQuery.Page > 0 && paginatorQuery.PageSize > 0)
            {
                int queryPageCount = query.Count();

                pageCount = (int)Math.Ceiling((double)queryPageCount / (int)paginatorQuery.PageSize);

                if (paginatorQuery.Page > pageCount)
                {
                    paginatorQuery.Page = pageCount;
                }
                else if (paginatorQuery.Page < 1)
                {
                    paginatorQuery.Page = 1;
                }
                else if (pageCount == 0)
                {
                    pageCount = 1;
                    paginatorQuery.Page = 1;
                }

                int skipAmount = ((int)paginatorQuery.Page - 1) * (int)paginatorQuery.PageSize;
                query = query.Skip(skipAmount).Take((int)paginatorQuery.PageSize);
            }

            return (await query.ToListAsync(), pageCount);
        }

        public override async Task<User?> GetByIdAsync(Guid id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<(IEnumerable<User> Sellers, int PageCount)> GetSellers(PaginatorQuery paginatorQuery)
        {
            var query = _dbSet
                .Include(u => u.SellerProfile)
                    .ThenInclude(u => u!.SellerApplication)
                    .ThenInclude(sa => sa.BaseImage)
                .Where(u => u.UserType == UserType.Seller);

            int pageCount = 1;

            if (paginatorQuery.Page > 0 && paginatorQuery.PageSize > 0)
            {
                int queryPageCount = await query.CountAsync();

                pageCount = (int)Math.Ceiling((double)queryPageCount / (int)paginatorQuery.PageSize);

                if (paginatorQuery.Page > pageCount)
                {
                    paginatorQuery.Page = pageCount;
                }
                else if (paginatorQuery.Page < 1)
                {
                    paginatorQuery.Page = 1;
                }
                else if (pageCount == 0)
                {
                    pageCount = 1;
                    paginatorQuery.Page = 1;
                }

                int skipAmount = ((int)paginatorQuery.Page - 1) * (int)paginatorQuery.PageSize;
                query = query.Skip(skipAmount).Take((int)paginatorQuery.PageSize);
            }

            return (await query.ToListAsync(), pageCount);
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
