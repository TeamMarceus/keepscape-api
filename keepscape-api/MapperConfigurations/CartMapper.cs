using AutoMapper;
using keepscape_api.Dtos.Carts;
using keepscape_api.Models;
using Microsoft.IdentityModel.Tokens;

namespace keepscape_api.MapperConfigurations
{
    public class CartMapper : Profile
    {
        public CartMapper()
        {
            CreateMap<CartItem, CartItemResponseDto>()
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => !src.Product!.Images
                .IsNullOrEmpty() ? src.Product!.Images.ToList().First().ImageUrl : ""))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product!.Id))
                .ForMember(dest => dest.IsCustomizable, opt => opt.MapFrom(src => src.Product!.IsCustomizable))
                .ForMember(dest => dest.CustomizationMessage, opt => opt.MapFrom(src => src.CustomizationMessage))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                ;
        }
    }
}
