namespace MotorcycleRent.IoC.Injectors;

public static class ConfigureValidators
{
    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<MotorcycleDto>, MotorcycleDtoValidator>();
        services.AddScoped<IValidator<UserDto>, UserDtoValidator>();
        services.AddScoped<IValidator<DeliveryPartnerDto>, DeliveryPartnerDtoValidator>();
        services.AddScoped<IValidator<IFormFile>, DriverLicenseImageValidator>();
        services.AddScoped<IValidator<MotorcycleRentDto>, MotorcycleRentDtoValidator>();
        services.AddScoped<IValidator<DriverLicenseDto>, DriverLicenseDtoValidator>();

        return services;
    }
}
