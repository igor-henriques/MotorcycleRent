namespace MotorcycleRent.Application.Models.Options;

public sealed record MotorcycleRentalDatabaseOptions
{
    public string? ConnectionString { get; init; }
    public string? DatabaseName { get; init; }
    public string? UserCollectionName { get; init; }
    public string? MotorcycleCollectionName { get; init; }
    public string? OrderCollectionName { get; init; }
    public string? MotorcycleRentalCollectionName { get; init; }

    public string? GetCollectionName<TEntity>()
    {
        var entityName = typeof(TEntity).Name;

        return entityName switch
        {
            nameof(DeliveryPartner) => UserCollectionName,
            nameof(Administrator) => UserCollectionName,
            nameof(User) => UserCollectionName,
            nameof(Motorcycle) => MotorcycleCollectionName,
            nameof(Order) => OrderCollectionName,
            nameof(MotorcycleRental) => MotorcycleRentalCollectionName,
            _ => throw new InvalidOperationException($"Object {entityName} do not have a matching collection")
        };
    }
}
