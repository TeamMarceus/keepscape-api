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
            CreateMap<BalanceLog, BalanceLogResponseDto>();
            CreateMap<Balance, BalanceResponseDto>()
                .ForMember(dest => dest.Histories, opt => opt.MapFrom(src => src.Histories.Select(x => new BalanceLogResponseDto
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    Remarks = x.Remarks ?? "",
                })))
                .ForMember(dest => dest.Withdrawals, opt => opt.MapFrom(src => src.Withdrawals.Select(x => new BalanceWithdrawalResponseDto
                {
                    Id = x.Id,
                    BalanceId = x.BalanceId,
                    Amount = x.Amount,
                    PaymentMethod = x.PaymentMethod.ToString(),
                    PaymentDetails = x.PaymentDetails ?? "",
                    PaymentProfileImageUrl = x.PaymentProfileImageUrl ?? "",
                    PaymentProofImageUrl = x.PaymentProofImageUrl ?? "",
                    Status = x.Status.ToString(),
                    Remarks = x.Remarks ?? "",
                })));
        }
    }
}
