using keepscape_api.Enums;
using keepscape_api.Models;
using keepscape_api.Repositories.Generics;

namespace keepscape_api.Repositories.Interfaces
{
    public interface IOrderPaymentRepository : IBaseRepository<OrderPayment>
    {
        public Task<OrderPayment?> GetOrderPaymentByPaymentMethodOrderId(string paymentMethodOrderId, PaymentMethod paymentMethod);
    }
}
