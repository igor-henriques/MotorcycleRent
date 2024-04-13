namespace MotorcycleRent.Api.Endpoints;

public static class OrderEndpoints
{
    public static void ConfigureOrderEndpoints(this WebApplication app)
    {
        app.MapPost(Routes.Order.Create, CreateOrder)
           .WithTags(SwaggerTags.Order)
           .RequireAuthorization(Roles.Administrator);

        app.MapGet(Routes.Order.NotifiedPartners, GetNotifiedPartners)
           .WithTags(SwaggerTags.Order)
           .RequireAuthorization(Roles.Administrator);

        app.MapGet(Routes.Order.CheckAvailability, CheckOrderAvailability)
           .WithTags(SwaggerTags.Order)
           .RequireAuthorization(Roles.DeliveryPartner);

        app.MapPatch(Routes.Order.UpdateStatus, UpdateOrderStatus)
           .WithTags(SwaggerTags.Order)
           .RequireAuthorization(Roles.DeliveryPartner);          
    }

    private static async Task<IResult> CreateOrder(
       [FromServices] IOrderServiceOrchestrator service,
       [FromBody] OrderDto createOrder,
       CancellationToken cancellationToken)
    {
        var createdOrder = await service.CreateOrderAsync(createOrder, cancellationToken);
        return Results.Ok(createdOrder);
    }

    private static async Task<IResult> CheckOrderAvailability(
       [FromServices] IOrderServiceOrchestrator service,
       [FromQuery] PublicOrderIdDto publicOrderId,
       CancellationToken cancellationToken)
    {
        _ = await service.CheckOrderAvailabilityAsync(publicOrderId, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> UpdateOrderStatus(
       [FromServices] IOrderServiceOrchestrator service,
       [FromBody] UpdateOrderStatusDto updateOrderStatus,
       CancellationToken cancellationToken)
    {
        await service.UpdateOrderStatusAsync(updateOrderStatus, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> GetNotifiedPartners(
      [FromServices] IOrderServiceOrchestrator service,
      [FromQuery] PublicOrderIdDto publicOrderIdDto,
      CancellationToken cancellationToken)
    {
        var notifiedPartners = await service.GetNotifiedPartnersAsync(publicOrderIdDto, cancellationToken);
        return Results.Ok(notifiedPartners);
    }
}
