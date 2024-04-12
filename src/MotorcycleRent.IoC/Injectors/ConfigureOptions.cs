namespace MotorcycleRent.IoC.Injectors;

public static class ConfigureOptions
{
    public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RentOptions>(configuration.GetSection(nameof(RentOptions)));
        services.Configure<JwtAuthenticationOptions>(configuration.GetSection(nameof(JwtAuthenticationOptions)));
        services.Configure<MotorcycleRentDatabaseOptions>(configuration.GetSection(nameof(MotorcycleRentDatabaseOptions)));
        services.Configure<SeedOptions>(configuration.GetSection(nameof(SeedOptions)));
        services.Configure<DriverLicenseOptions>(configuration.GetSection(nameof(DriverLicenseOptions)));
        services.Configure<StoragingOptions>(configuration.GetSection(nameof(StoragingOptions)));

        return services;
    }
}
