namespace MotorcycleRent.Api.Endpoints;

public static class RentalEndpoints
{
    public static void ConfigureRentEndpoints(this WebApplication app)
    {
        app.MapPost(Routes.Rental.Rent, RentMotorcycle)
           .WithTags(SwaggerTags.Rental)
           .RequireAuthorization(Roles.DeliveryPartner)
           .WithMetadata(SwaggerApiDescriber.RentMotorcycle());

        app.MapPost(Routes.Rental.PeekPrice, PeekRentalPrice)
           .WithTags(SwaggerTags.Rental)
           .WithMetadata(SwaggerApiDescriber.PeekRentalPrice()); 

        app.MapPatch(Routes.Rental.Return, ReturnMotorcycleRental)
           .WithTags(SwaggerTags.Rental)
           .RequireAuthorization(Roles.DeliveryPartner)
           .WithMetadata(SwaggerApiDescriber.ReturnMotorcycleRental());
    }

    private static async Task<IResult> RentMotorcycle(
        [FromServices] IRentalServiceOrchestrator service,
        [FromBody] MotorcycleRentalDto startRental,
        CancellationToken cancellationToken)
    {
        var rentCost = await service.RentMotorcycleAsync(startRental, cancellationToken);
        return Results.Ok(rentCost);
    }

    private static IResult PeekRentalPrice(
        [FromServices] IRentalServiceOrchestrator service,
        [FromBody] MotorcycleRentalDto peekCost)
    {
        var rentCost = service.PeekRentalPrice(peekCost);
        return Results.Ok(rentCost);
    }

    private static async Task<IResult> ReturnMotorcycleRental(
        [FromServices] IRentalServiceOrchestrator service,
        CancellationToken cancellationToken)
    {
        await service.ReturnMotorcycleRentalAsync(cancellationToken);
        return Results.NoContent();
    }
}
