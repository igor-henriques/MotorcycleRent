namespace MotorcycleRent.Infrastructure.Services;

public class BaseServiceBusPublisher<TMessage> : IPublisher<TMessage> where TMessage : class
{
    private readonly string _connectionString;
    private readonly string _queueName;
    private readonly ILogger<TMessage> _logger;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusSender _sender;    

    public BaseServiceBusPublisher(IOptions<ServiceBusOptions> options, ILogger<TMessage> logger)
    {
        _connectionString = options.Value.ConnectionString!;
        _queueName = options.Value.QueueName!;
        _client = new ServiceBusClient(_connectionString);
        _sender = _client.CreateSender(_queueName);
        _logger = logger;
    }

    public async Task PublishMessageAsync(TMessage message, CancellationToken cancellationToken = default)
    {
        string messageAsJson = message.AsJson();

        try
        {
            var serviceBusMessage = new ServiceBusMessage(messageAsJson);
            await _sender.SendMessageAsync(serviceBusMessage, cancellationToken);

            _logger.LogInformation("New {MessageType} published to the topic processor", 
                typeof(TMessage).Name);
        }
        catch (Exception)
        {
            _logger.LogError("Failed to publish message type {MessageType}: {MessageContent}", 
                typeof(TMessage).Name, 
                messageAsJson);

            throw;
        }
    }
}
