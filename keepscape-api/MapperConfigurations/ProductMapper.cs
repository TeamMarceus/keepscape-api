using AutoMapper;
using keepscape_api.Dtos.Products;
using keepscape_api.Models;
using keepscape_api.Models.Categories;

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
                .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.Place!.Name))
                .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Images.ToList()[0].Url));

            CreateMap<ProductReview, ProductReviewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.BuyerProfile!.User!.FirstName));
            CreateMap<ProductCreateDto, Product>()
                .ForMember(dest => dest.Images, opt => opt.Ignore())
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.CategoryIds.Select(c => new Category
                {
                    Id = c
                })))
                .ForMember(dest => dest.Place, opt => opt.MapFrom(src => new Place { Id = src.PlaceId }))
                .ForMember(dest => dest.PlaceId , opt => opt.MapFrom(src => src.PlaceId));
            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images.Select(i => i.Url)))
                .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.Place!.Name))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(c => c.Name)))
                .ForMember(dest => dest.Reviews, opt => opt.MapFrom(src => src.Reviews.Select(r => new ProductReviewDto
                {
                    UserName = r.BuyerProfile!.User!.FirstName,
                    Description = r.Review,
                    Rating = r.Rating
                })))
                .ForMember(dest => dest.Seller, opt => opt.MapFrom(src => new ProductSellerDto
                {
                    Name = src.SellerProfile!.Name,
                    Description = src.SellerProfile!.Description,
                    Email = src.SellerProfile!.User!.Email,
                    Phone = src.SellerProfile!.User!.PhoneNumber
                }))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => Math.Round(src.Rating)))
                ;
        }
    }
}
