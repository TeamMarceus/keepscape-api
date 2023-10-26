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

        public async Task<(IEnumerable<User> Buyers, int PageCount)> GetBuyers(UserQuery userQuery)
        {
            return await GetQuery(userQuery, UserType.Buyer);
        }

        public override async Task<User?> GetByIdAsync(Guid id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<(IEnumerable<User> Sellers, int PageCount)> GetSellers(UserQuery userQuery)
        {
            return await GetQuery(userQuery, UserType.Seller);
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

        private async Task<(IEnumerable<User> Users, int PageCount)> GetQuery(UserQuery userQuery, UserType userType)
        {
            IQueryable<User> query;

            if (userType == UserType.Buyer)
            {
                query = _dbSet
                .Include(u => u.BuyerProfile)
                .Where(u => u.UserType == userType);
            }
            else if (userType == UserType.Seller)
            {
                query = _dbSet
                .Include(u => u.SellerProfile)
                    .ThenInclude(u => u!.SellerApplication)
                .Include(u => u.Balance)
                    .ThenInclude(b => b!.Histories)
                .Include(u => u.Balance)
                    .ThenInclude(b => b!.Withdrawals)
                .Where(
                    u => u.UserType == userType &&
                    u.SellerProfile!.SellerApplication!.Status == ApplicationStatus.Approved
                );
            }
            else
            {
                query = _dbSet
                .Where(u => u.UserType == userType);
            }

            int pageCount = 1;

            if (!string.IsNullOrEmpty(userQuery.Search))
            {
                query = query.Where(
                        u => u.FirstName.Contains(userQuery.Search) ||
                        u.LastName.Contains(userQuery.Search) ||
                        u.Email.Contains(userQuery.Search) ||
                        (u.SellerProfile != null && u.SellerProfile.Name.Contains(userQuery.Search))
                        );
            }

            if (!string.IsNullOrEmpty(userQuery.OrderBy))
            {
                if (userQuery.OrderBy == "FirstName")
                {
                    query = query.OrderBy(u => u.FirstName);
                }
                else if (userQuery.OrderBy == "LastName")
                {
                    query = query.OrderBy(u => u.LastName);
                }
                else if (userQuery.OrderBy == "Email")
                {
                    query = query.OrderBy(u => u.Email);
                }
                else if (userQuery.OrderBy == "PhoneNumber")
                {
                    query = query.OrderBy(u => u.PhoneNumber);
                }
                else if (userQuery.OrderBy == "DateTimeCreated")
                {
                    query = query.OrderBy(u => u.DateTimeCreated);
                }
                else if (userQuery.OrderBy == "DateTimeUpdated")
                {
                    query = query.OrderBy(u => u.DateTimeUpdated);
                }
                else
                {
                    query = query.OrderBy(u => u.DateTimeCreated);
                }
            }
            else
            {
                query = query.OrderBy(u => u.DateTimeCreated);
            }

            if (userQuery.IsDescending)
            {
                query = query.OrderByDescending(u => u.DateTimeCreated);
            }

            if (userQuery.IsBanned != null)
            {
                query = query.Where(u => u.IsBanned == userQuery.IsBanned);
            }

            if (query.Count() == 0)
            {
                return (new List<User>().AsQueryable(), 0);
            }

            if (userQuery.Page > 0 && userQuery.PageSize > 0)
            {
                int queryPageCount = query.Count();

                pageCount = (int)Math.Ceiling((double)queryPageCount / (int)userQuery.PageSize);

                if (userQuery.Page > pageCount)
                {
                    userQuery.Page = pageCount;
                }
                else if (userQuery.Page < 1)
                {
                    userQuery.Page = 1;
                }
                else if (pageCount == 0)
                {
                    pageCount = 1;
                    userQuery.Page = 1;
                }

                int skipAmount = ((int)userQuery.Page - 1) * (int)userQuery.PageSize;
                query = query.Skip(skipAmount).Take((int)userQuery.PageSize);
            }

            return (await query.ToListAsync(), pageCount);
        }
    }
}
