namespace MotorcycleRent.NotificationConsumer.Models;

internal class ConsumerOptions
{
    public string? ConnectionString { get; init; }
    public string? QueueName { get; init; }
}
