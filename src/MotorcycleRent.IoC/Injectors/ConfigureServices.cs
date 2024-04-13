using MotorcycleRent.Domain.Services;

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

        services.AddScoped<IRentalServiceOrchestrator, RentalServiceOrchestrator>()
            .Decorate<IRentalServiceOrchestrator>((inner, serviceProvider) => ValidationInterceptor.Create(inner, serviceProvider));

        services.AddScoped<IDriverLicenseImageHandlerService, DriverLicenseImageHandlerService>()
            .Decorate<IDriverLicenseImageHandlerService>((inner, serviceProvider) => ValidationInterceptor.Create(inner, serviceProvider));   

        services.AddScoped<IOrderServiceOrchestrator, OrderServiceOrchestrator>()
            .Decorate<IOrderServiceOrchestrator>((inner, serviceProvider) => ValidationInterceptor.Create(inner, serviceProvider));

        services.AddScoped<IEmailClaimProvider, EmailClaimProvider>();

        services.AddSingleton<IPasswordHashingService, PasswordHashingService>();
        services.AddSingleton<ITokenGeneratorService, TokenGeneratorService>();

        return services;
    }

    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddSingleton<IRentalCostCalculatorService, WeeklyRentalCostCalculatorService>();
        services.AddSingleton<IRentalCostCalculatorService, BiweeklyRentalCostCalculatorService>();
        services.AddSingleton<IRentalCostCalculatorService, MonthlyRentalCostCalculatorService>();

        services.AddScoped<IOrderStatusManagementService, OrderStatusManagementService>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<IDatabaseSeedService, DatabaseSeedService>();
        services.AddSingleton(typeof(IPublisher<>), typeof(BaseServiceBusPublisher<>));

        return services;
    }
}
