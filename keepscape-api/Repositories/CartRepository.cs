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
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p!.Images)
                .ToListAsync();
        }
        public override async Task<Cart?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p!.Images)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Cart?> GetCartByBuyerProfileId(Guid buyerProfileId)
        {
            var cart = await _dbSet
                .Include(t => t.CartItems)
                    .ThenInclude(t => t.Product)
                        .ThenInclude(t => t!.Images)
                .AsSplitQuery()
                .FirstOrDefaultAsync(t => t.BuyerProfileId == buyerProfileId);

            if (cart != null && cart.CartItems != null)
            {
                cart.CartItems = cart.CartItems.OrderBy(ci => ci.DateTimeCreated).ToList();
            }

            return cart;
        }

        public new async Task<bool> UpdateAsync(Cart cart)
        {
            foreach (var cartItem in cart.CartItems)
            {
                _context.Products.Attach(cartItem.Product!);
            }

            return await base.UpdateAsync(cart);
        }
    }
}
