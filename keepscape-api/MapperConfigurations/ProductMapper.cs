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
            CreateMap<Product, ProductResponseDto>();

            CreateMap<Category, ProductCategoryPlaceDto>();

            CreateMap<Place, ProductCategoryPlaceDto>();

            CreateMap<Product, ProductResponseHomeDto>()
                .ForMember(dest => dest.Stars, opt => opt.MapFrom(src => Math.Round(src.Rating)))
                .ForMember(dest => dest.Province, opt => opt.MapFrom(src => new ProductCategoryPlaceNoImageDto
                {
                    Id = src.Place != null ? src.Place!.Id : Guid.Empty,
                    Name = src.Place != null ? src.Place!.Name : string.Empty,
                }))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Images.ToList()[0].Url));

            CreateMap<ProductReview, ProductReviewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.BuyerProfile!.User!.FirstName));

            CreateMap<ProductReviewCreateDto, ProductReview>();
           
            CreateMap<ProductCreateDto, Product>()
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.CategoryIds.Select(c => new Category
                {
                    Id = c 
                })))
                .ForMember(dest => dest.Place, opt => opt.MapFrom(src => new Place { Id = src.PlaceId }))
                .ForMember(dest => dest.PlaceId , opt => opt.MapFrom(src => src.PlaceId));
            
            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => !src.Images.IsNullOrEmpty() ? src.Images.Select(i => i.Url) : new List<string>()))
                .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.Place != null ? new ProductCategoryPlaceNoImageDto
                {
                    Id = src.Place.Id,
                    Name = src.Place.Name,
                } : null))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => !src.Categories.IsNullOrEmpty() ? src.Categories.Select(c => new ProductCategoryPlaceNoImageDto
                {
                    Id = c != null ? c.Id : Guid.Empty,
                    Name = c != null ? c.Name : string.Empty
                }) : new List<ProductCategoryPlaceNoImageDto>()))
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => !src.Reviews.IsNullOrEmpty() ? src.Reviews.Select(r => new ProductReviewDto
                {
                    UserName = r.BuyerProfile!.User!.FirstName,
                    Description = r.Review,
                    Rating = r.Rating
                }): new List<ProductReviewDto>()))
                .ForMember(dest => dest.Seller, opt => opt.MapFrom(src => src.SellerProfile != null ? new ProductSellerDto
                {
                    Name = src.SellerProfile!.Name,
                    Description = src.SellerProfile!.Description,
                    Email = src.SellerProfile.User != null ? src.SellerProfile!.User!.Email : string.Empty,
                    Phone = src.SellerProfile.User != null ? src.SellerProfile!.User!.PhoneNumber : string.Empty,
                }: null))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => Math.Round(src.Rating)))
                ;
        }
    }
}
