namespace MotorcycleRent.Api.Endpoints;

public static class DriverLicenseEndpoints
{
    public static void ConfigureDriverLicenseEndpoints(this WebApplication app)
    {
        app.MapPost(Routes.DriverLicense.Create, CreateDriverLicense)
           .WithTags(SwaggerTags.DriverLicense)           
           .Accepts<DriverLicenseDto>("multipart/form-data")
           .RequireAuthorization(Roles.DeliveryPartner);

        app.MapPatch(Routes.DriverLicense.Update, UpdateDriverLicense)
           .WithTags(SwaggerTags.DriverLicense)
           .Accepts<DriverLicenseDto>("multipart/form-data")
           .RequireAuthorization(Roles.DeliveryPartner); ;
    }

    private static async Task<IResult> CreateDriverLicense(
        [FromServices] IDriverLicenseServiceOrchestrator service,
        DriverLicenseDto driverLicenseDto,
        CancellationToken cancellationToken)
    {
        await service.CreateDriverLicense(driverLicenseDto, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> UpdateDriverLicense(
        [FromServices] IDriverLicenseServiceOrchestrator service,
        DriverLicenseDto driverLicenseDto,
        CancellationToken cancellationToken)
    {
        await service.UpdateDriverLicense(driverLicenseDto, cancellationToken);
        return Results.NoContent();
    }
}
