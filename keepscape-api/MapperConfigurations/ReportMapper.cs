using AutoMapper;
using keepscape_api.Dtos.Reports;
using keepscape_api.Models;
using keepscape_api.Models.Checkouts.Products;

namespace keepscape_api.MapperConfigurations
{
    public class ReportMapper : Profile
    {
        public ReportMapper()
        {
            CreateMap<ProductReport, ReportProductResponseDto>();
            CreateMap<OrderReport, ReportOrderResponseDto>()
                .ForMember(dest => dest.SellerId, opt => opt.MapFrom(src => src.User!.SellerProfile!.Id))
                ;
        }
    }
}
