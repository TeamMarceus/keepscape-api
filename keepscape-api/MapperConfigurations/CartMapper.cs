using AutoMapper;
using keepscape_api.Dtos.Carts;
using keepscape_api.Models;

namespace keepscape_api.MapperConfigurations
{
    public class CartMapper : Profile
    {
        public CartMapper()
        {
            CreateMap<CartItem, CartItemResponseDto>()
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product!.Images.ToList()[0].ImageUrl))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product!.Id))
                .ForMember(dest => dest.CustomizationMessage, opt => opt.MapFrom(src => src.CustomizationMessage))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                ;

            CreateMap<Cart, CartResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                ;
        }
    }
}
