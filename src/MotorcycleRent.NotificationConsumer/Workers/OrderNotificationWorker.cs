namespace MotorcycleRent.NotificationConsumer.Workers;

internal sealed class OrderNotificationWorker : BackgroundService
{
    private readonly IBaseRepository<Order> _orderRepository;
    private readonly IBaseRepository<DeliveryPartner> _partnerRepository;
    private readonly ILogger<OrderNotificationWorker> _logger;
    private readonly ConsumerOptions _options;

    private CancellationTokenSource? _consumerCts;
    private ServiceBusClient? _client;
    private ServiceBusProcessor? _processor;

    public OrderNotificationWorker(ILogger<OrderNotificationWorker> logger,
                                   IBaseRepository<Order> orderRepository,
                                   IOptions<ConsumerOptions> options,
                                   IBaseRepository<DeliveryPartner> partnerRepository)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _options = options.Value;
        _client = new ServiceBusClient(_options.ConnectionString);
        _processor = _client.CreateProcessor(_options.QueueName, new ServiceBusProcessorOptions()
        {
            ReceiveMode = ServiceBusReceiveMode.PeekLock
        });
        _partnerRepository = partnerRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _consumerCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

            _processor!.ProcessMessageAsync += MessageHandler;
            _processor!.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync(stoppingToken);

            _logger.LogInformation("Consumer is now running. Press <ctrl + c> to exit.");
            await FreezeThread();
        }
        catch (TaskCanceledException) { }

        _logger.LogInformation("Finishing consumer.");
    }

    private async Task FreezeThread()
    {
        Console.CancelKeyPress += (sender, args) =>
        {
            args.Cancel = true;
            _consumerCts!.Cancel();
        };

        while (!_consumerCts!.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }

    private Task ErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError("Error occurred. ErrorSource: {ErrorSource}, Exception: {Exception}", args.ErrorSource, args.Exception);
        return Task.CompletedTask;
    }

    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();

        try
        {
            await HandlePartnerNotifications(args, body);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while notifying partners about an order: {RawOrder}. Exception: {exception}",
                body,
                ex);

            return;
        }

        await args.CompleteMessageAsync(args.Message);
    }

    private async Task HandlePartnerNotifications(ProcessMessageEventArgs args, string body)
    {
        var availableOrder = JsonSerializer.Deserialize<Order>(body);
        if (availableOrder is null)
        {
            _logger.LogError("An available order failed to be deserialized: {OrderBody}", body);
            await args.CompleteMessageAsync(args.Message);
            return;
        }

        _logger.LogInformation("Order received: {Order}", availableOrder);

        var order = await _orderRepository.GetByAsync(o => o.PublicOrderId == availableOrder!.PublicOrderId, _consumerCts!.Token);
        if (order is null || !order.CanPartnersBeNotified)
        {
            _logger.LogError("Order ID {PublicOrderId} is not eligible for notifications", availableOrder.PublicOrderId);
            await args.CompleteMessageAsync(args.Message);
            return;
        }

        var availablePartners = await _partnerRepository.GetAllByAsync(p => p.HasActiveRental & p.IsAvailable, _consumerCts.Token);
        var partnerNotificationUpdateTasks = new List<Task>();

        foreach (var partner in availablePartners)
        {
            partnerNotificationUpdateTasks.Add(NotifyPartner(partner, order));
        }

        await Task.WhenAll(partnerNotificationUpdateTasks);
        await UpdateOrderNotifications(order);
    }

    private async Task NotifyPartner(DeliveryPartner partner, Order order)
    {
        try
        {
            partner.Notifications.Add(order);

            var filterBuilder = Builders<DeliveryPartner>.Filter.Eq(d => d.Email, partner.Email);
            var updateBuilder = Builders<DeliveryPartner>.Update.Set(d => d.Notifications, partner.Notifications);

            bool updateResult = await _partnerRepository.UpdateAsync(filterBuilder, updateBuilder, _consumerCts!.Token);
            if (!updateResult)
            {
                _logger.LogError("An error occurred when notifying partner {PartnerEmail} about an order ID {PublicOrderId}",
                    partner.Email,
                    order.PublicOrderId);

                return;
            }

            _logger.LogInformation("Successfully notified partner {PartnerEmail} about order {PublicOrderId}",
                partner.Email,
                order.PublicOrderId);

            order.NotifiedPartnersEmails.Add(partner.Email!);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred when notifying partner {PartnerEmail} about order ID {OrderPublicId}. Exception: {exception}",
                partner.Email,
                order.PublicOrderId,
                ex);
        }
    }

    private async Task UpdateOrderNotifications(Order order)
    {
        var filterBuilder = Builders<Order>.Filter.Eq(d => d.PublicOrderId, order.PublicOrderId);
        var updateBuilder = Builders<Order>.Update.Set(d => d.NotifiedPartnersEmails, order.NotifiedPartnersEmails);

        bool updateResult = await _orderRepository.UpdateAsync(filterBuilder, updateBuilder, _consumerCts!.Token);
        if (!updateResult)
        {
            _logger.LogError("An error occurred when updating an order ID {PublicOrderId}",
               order.PublicOrderId);
        }

        _logger.LogInformation("Successfully notified {PartnerCount} partners about order {PublicOrderId}",
            order.NotifiedPartnersEmails.Count,
            order.PublicOrderId);
    }

    public async ValueTask DisposeAsync()
    {
        if (_processor != null)
        {
            await _processor.DisposeAsync();
            _processor = null;
        }

        if (_client != null)
        {
            await _client.DisposeAsync();
            _client = null;
        }
    }
}