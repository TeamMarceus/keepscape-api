using AutoMapper;
using keepscape_api.Dtos.Orders;
using keepscape_api.Dtos.Products;
using keepscape_api.Dtos.Reports;
using keepscape_api.Enums;
using keepscape_api.Models;
using keepscape_api.Repositories.Interfaces;
using keepscape_api.Services.Emails;
using Microsoft.IdentityModel.Tokens;

namespace keepscape_api.Services.Reports
{
    public class ReportService : IReportService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProductReportRepository _productReportRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderReportRepository _orderReportRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IBalanceRepository _balanceRepository;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        public ReportService(
            IUserRepository userRepository,
            IProductReportRepository productReportRepository, 
            IProductRepository productRepository,
            IOrderReportRepository orderReportRepository,
            IOrderRepository orderRepository,
            IBalanceRepository balanceRepository,
            IEmailService emailService,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _productReportRepository = productReportRepository;
            _productRepository = productRepository;
            _orderReportRepository = orderReportRepository;
            _orderRepository = orderRepository;
            _balanceRepository = balanceRepository;
            _emailService = emailService;
            _mapper = mapper;
        }

        public Task<bool> CreateOrderReport(Guid orderId, Guid userId, ReportRequestDto reportRequestDto)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CreateProductReport(Guid productId, Guid userId, ReportRequestDto reportRequestDto)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
            {
                return false;
            }

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            product.Reports.Add(new ProductReport
            {
                ProductId = productId,
                UserId = userId,
                Reason = reportRequestDto.Reason
            });

            var subject = $"Product with name {product.Name} has been reported";
            var email = $"<p>Hi {product.SellerProfile!.User!.FirstName},</p>" +
                        $"<p>Your product with id {product.Id} has been reported.</p>" +
                        $"<p>Reason: {reportRequestDto.Reason}</p>" +
                        $"<p>Please check your product and resolve the issue.</p>" + 
                        $"<p>Thank you for using Keepscape!</p>";

            await _emailService.SendEmailAsync(product.SellerProfile!.User!.Email, subject, email);

            return await _productRepository.UpdateAsync(product);
        }

        public async Task<IEnumerable<OrderAdminResponseDto>> GetAllOrderReports()
        {
            var orderReports = await _orderReportRepository.GetAllAsync();

            return orderReports.Select(report => _mapper.Map<OrderAdminResponseDto>(report.Order));
        }

        public async Task<IEnumerable<ProductResponseAdminDto>> GetAllProductReports()
        {
            var productReports = await _productReportRepository.GetAllAsync();
            var uniqueProducts = productReports.Select(p => p.Product).Distinct();

            var products = new List<ProductResponseAdminDto>();

            foreach (var product in uniqueProducts)
            {
                var orders = await _orderRepository.GetByProductId(product.Id);

                var totalSold = orders.Where(o => o.Status == OrderStatus.Delivered)
                                .Sum(o => o.Items.Where(i => i.ProductId == product.Id)
                                .Single().Quantity);

                var totalReports = productReports.Where(p => p.ProductId == product.Id).Count();

                products.Add(new ProductResponseAdminDto
                {
                    Id = product.Id,
                    SellerUserId = product.SellerProfile!.User!.Id,
                    DateTimeCreated = product.DateTimeCreated,
                    Name = product.Name,
                    Price = product.BuyerPrice,
                    Quantity = product.Quantity,
                    ImageUrls = product.Images.Select(i => i.ImageUrl),
                    TotalSold = totalSold,
                    TotalReports = totalReports
                });
            }

            return products;
        }

        public async Task<ReportOrderResponseDto?> GetOrderReport(Guid orderId)
        {
            var orderReport = await _orderReportRepository.GetOrderReport(orderId);

            if (orderReport == null)
            {
                return null;
            }

            return _mapper.Map<ReportOrderResponseDto>(orderReport);
        }

        public async Task<IEnumerable<ReportProductResponseDto>> GetProductReports(Guid productId)
        {
            var productReports = await _productReportRepository.GetByProductIdAsync(productId);

            if (productReports.IsNullOrEmpty())
            {
                return Enumerable.Empty<ReportProductResponseDto>();
            }

            return productReports.Select(report => _mapper.Map<ReportProductResponseDto>(report));
        }

        public async Task<bool> RefundOrderReport(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                return false;
            }

            if (order.Status != OrderStatus.AwaitingConfirmation)
            {
                return false;
            }

            order.OrderReport!.IsRefunded = true;
            order.Status = OrderStatus.Refunded;

            var balance = await _balanceRepository.GetBalanceByUserId(order.BuyerProfile!.UserId);

            if (balance == null)
            {
                return false;
            }

            balance.Amount += order.TotalPrice;

            var email = $"<p>Hi {order.BuyerProfile!.User!.FirstName},</p>" +
                        $"<p>Your order with id {order.Id} will be refunded.</p>" +
                        $"<p>Please wait for our refund in a few days.</p>" + 
                        $"<p>Thank you for using Keepscape!</p>";

            await _emailService.SendEmailAsync(order.BuyerProfile!.User!.Email, "Order Refunded", email);

            return await _orderRepository.UpdateAsync(order) && await _balanceRepository.UpdateAsync(balance);
        }

        public async Task<bool> ResolveOrderReport(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                return false;
            }

            if (order.Status != OrderStatus.AwaitingConfirmation)
            {
                return false;
            }

            order.OrderReport!.IsResolved = true;
            order.Status = OrderStatus.Delivered;

            var balance = await _balanceRepository.GetBalanceByUserId(order.SellerProfile!.UserId);

            if (balance == null)
            {
                return false;
            }

            balance.Amount += order.TotalPrice;

            return await _orderRepository.UpdateAsync(order) && await _balanceRepository.UpdateAsync(balance);
        }

        public async Task<bool> ResolveProductReports(Guid productId)
        {
            var productReports = await _productReportRepository.GetByProductIdAsync(productId);

            if (productReports.IsNullOrEmpty())
            {
                return false;
            }

            foreach (var report in productReports)
            {
                report.IsResolved = true;
            }

            return await _productReportRepository.UpdateRangeAsync(productReports);
        }
    }
}
