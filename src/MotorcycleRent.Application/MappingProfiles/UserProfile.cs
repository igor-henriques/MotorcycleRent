namespace MotorcycleRent.Application.MappingProfiles;

public sealed class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserDto, Administrator>()
            .ForMember(dest => dest.HashedPassword, opt => opt.Ignore())
            .ForMember(dest => dest.Claims, opt => opt.Ignore());

        CreateMap<AdministratorDto, Administrator>()
            .ForMember(dest => dest.HashedPassword, opt => opt.Ignore())
            .ForMember(dest => dest.Claims, opt => opt.Ignore());

        CreateMap<DeliveryPartnerDto, DeliveryPartner>()
            .ForMember(dest => dest.HashedPassword, opt => opt.Ignore())
            .ForMember(dest => dest.Claims, opt => opt.Ignore());
    }
}
