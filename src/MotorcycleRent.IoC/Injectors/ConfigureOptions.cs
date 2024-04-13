namespace MotorcycleRent.IoC.Injectors;

public static class ConfigureOptions
{
    public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RentalOptions>(configuration.GetSection(nameof(RentalOptions)));
        services.Configure<JwtAuthenticationOptions>(configuration.GetSection(nameof(JwtAuthenticationOptions)));
        services.Configure<DatabaseOptions>(configuration.GetSection(nameof(DatabaseOptions)));
        services.Configure<SeedOptions>(configuration.GetSection(nameof(SeedOptions)));
        services.Configure<DriverLicenseOptions>(configuration.GetSection(nameof(DriverLicenseOptions)));
        services.Configure<StoragingOptions>(configuration.GetSection(nameof(StoragingOptions)));
        services.Configure<PublisherOptions>(configuration.GetSection(nameof(PublisherOptions)));

        return services;
    }
}
