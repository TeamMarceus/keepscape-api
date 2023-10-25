using AutoMapper;
using keepscape_api.Dtos.Products;
using keepscape_api.Models;
using keepscape_api.Models.Categories;
using Microsoft.IdentityModel.Tokens;

namespace keepscape_api.MapperConfigurations
{
    public class ProductMapper : Profile
    {
        public ProductMapper()
        {
            CreateMap<Category, ProductCategoryPlaceDto>();

            CreateMap<Place, ProductCategoryPlaceDto>();

            CreateMap<Product, ProductResponseHomeDto>()
                .ForMember(dest => dest.Stars, opt => opt.MapFrom(src => Math.Round(src.Rating)))
                .ForMember(dest => dest.Province, opt => opt.MapFrom(src => new ProductCategoryPlaceNoImageDto
                {
                    Id = src.Place != null ? src.Place!.Id : Guid.Empty,
                    Name = src.Place != null ? src.Place!.Name : string.Empty,
                }))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => !src.Images.IsNullOrEmpty() ? src.Images.First().ImageUrl : ""))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.BuyerPrice))
                ;

            CreateMap<ProductReview, ProductReviewDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.BuyerProfile!.User!.FirstName} {src.BuyerProfile!.User!.LastName}"));

            CreateMap<ProductReviewCreateDto, ProductReview>();

            CreateMap<ProductReview, ProductReviewResponseDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.BuyerProfile!.User!.FirstName} {src.BuyerProfile!.User!.LastName}"));

            CreateMap<ProductCreateDto, Product>()
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.CategoryIds.Select(c => new Category
                {
                    Id = c 
                })))
                .ForMember(dest => dest.Place, opt => opt.MapFrom(src => new Place { Id = src.PlaceId }))
                .ForMember(dest => dest.PlaceId , opt => opt.MapFrom(src => src.PlaceId))
                ;
            
            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => !src.Images.IsNullOrEmpty() ? src.Images.Select(i => i.ImageUrl) : new List<string>()))
                .ForMember(dest => dest.Seller, opt => opt.MapFrom(src => src.SellerProfile != null && src.SellerProfile.User != null ? new ProductSellerDto
                {
                    SellerProfileId = src.SellerProfile.Id,
                    Name = src.SellerProfile.Name,
                    Description = src.SellerProfile.Description,
                    Email = src.SellerProfile.User.Email,
                    Phone = src.SellerProfile.User.PhoneNumber,
                    Stars = (int)Math.Round(src.SellerProfile.Rating)
                }: null))
                .ForMember(dest => dest.TotalRatings, opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count : 0))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.BuyerPrice))
                .ForMember(dest => dest.Stars, opt => opt.MapFrom(src => (int)Math.Round(src.Rating)))
                .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.Place != null ? new ProductCategoryPlaceDto
                {
                    Id = src.Place.Id,
                    Name = src.Place.Name,
                    ImageUrl = src.Place.ImageUrl ?? string.Empty
                } : null))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories != null ? src.Categories.Select(c => new ProductCategoryPlaceDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    ImageUrl = c.ImageUrl ?? string.Empty
                }) : new List<ProductCategoryPlaceDto>()))
                ;
        }
    }
}
