namespace MotorcycleRent.Application.Models.Options;

public sealed record ServiceBusOptions
{
    public string? ConnectionString { get; init; }
    public string? QueueName { get; init; }
}
