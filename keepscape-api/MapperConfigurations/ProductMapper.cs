using AutoMapper;
using keepscape_api.Dtos.Products;
using keepscape_api.Enums;
using keepscape_api.Models;
using keepscape_api.Models.Categories;

namespace keepscape_api.MapperConfigurations
{
    public class ProductMapper : Profile
    {
        public ProductMapper()
        {
            CreateMap<Product, ProductResponseDto>()
                .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.ProductCategories.Where(p => p.Type == CategoryType.Provinces).FirstOrDefault()));
            CreateMap<BaseCategory, ProductCategoryDto>();
        }
    }
}
