namespace MotorcycleRent.Application.MappingProfiles;

public sealed class DriverLicenseProfile : Profile
{
    public DriverLicenseProfile()
    {
        CreateMap<DriverLicenseDto, DriverLicense>();
    }
}
