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
                        .ThenInclude(p => p!.ProductImages)
                            .ThenInclude(pi => pi.BaseImage)
                .ToListAsync();
        }
        public override async Task<Cart?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p!.ProductImages)
                            .ThenInclude(pi => pi.BaseImage)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Cart?> CreateCartItemByCartId(Guid cartId, CartItem cartItem)
        {
            var cart = await _dbSet.Include(t => t.CartItems).FirstOrDefaultAsync(t => t.Id == cartId);

            if (cart == null)
            {
                return null;
            }

            cart.CartItems.Add(cartItem);

            return await _context.SaveChangesAsync() > 0 ? cart : null;
        }

        public async Task<Cart?> GetCartByUserGuid(Guid buyerProfileId)
        {
            var cart = await _dbSet
                .Include(t => t.CartItems)
                    .ThenInclude(t => t.Product)
                        .ThenInclude(t => t!.ProductImages)
                            .ThenInclude(t => t.BaseImage)
                .AsSplitQuery()
                .FirstOrDefaultAsync(t => t.BuyerProfileId == buyerProfileId);

            if (cart != null && cart.CartItems != null)
            {
                cart.CartItems = cart.CartItems.OrderBy(ci => ci.DateTimeCreated).ToList();
            }

            return cart;
        }
    }
}
