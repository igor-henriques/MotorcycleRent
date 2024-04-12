namespace MotorcycleRent.Application.MappingProfiles;

public sealed class RentProfile : Profile
{
    public RentProfile()
    {
        CreateMap<MotorcycleRentDto, Domain.Entities.MotorcycleRent>();
    }
}
