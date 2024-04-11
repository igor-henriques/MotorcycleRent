namespace MotorcycleRent.IoC.Injectors;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IMotorcycleServiceOrchestrator, MotorcycleServiceOrchestrator>()
            .Decorate<IMotorcycleServiceOrchestrator>((inner, serviceProvider) => ValidationInterceptor.Create(inner, serviceProvider));

        services.AddScoped<IUserServiceOrchestrator, UserServiceOrchestrator>()
            .Decorate<IUserServiceOrchestrator>((inner, serviceProvider) => ValidationInterceptor.Create(inner, serviceProvider));    

        services.AddScoped<IDriverLicenseServiceOrchestrator, DriverLicenseServiceOrchestrator>()
            .Decorate<IDriverLicenseServiceOrchestrator>((inner, serviceProvider) => ValidationInterceptor.Create(inner, serviceProvider));

        services.AddScoped<IDriverLicenseImageHandlerService, DriverLicenseImageHandlerService>();

        services.AddSingleton<IPasswordHashingService, PasswordHashingService>();
        services.AddSingleton<ITokenGeneratorService, TokenGeneratorService>();

        return services;
    }

    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IDatabaseSeedService, DatabaseSeedService>();
        return services;
    }
}
