namespace MotorcycleRent.Infrastructure.Services;

public sealed class DatabaseSeedService : IDatabaseSeedService
{
    private readonly SeedOptions _options;
    private readonly ILogger<DatabaseSeedService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IBaseRepository<User> _repo;

    public DatabaseSeedService(
        IOptions<SeedOptions> options,
        ILogger<DatabaseSeedService> logger,
        IServiceScopeFactory serviceScopeFactory,
        IBaseRepository<User> repo)
    {
        _options = options.Value;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _repo = repo;
    }

    public async Task SeedAsync()
    {
        try
        {
            if (!_options.IsSeedingActive)
            {
                _logger.LogInformation("Seed operation is disabled. Proceeding with startup without seeding the database.");
                return;
            }

            bool collectionHasData = await _repo.CountDocumentsAsync(new EstimatedDocumentCountOptions() { MaxTime = TimeSpan.FromMilliseconds(100) }) > 0;

            if (collectionHasData)
            {
                _logger.LogInformation("Seed operation aborted because the collection is not empty");
                return;
            }

            _logger.LogInformation("Starting database seed operation.");

            using var scope = _serviceScopeFactory.CreateScope();
            var userService = scope.ServiceProvider.GetRequiredService<IUserServiceOrchestrator>();

            var adminCreationTask = userService.CreateAdministratorAsync(_options.AdministratorSeedUser!);
            var deliveryPartnerCreationTask = userService.CreateDeliveryPartnerAsync(_options.DeliveryPartnerSeedUser!);

            await SeedMassiveDataAsync(scope);

            await Task.WhenAll(adminCreationTask, deliveryPartnerCreationTask);

            _logger.LogInformation("Database successfully seeded. Proceeding with startup.");
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while seeding the database. Proceeding with startup. Exception: {exception}", ex);
        }
    }

    private async Task SeedMassiveDataAsync(IServiceScope scope)
    {
        
    }
}
