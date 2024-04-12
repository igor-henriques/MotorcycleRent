namespace MotorcycleRent.Api.Endpoints;

public static class RentEndpoints
{
    public static void ConfigureRentEndpoints(this WebApplication app)
    {
        app.MapPost(Routes.Rent.Create, CreateMotorcycleRent)
           .WithTags(SwaggerTags.MotorcycleRent)
           .RequireAuthorization(Roles.DeliveryPartner);

        app.MapPost(Routes.Rent.PeekPrice, PeekRentPrice)
           .WithTags(SwaggerTags.MotorcycleRent);

        app.MapPatch(Routes.Rent.Return, ReturnMotorcycleRent)
           .WithTags(SwaggerTags.MotorcycleRent)
           .RequireAuthorization(Roles.DeliveryPartner);
    }

    private static async Task<IResult> CreateMotorcycleRent(
        [FromServices] IRentServiceOrchestrator service,
        [FromBody] MotorcycleRentDto createRent,
        CancellationToken cancellationToken)
    {
        var rentCost = await service.CreateMotorcycleRentAsync(createRent, cancellationToken);
        return Results.Ok(rentCost);
    }

    private static IResult PeekRentPrice(
        [FromServices] IRentServiceOrchestrator service,
        [FromBody] MotorcycleRentDto peekCost,
        CancellationToken cancellationToken)
    {
        var rentCost = service.PeekRentPrice(peekCost);
        return Results.Ok(rentCost);
    }

    private static async Task<IResult> ReturnMotorcycleRent(
        [FromServices] IRentServiceOrchestrator service,
        CancellationToken cancellationToken)
    {
        await service.ReturnMotorcycleRentAsync(cancellationToken);
        return Results.NoContent();
    }
}
