namespace MotorcycleRent.Application.Models.Options;

public sealed record StoragingOptions
{
    public string? ConnectionString { get; init; }
    public string? ContainerName { get; init; }
}
