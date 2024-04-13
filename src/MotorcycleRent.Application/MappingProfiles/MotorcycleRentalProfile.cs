namespace MotorcycleRent.Application.MappingProfiles;

public sealed class MotorcycleRentalProfile : Profile
{
    public MotorcycleRentalProfile()
    {
        CreateMap<MotorcycleRentalDto, MotorcycleRental>();
    }
}
