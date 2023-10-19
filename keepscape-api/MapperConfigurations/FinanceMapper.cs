using AutoMapper;
using keepscape_api.Dtos.Finances;
using keepscape_api.Models;

namespace keepscape_api.MapperConfigurations
{
    public class FinanceMapper : Profile
    {
        public FinanceMapper()
        {
            CreateMap<BalanceWithdrawal, BalanceWithdrawalResponseDto>()
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                ;
        }
    }
}
