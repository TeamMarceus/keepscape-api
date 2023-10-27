using keepscape_api.Data;
using keepscape_api.Models;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class CartRepository : BaseRepository<Cart>, ICartRepository
    {
        public CartRepository(APIDbContext context) : base(context)
        {
        }
        public override async Task<IEnumerable<Cart>> GetAllAsync()
        {
            return await _dbSet
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p!.Images)
                .ToListAsync();
        }
        public override async Task<Cart?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p!.Images)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cart?> GetCartByBuyerProfileId(Guid buyerProfileId)
        {
            var cart = await _dbSet
                .Include(t => t.Items)
                    .ThenInclude(t => t.Product)
                        .ThenInclude(t => t!.SellerProfile)
                            .ThenInclude(t => t!.User)
                .Include(t => t.Items)
                    .ThenInclude(t => t.Product)
                        .ThenInclude(t => t!.Images)
                .AsSplitQuery()
                .FirstOrDefaultAsync(t => t.BuyerProfileId == buyerProfileId);

            if (cart != null && cart.Items != null)
            {
                cart.Items = cart.Items.OrderBy(ci => ci.DateTimeCreated).ToList();
            }

            return cart;
        }

        public new async Task<bool> UpdateAsync(Cart cart)
        {
            foreach (var cartItem in cart.Items)
            {
                var buyerProfile = cart.BuyerProfile;

                if (buyerProfile != null)
                {
                    _context.BuyerProfiles.Attach(cart.BuyerProfile!);
                }
            }
            
            return await base.UpdateAsync(cart);
        }
    }
}
