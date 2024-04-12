namespace MotorcycleRent.Api.Endpoints;

public static class UserEndpoints
{
    public static void ConfigureUserEndpoints(this WebApplication app)
    {
        app.MapPost(Routes.User.CreateAdmin, CreateAdministrator)
           .WithTags(SwaggerTags.User)
           .RequireAuthorization(Roles.Administrator);

        app.MapPost(Routes.User.CreateDeliveryPartner, CreateDeliveryPartner)
           .WithTags(SwaggerTags.User);

        app.MapPost(Routes.User.Authenticate, Authenticate)
           .WithTags(SwaggerTags.User);
    }

    private static async Task<IResult> CreateAdministrator(
        [FromServices] IUserServiceOrchestrator service,
        [FromBody] AdministratorDto administratorDto,
        CancellationToken cancellationToken)
    {
        await service.CreateAdministratorAsync(administratorDto, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> CreateDeliveryPartner(
        [FromServices] IUserServiceOrchestrator service,
        [FromBody] DeliveryPartnerDto administratorDto,
        CancellationToken cancellationToken)
    {
        await service.CreateDeliveryPartnerAsync(administratorDto, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> Authenticate(
        [FromServices] IUserServiceOrchestrator service,
        [FromBody] UserDto userDto,
        CancellationToken cancellationToken)
    {
        var token = await service.AuthenticateAsync(userDto, cancellationToken);
        return Results.Ok(token);
    }
}
