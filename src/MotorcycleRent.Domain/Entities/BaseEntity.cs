namespace MotorcycleRent.Domain.Entities;

public abstract record BaseEntity
{
    [BsonId]
    public Guid Id { get; init; }
}
