namespace MotorcycleRent.NotificationConsumer.Models;

internal abstract record BaseEntity
{
    [BsonId]
    public Guid Id { get; init; }    
}
