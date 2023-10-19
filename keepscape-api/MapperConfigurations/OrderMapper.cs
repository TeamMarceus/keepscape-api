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
            CreateMap<Order, OrderResponseAdminDto>()
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
        }
    }
}
