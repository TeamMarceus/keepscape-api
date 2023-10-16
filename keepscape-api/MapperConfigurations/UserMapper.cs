using AutoMapper;
using keepscape_api.Dtos.Users;
using keepscape_api.Models;

namespace keepscape_api.MapperConfigurations
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<UserCreateBuyerDto, User>();
            CreateMap<UserCreateSellerDto, User>();
            CreateMap<UserCreateBuyerDto, BuyerProfile>();
            CreateMap<UserCreateSellerDto, SellerProfile>();

            CreateMap<BuyerProfile, UserResponseBuyerDto>()
                .ForMember(dest => dest.BuyerProfileId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User!.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User!.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(opt => opt.User!.Email));

            CreateMap<SellerProfile, UserResponseSellerDto>()
                .ForMember(dest => dest.SellerProfileId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SellerName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User!.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User!.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(opt => opt.User!.Email));

            CreateMap<User, UserResponseAdminDto>();
            CreateMap<User, UserResponseBuyerDto>()
                .ForMember(dest => dest.BuyerProfileId, opt => opt.MapFrom(src => src.BuyerProfile!.Id))
                .ForMember(dest => dest.Interests, opt => opt.MapFrom(src => src.BuyerProfile!.Interests))
                .ForMember(dest => dest.Preferences, opt => opt.MapFrom(src => src.BuyerProfile!.Preferences))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.BuyerProfile!.Description));

            CreateMap<User, UserResponseSellerDto>()
                .ForMember(dest => dest.SellerProfileId, opt => opt.MapFrom(src => src.SellerProfile!.Id))
                .ForMember(dest => dest.SellerApplicationId, opt => opt.MapFrom(src => src.SellerProfile!.SellerApplication!.Id))
                .ForMember(dest => dest.SellerName, opt => opt.MapFrom(src => src.SellerProfile!.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.SellerProfile!.Description));

            CreateMap<SellerApplication, UserSellerApplicationDto>()
                .ForMember(dest => dest.Id , opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.IdUrl, opt => opt.MapFrom(src => src.BaseImage!.Url))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.SellerProfile!. User!.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.SellerProfile!.User!.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.SellerProfile!.User!.Email))
                .ForMember(dest => dest.SellerName, opt => opt.MapFrom(src => src.SellerProfile!.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.SellerProfile!.Description));
        }
    }
}
