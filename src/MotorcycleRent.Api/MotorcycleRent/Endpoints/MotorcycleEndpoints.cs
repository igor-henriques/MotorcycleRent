﻿namespace MotorcycleRent.Api.Endpoints;

public static class MotorcycleEndpoints
{
    public static void ConfigureMotorcycleEndpoints(this WebApplication app)
    {
        app.MapPost(Routes.Motorcycle.Create, CreateMotorcycle)
           .WithTags(SwaggerTags.Motorcycle)
           .RequireAuthorization(Roles.Administrator);

        app.MapGet(Routes.Motorcycle.List, ListMotorcycles)
           .WithTags(SwaggerTags.Motorcycle)
           .RequireAuthorization(Roles.Administrator);

        app.MapPatch(Routes.Motorcycle.UpdatePlate, UpdateMotorcyclePlate)
           .WithTags(SwaggerTags.Motorcycle)
           .RequireAuthorization(Roles.Administrator);

        app.MapPatch(Routes.Motorcycle.UpdateState, UpdateMotorcycleState)
           .WithTags(SwaggerTags.Motorcycle)
           .RequireAuthorization(Roles.Administrator);

        app.MapDelete(Routes.Motorcycle.Delete, DeleteMotorcycle)
           .WithTags(SwaggerTags.Motorcycle)
           .RequireAuthorization(Roles.Administrator);
    }

    private static async Task<IResult> CreateMotorcycle(
        [FromServices] IMotorcycleServiceOrchestrator service,
        [FromBody] MotorcycleDto motorcycle,
        CancellationToken cancellationToken)
    {
        Guid motorcycleId = await service.CreateMotorcycleAsync(motorcycle, cancellationToken);
        return Results.Ok(motorcycleId);
    }

    private static async Task<IResult> ListMotorcycles(
        [FromServices] IMotorcycleServiceOrchestrator service,
        [AsParameters] MotorcycleSearchCriteria searchCriteria,
        CancellationToken cancellationToken)
    {
        var motorcycles = await service.ListMotorcyclesAsync(searchCriteria, cancellationToken);
        return Results.Ok(motorcycles);
    }

    private static async Task<IResult> UpdateMotorcyclePlate(
        [FromServices] IMotorcycleServiceOrchestrator service,
        [FromBody] UpdateMotorcyclePlateDto plateUpdateDto,
        CancellationToken cancellationToken)
    {
        await service.UpdateMotorcyclePlateAsync(plateUpdateDto, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> UpdateMotorcycleState(
        [FromServices] IMotorcycleServiceOrchestrator service,
        [FromBody] UpdateMotorcycleStateDto stateUpdateDto,
        CancellationToken cancellationToken)
    {
        await service.UpdateMotorcycleStateAsync(stateUpdateDto, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteMotorcycle(
        [FromServices] IMotorcycleServiceOrchestrator service,
        [FromRoute] string motorcyclePlate,
        CancellationToken cancellationToken)
    {
        await service.DeleteMotorcycleAsync(motorcyclePlate, cancellationToken);
        return Results.NoContent();
    }
}
