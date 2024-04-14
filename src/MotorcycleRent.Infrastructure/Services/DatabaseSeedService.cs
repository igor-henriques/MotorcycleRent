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

            if (_options.IsMassiveSeedingActive)
            {
                await SeedMassiveDataAsync(scope);
            }            

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
        var passwordHashingService = scope.ServiceProvider.GetRequiredService<IPasswordHashingService>();
        string hashedPassword = passwordHashingService.HashPassword(_options.DeliveryPartnerSeedUser!.Password);

        var partnerRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<DeliveryPartner>>();
        var motorcycleRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<Motorcycle>>();
        var rentalRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<MotorcycleRental>>();
        var orderRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<Order>>();
        var calculatorServices = scope.ServiceProvider.GetRequiredService<IEnumerable<IRentalCostCalculatorService>>();

        using var progressBar = new ProgressBarService(_logger);

        var partners = BuildDeliveryPartnerGenerator(hashedPassword).Generate(1000);                

        var motorcycles = BuildMotorcycleGenerator().Generate(1000).DistinctBy(m => m.Plate).ToList();
        await motorcycleRepository.CreateManyAsync(motorcycles);

        progressBar.Report(.25);

        _logger.LogInformation("{MotorcycleCount}x motorcycles successfully added to the database through {ServiceName}",
            motorcycles.Count,
            nameof(DatabaseSeedService));

        var rentals = BuildRentalGenerator(motorcycles, partners, calculatorServices).Generate(1000);
        await rentalRepository.CreateManyAsync(rentals);

        progressBar.Report(.50);

        _logger.LogInformation("{MotorcycleRentalCount}x rentals successfully added to the database through {ServiceName}",
            rentals.Count,
            nameof(DatabaseSeedService));

        var orders = BuildAvailableOrderGenerator().Generate(1000);        

        foreach (var order in orders)
        {
            var partnersAbleToBeNotified = partners.Where(partner => partner.HasActiveRental && partner.IsAvailable).ToList();

            foreach (var partner in partnersAbleToBeNotified)
            {
                partner.Notifications.Add(OrderNotification.BuildFromOrder(order));
            }

            order.NotifiedPartnersEmails.AddRange(partnersAbleToBeNotified.Select(p => p.Email).ToList()!);
        }
        
        await partnerRepository.CreateManyAsync(partners);

        progressBar.Report(.75);

        _logger.LogInformation("{DeliveryPartnerCount}x delivery partners successfully added to the database through {ServiceName}",
            partners.Count,
            nameof(DatabaseSeedService));

        // Create an index with the public order id to ensure uniqueness.
        // Only creates the index case it don't exist.                                                                   
        await orderRepository.CreateIndexAsync(
            new CreateIndexModel<Order>(
                Builders<Order>.IndexKeys.Descending(d => d.PublicOrderId),
                new CreateIndexOptions { Unique = true, Name = "UniquePublicOrderId" }));

        await orderRepository.CreateManyAsync(orders);

        progressBar.Report(1);

        _logger.LogInformation("{OrderCount}x orders successfully added to the database through {ServiceName}",
            orders.Count,
            nameof(DatabaseSeedService));
    }

    private static Faker<DeliveryPartner> BuildDeliveryPartnerGenerator(string hashedPassword)
    {
        return new Faker<DeliveryPartner>()
                    .CustomInstantiator(f =>
                    {
                        var email = f.Internet.Email();

                        var driverLicense = new DriverLicense()
                        {
                            DriverLicenseId = f.Random.Replace("###########"),
                            DriverLicenseImageUrl = "http://www.google.com",
                            DriverLicenseType = f.PickRandom<EDriverLicenseType>()
                        };

                        var hasActiveRental = driverLicense.DriverLicenseType is not EDriverLicenseType.Invalid && f.Random.Bool();

                        return new DeliveryPartner
                        {
                            Id = Guid.NewGuid(),
                            Email = email,
                            FullName = f.Person.FullName,
                            HashedPassword = hashedPassword,
                            IsAvailable = f.Random.Bool(),
                            HasActiveRental = hasActiveRental,
                            BirthDate = f.Date.Past(18),
                            NationalId = f.Random.Replace("##.###.###/0001-#"),
                            DriverLicense = driverLicense,
                            Claims =
                            [
                                new Claim(ClaimTypes.Role, nameof(DeliveryPartner)),
                                new Claim(ClaimTypes.Email, email)
                            ]
                        };
                    });
    }
    private static Faker<Motorcycle> BuildMotorcycleGenerator()
    {
        return new Faker<Motorcycle>()
            .RuleFor(x => x.Plate, f => f.Random.AlphaNumeric(7).ToUpper())
            .RuleFor(x => x.Model, f => f.Vehicle.Model())
            .RuleFor(x => x.Status, f => f.PickRandom<EMotorcycleStatus>())
            .RuleFor(x => x.Year, f => f.Random.Int(min: DateTime.UtcNow.AddYears(-10).Year, max: DateTime.UtcNow.Year));
    }
    private static Faker<MotorcycleRental> BuildRentalGenerator(List<Motorcycle> motorcycles, List<DeliveryPartner> partners, IEnumerable<IRentalCostCalculatorService> calcServices)
    {
        return new Faker<MotorcycleRental>()
           .CustomInstantiator(f =>
           {
               var period = f.PickRandom(7, 14, 30);
               var planRenovations = f.Random.Int(min: 1, max: 10);

               var initialRentalPeriodDate = f.Date.Past(f.Random.Int(min: 0, max: 5));
               var rentalPeriod = new DateTimeRange(initialRentalPeriodDate, initialRentalPeriodDate.AddDays(period * planRenovations).AddDays(f.Random.Int(0, 100)));

               var partnersAbleToRent = partners.Where(p => p.HasActiveRental).ToList();
               
               var selectedPartner = partnersAbleToRent.ElementAt(f.Random.Int(min: 0, max: partnersAbleToRent.Count - 1));

               var rentedMotorcycles = motorcycles.Where(m => m.Status is EMotorcycleStatus.Rented).ToList();
               var selectedMotorcycle = rentedMotorcycles.ElementAt(f.Random.Int(0, rentedMotorcycles.Count - 1));

               var rental = new MotorcycleRental()
               {
                   Id = Guid.NewGuid(),
                   DeliveryPartner = selectedPartner,
                   RentalPeriod = rentalPeriod,
                   Status = rentalPeriod.End >= DateTime.UtcNow ? f.PickRandom<ERentStatus>() : ERentStatus.Finished,
                   RentalPlan = (ERentalPlan)period
               };

               return calcServices.FirstOrDefault(c => c.CanCalculate(rental.RentalPlan))!.CalculateRentalCost(rental);
           });            
    }
    private static Faker<Order> BuildAvailableOrderGenerator()
    {
        return new Faker<Order>()
            .CustomInstantiator(f =>
            {
                var orderId = Guid.NewGuid();
                var order =  Order.CreateNewOrder(f.Random.Int(min: 10, max: 100), EOrderStatus.Available) with 
                {
                    Id = orderId,
                    CreationDate = f.Date.Past(1, refDate: DateTime.UtcNow.AddMonths(-6)),
                    PublicOrderId = FriendlyIdGenerator.CreateFriendlyId(orderId),
                };

                return order;
            });
    }
}
