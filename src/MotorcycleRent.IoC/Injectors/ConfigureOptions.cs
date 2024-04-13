namespace MotorcycleRent.IoC.Injectors;

public static class ConfigureOptions
{
    public static IServiceCollection AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RentalOptions>(configuration.GetSection(nameof(RentalOptions)));
        services.Configure<JwtAuthenticationOptions>(configuration.GetSection(nameof(JwtAuthenticationOptions)));
        services.Configure<MotorcycleRentalDatabaseOptions>(configuration.GetSection(nameof(MotorcycleRentalDatabaseOptions)));
        services.Configure<SeedOptions>(configuration.GetSection(nameof(SeedOptions)));
        services.Configure<DriverLicenseOptions>(configuration.GetSection(nameof(DriverLicenseOptions)));
        services.Configure<StoragingOptions>(configuration.GetSection(nameof(StoragingOptions)));
        services.Configure<ServiceBusOptions>(configuration.GetSection(nameof(ServiceBusOptions)));

        return services;
    }
}
