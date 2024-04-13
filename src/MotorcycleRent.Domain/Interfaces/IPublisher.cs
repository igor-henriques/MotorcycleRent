namespace MotorcycleRent.Domain.Interfaces;

public interface IPublisher<TMessage>
{
    Task PublishMessageAsync(TMessage message, CancellationToken cancellationToken = default);
}
