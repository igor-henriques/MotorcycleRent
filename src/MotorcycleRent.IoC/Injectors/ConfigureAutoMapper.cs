namespace MotorcycleRent.IoC.Injectors;

public static class ConfigureAutoMapper
{
    public static IServiceCollection AddMapperProfiles(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(UserProfile));
        services.AddAutoMapper(typeof(MotorcycleProfile));
        services.AddAutoMapper(typeof(DriverLicenseProfile));
        services.AddAutoMapper(typeof(RentProfile));

        return services;
    }
}
