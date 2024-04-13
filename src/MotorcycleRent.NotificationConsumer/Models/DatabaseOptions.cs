namespace MotorcycleRent.NotificationConsumer.Models;

internal sealed record DatabaseOptions
{
    public string? ConnectionString { get; init; }
    public string? DatabaseName { get; init; }
    public string? UserCollectionName { get; init; }
    public string? OrderCollectionName { get; init; }

    public string? GetCollectionName<TEntity>()
    {
        var entityName = typeof(TEntity).Name;

        return entityName switch
        {
            nameof(DeliveryPartner) => UserCollectionName,
            nameof(Order) => OrderCollectionName,
            _ => throw new InvalidOperationException($"Object {entityName} do not have a matching collection")
        };
    }
}
