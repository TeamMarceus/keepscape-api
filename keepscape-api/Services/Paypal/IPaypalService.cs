namespace keepscape_api.Services.Paypal
{
    public interface IPaypalService
    {
        public Task<bool> ValidatePaypalPayment(Guid userId, Guid PaypalOrderId);
    }
}
