using keepscape_api.Dtos.Dashboards.Admin;
using keepscape_api.Dtos.Dashboards.Seller;
using keepscape_api.Enums;
using keepscape_api.Repositories.Interfaces;

namespace keepscape_api.Services.Dashboards
{
    public class DashboardService : IDashboardService
    {
        private readonly ISellerApplicationRepository _sellerApplicationRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public DashboardService(
            ISellerApplicationRepository sellerApplicationRepository,
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IUserRepository userRepository
        )
        {
            _sellerApplicationRepository = sellerApplicationRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        public async Task<AdminDashboardResponseDto> GetAdminDashboard()
        {
            var dashboard = new AdminDashboardResponseDto();

            var users = await _userRepository.GetAllAsync();

            var sellerApplications = await _sellerApplicationRepository.GetAllAsync();
            var pendingSellerApplications = sellerApplications.Where(sa => sa.Status == ApplicationStatus.Pending);
            dashboard.SellerApplications = pendingSellerApplications.Count();

            var orders = await _orderRepository.GetAllAsync();
            var pendingOrders = orders.Where(o => o.Status == OrderStatus.Pending);
            dashboard.OngoingOrders = pendingOrders.Count();

            var products = await _productRepository.GetAllAsync();
            dashboard.Products = products.Count();

            for (var i = 1; i <= 12; i++)
            {
                var month = (Month)i;
                var monthlyStats = new AdminMonthlyStatsDto
                {
                    Products = products.Count(p => p.DateTimeCreated.Month == (int)month && p.DateTimeCreated.Year == DateTime.UtcNow.Year),
                    Buyers = users
                    .Count(u => u.DateTimeCreated.Month == (int)month && u.UserType == UserType.Buyer && u.DateTimeCreated.Year == DateTime.UtcNow.Year),
                    Sellers = users
                    .Count(u => u.DateTimeCreated.Month == (int)month && u.UserType == UserType.Seller && u.DateTimeCreated.Year == DateTime.UtcNow.Year)
                };

                dashboard.MonthlyStatistics.Add(month, monthlyStats);
            }

            return dashboard;
        }

        public async Task<SellerDashboardResponseDto?> GetSellerDashboard(Guid sellerId)
        {
            var seller = await _userRepository.GetByIdAsync(sellerId);

            if (seller == null)
            {
                return null;
            }

            var sellerOrders = await _orderRepository.GetBySellerProfileId(seller.SellerProfile!.Id);

            return new SellerDashboardResponseDto
            {
                PendingOrders = sellerOrders.Count(o => o.Status == OrderStatus.Pending),
                OngoingOrders = sellerOrders.Count(o => o.Status == OrderStatus.Ongoing),
                CompletedOrders = sellerOrders.Count(o => o.Status == OrderStatus.Delivered)
            };
        }
    }
}
