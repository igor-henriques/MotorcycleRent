namespace MotorcycleRent.Application.Models.Options;

public sealed record PublisherOptions
{
    public string? ConnectionString { get; init; }
    public string? QueueName { get; init; }
}
