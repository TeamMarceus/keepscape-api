using keepscape_api.Data;
using keepscape_api.Models;
using keepscape_api.Repositories.Interfaces;

namespace keepscape_api.Repositories
{
    public class OrderPaymentRepository : BaseRepository<OrderPayment>, IOrderPaymentRepository
    {
        public OrderPaymentRepository(APIDbContext context) : base(context)
        {
        }


    }
}
