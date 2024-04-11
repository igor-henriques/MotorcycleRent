namespace MotorcycleRent.Application.Models.Options;

public sealed record MotorcycleRentDatabaseOptions
{
    public string? ConnectionString { get; init; }
    public string? DatabaseName { get; init; }
    public string? UserCollectionName { get; init; }
    public string? MotorcycleCollectionName { get; init; }
    public string? OrderCollectionName { get; init; }
    public string? MotorcycleRentCollectionName { get; init; }

    public string? GetCollectionName<TEntity>()
    {
        var entityName = typeof(TEntity).Name;

        return entityName switch
        {
            nameof(DeliveryPartner) => UserCollectionName,
            nameof(Administrator) => UserCollectionName,
            var name when nameof(UserCollectionName).Contains(name, StringComparison.CurrentCulture) => UserCollectionName,
            var name when nameof(MotorcycleCollectionName).Contains(name, StringComparison.CurrentCulture) => MotorcycleCollectionName,
            var name when nameof(OrderCollectionName).Contains(name, StringComparison.CurrentCulture) => OrderCollectionName,
            var name when nameof(MotorcycleRentCollectionName).Contains(name, StringComparison.CurrentCulture) => MotorcycleRentCollectionName,
            _ => throw new InvalidOperationException($"Object {entityName} do not have a matching collection")
        };
    }
}
