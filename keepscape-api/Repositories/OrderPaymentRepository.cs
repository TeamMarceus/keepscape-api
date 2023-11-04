using keepscape_api.Data;
using keepscape_api.Enums;
using keepscape_api.Models;
using keepscape_api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace keepscape_api.Repositories
{
    public class OrderPaymentRepository : BaseRepository<OrderPayment>, IOrderPaymentRepository
    {
        public OrderPaymentRepository(APIDbContext context) : base(context)
        {
        }

        public async Task<OrderPayment?> GetOrderPaymentByPaymentMethodOrderId(string paymentMethodOrderId, PaymentMethod paymentMethod)
        {
            return await _dbSet
                .Include(op => op.Order)
                .FirstOrDefaultAsync(op => op.PaymentMethodOrderId == paymentMethodOrderId && op.PaymentMethod == paymentMethod);
        }
    }
}
