using AutoMapper;
using keepscape_api.Dtos.Orders;
using keepscape_api.Dtos.Reports;
using keepscape_api.Models;
using Microsoft.IdentityModel.Tokens;

namespace keepscape_api.MapperConfigurations
{
    public class OrderMapper : Profile
    {
        public OrderMapper()
        {
            CreateMap<OrderItem, OrderItemResponseDto>();
            CreateMap<Order, OrderResponseDto>();
            CreateMap<Order, OrderAdminResponseDto>()
                .ForMember(dest => dest.Buyer, opt => opt.MapFrom(src => new OrderBuyerDto
                {
                    Id = src.BuyerProfile!.User!.Id,
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
                .ForMember(dest => dest.Seller, opt => opt.MapFrom(src => new OrderSellerDto
                {
                    Id = src.SellerProfile!.User!.Id,
                    Email = src.SellerProfile!.User!.Email,
                    PhoneNumber = src.SellerProfile!.User!.PhoneNumber,
                    IdImageUrl = src.SellerProfile!.SellerApplication!.IdImageUrl,
                    BusinessPermitUrl = src.SellerProfile!.SellerApplication!.BusinessPermitUrl,
                    SellerName = src.SellerProfile!.Name,
                    Description = src.SellerProfile!.Description,
                }))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.Select(i => new OrderItemResponseDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product != null ? i.Product.Name : "",
                    ProductImageUrl = i.Product != null ? !i.Product.Images.IsNullOrEmpty() ? i.Product.Images.First().ImageUrl : "" : "",
                    QRImageUrl = i.QRImageUrl ?? "",
                    CustomizationMessage = i.CustomizationMessage ?? "",
                    Quantity = i.Quantity,
                    Price = i.Price,
                })))
                .ForMember(dest => dest.DeliveryLogs, opt => opt.MapFrom(src => src.DeliveryLogs.Select(i => new OrderDeliveryLogDto
                {
                    DateTime = i.DateTime,
                    Log = i.Log,
                })))
                .ForMember(dest => dest.Report, opt => opt.MapFrom(src => new ReportOrderResponseDto
                {
                    Reason = src.OrderReport!.Reason,
                    DateTimeCreated = src.OrderReport!.DateTimeCreated,
                    UserId = src.OrderReport!.UserId,
                }))
                ;
            CreateMap<Order, OrderSellerResponseDto>()
                .ForMember(dest => dest.Buyer, opt => opt.MapFrom(src => new OrderBuyerDto
                {
                    Id = src.BuyerProfile!.User!.Id,
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
                    ProductImageUrl = i.Product != null ? !i.Product.Images.IsNullOrEmpty() ? i.Product.Images.First().ImageUrl : "" : "",
                    QRImageUrl = i.QRImageUrl ?? "",
                    CustomizationMessage = i.CustomizationMessage ?? "",
                    Quantity = i.Quantity,
                    Price = i.Price,
                })))
                .ForMember(dest => dest.DeliveryLogs, opt => opt.MapFrom(src => src.DeliveryLogs.Select(i => new OrderDeliveryLogDto
                {
                    DateTime = i.DateTime,
                    Log = i.Log,
                }).OrderByDescending(i => i.DateTime)
                ));
            CreateMap<Order, OrderBuyerResponseDto>()
                .ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => src.BuyerProfile!.DeliveryAddress))
                .ForMember(dest => dest.DeliveryFullName, opt => opt.MapFrom(src => src.BuyerProfile!.DeliveryFullName))
                .ForMember(dest => dest.AltMobileNumber, opt => opt.MapFrom(src => src.BuyerProfile!.AltMobileNumber))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.BuyerProfile!.User!.PhoneNumber))
                .ForMember(dest => dest.Seller, opt => opt.MapFrom(src => new OrderSellerDto
                {
                    Id = src.SellerProfile!.User!.Id,
                    Email = src.SellerProfile!.User!.Email,
                    PhoneNumber = src.SellerProfile!.User!.PhoneNumber,
                    IdImageUrl = src.SellerProfile!.SellerApplication!.IdImageUrl,
                    BusinessPermitUrl = src.SellerProfile!.SellerApplication!.BusinessPermitUrl,
                    SellerName = src.SellerProfile!.Name,
                    Description = src.SellerProfile!.Description,
                }))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.Select(i => new OrderItemResponseDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product != null ? i.Product.Name : "",
                    ProductImageUrl = i.Product != null ? !i.Product.Images.IsNullOrEmpty() ? i.Product.Images.First().ImageUrl : "" : "",
                    QRImageUrl = i.QRImageUrl ?? "",
                    CustomizationMessage = i.CustomizationMessage ?? "",
                    Quantity = i.Quantity,
                    Price = i.Price,
                })))
                .ForMember(dest => dest.DeliveryLogs, opt => opt.MapFrom(src => src.DeliveryLogs.Select(i => new OrderDeliveryLogDto
                {
                    DateTime = i.DateTime,
                    Log = i.Log,
                }).OrderByDescending(i => i.DateTime)
                ));
            CreateMap<OrderItemGift, OrderItemGiftDto>()
                .ForMember(dest => dest.Place, opt => opt.MapFrom(src => src.Place!.Name));
        }
    }
}
