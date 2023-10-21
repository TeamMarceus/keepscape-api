using AutoMapper;
using keepscape_api.Dtos.Orders;
using keepscape_api.Models;

namespace keepscape_api.MapperConfigurations
{
    public class OrderMapper : Profile
    {
        public OrderMapper()
        {
            CreateMap<OrderItem, OrderItemResponseDto>();
            CreateMap<Order, OrderResponseDto>();
            CreateMap<Order, OrderAdminResponseDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.Select(i => new OrderItemResponseDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product != null ? i.Product.Name : "",
                    CustomizedMessage = i.CustomizedMessage ?? "",
                    Quantity = i.Quantity,
                    Price = i.Price,
                })))
                .ForMember(dest => dest.ReportId, opt => opt.MapFrom(src => src.OrderReport!.Id))
                ;
            CreateMap<Order, OrderSellerResponseDto>()
                .ForMember(dest => dest.BuyerProfile, opt => opt.MapFrom(src => new OrderBuyerDto
                {
                    AltMobileNumber = src.BuyerProfile!.AltMobileNumber,
                    DeliveryAddress = src.BuyerProfile!.DeliveryAddress,
                    DeliveryFullName = src.BuyerProfile!.DeliveryFullName,
                    Email = src.BuyerProfile!.User!.Email,
                    FirstName = src.BuyerProfile!.User!.FirstName,
                    LastName = src.BuyerProfile!.User!.LastName,
                    PhoneNumber = src.BuyerProfile!.User!.PhoneNumber,
                    Preferences = src.BuyerProfile!.Preferences,
                    Interests = src.BuyerProfile!.Interests,
                    Description = src.BuyerProfile!.Description,
                }))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.Select(i => new OrderItemResponseDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product != null ? i.Product.Name : "",
                    CustomizedMessage = i.CustomizedMessage ?? "",
                    Quantity = i.Quantity,
                    Price = i.Price,
                })))
                .ForMember(dest => dest.DeliveryLogs, opt => opt.MapFrom(src => src.DeliveryLogs.Select(i => new OrderDeliveryLogDto
                {
                    DateTime = i.DateTime,
                    Log = i.Log,
                })));
        }
    }
}
