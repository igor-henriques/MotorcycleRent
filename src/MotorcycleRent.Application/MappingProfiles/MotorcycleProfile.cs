namespace MotorcycleRent.Application.MappingProfiles;

public sealed class MotorcycleProfile : Profile
{
    public MotorcycleProfile()
    {
        CreateMap<MotorcycleDto, Motorcycle>()
            .ForMember(dest => dest.Plate, opt => opt.MapFrom(src => src.Plate!.ToUpper()))
            .ReverseMap();
    }
}
