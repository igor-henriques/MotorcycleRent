namespace MotorcycleRent.UnitTests.Application.Services;

public sealed class RentServiceOrchestratorTests
{
    private readonly Mock<IBaseRepository<DeliveryPartner>> _userRepositoryMock = new();
    private readonly Mock<IBaseRepository<MotorcycleRental>> _rentRepositoryMock = new();
    private readonly Mock<IEmailClaimProvider> _emailClaimProviderMock = new();
    private readonly Mock<ILogger<RentalServiceOrchestrator>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IMotorcycleServiceOrchestrator> _motorcycleServiceOrchestratorMock = new();
    private readonly Mock<IRentalCostCalculatorService> _weeklyRentCostCalculatorServiceMock = new();

    private readonly RentalServiceOrchestrator _orchestrator;

    public RentServiceOrchestratorTests()
    {
        _orchestrator = new RentalServiceOrchestrator(
            _userRepositoryMock.Object,
            _rentRepositoryMock.Object,
            _loggerMock.Object,
            _emailClaimProviderMock.Object,
            [_weeklyRentCostCalculatorServiceMock.Object],
            _mapperMock.Object,
            _motorcycleServiceOrchestratorMock.Object);
    }

    [Fact]
    public async Task RentMotorcycleAsync_ThrowsOnGoingRentException_WhenRentIsActive()
    {
        // Arrange
        var MotorcycleRentalDto = new MotorcycleRentalDto();
        var deliveryPartner = new DeliveryPartner { Email = "test@example.com", DriverLicense = new DriverLicense() { DriverLicenseType = EDriverLicenseType.A } };

        _emailClaimProviderMock.Setup(x => x.GetUserEmail()).Returns("test@example.com");

        _userRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<DeliveryPartner, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(deliveryPartner);

        _rentRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<MotorcycleRental, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new MotorcycleRental { Status = ERentStatus.Ongoing });

        // Act & Assert
        await Assert.ThrowsAsync<OnGoingRentalException>(() => _orchestrator.RentMotorcycleAsync(MotorcycleRentalDto));
    }

    [Fact]
    public async Task RentMotorcycleAsync_ThrowsPartnerUnableToRentException_WhenInvalidDriverLicense()
    {
        // Arrange
        var MotorcycleRentalDto = new MotorcycleRentalDto();
        var deliveryPartner = new DeliveryPartner { Email = "test@example.com", DriverLicense = new DriverLicense() { DriverLicenseType = EDriverLicenseType.Invalid } };

        _emailClaimProviderMock.Setup(x => x.GetUserEmail()).Returns("test@example.com");

        _userRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<DeliveryPartner, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(deliveryPartner);

        _rentRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<MotorcycleRental, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new MotorcycleRental { Status = ERentStatus.Ongoing });

        // Act & Assert
        await Assert.ThrowsAsync<PartnerUnableToRentException>(() => _orchestrator.RentMotorcycleAsync(MotorcycleRentalDto));
    }

    [Fact]
    public async Task RentMotorcycleAsync_ThrowsInternalErrorException_WhenPartnerNotFound()
    {
        // Arrange
        var MotorcycleRentalDto = new MotorcycleRentalDto();
        var deliveryPartner = new DeliveryPartner { Email = "test@example.com", DriverLicense = new DriverLicense() { DriverLicenseType = EDriverLicenseType.Invalid } };

        _emailClaimProviderMock.Setup(x => x.GetUserEmail()).Returns("test@example.com");

        _userRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<DeliveryPartner, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((DeliveryPartner?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InternalErrorException>(() => _orchestrator.RentMotorcycleAsync(MotorcycleRentalDto));
    }

    [Fact]
    public async Task RentMotorcycleAsync_ThrowsNoMotorcyclesAvailableException_WhenNoMotorcyclesAvailable()
    {
        // Arrange
        var MotorcycleRentalDto = new MotorcycleRentalDto();
        var deliveryPartner = new DeliveryPartner { Email = "test@example.com", DriverLicense = new DriverLicense() { DriverLicenseType = EDriverLicenseType.A } };

        _emailClaimProviderMock.Setup(x => x.GetUserEmail()).Returns("test@example.com");

        _userRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<DeliveryPartner, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(deliveryPartner);

        _rentRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<MotorcycleRental, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((MotorcycleRental?)null);

        _motorcycleServiceOrchestratorMock.Setup(m => m.ListMotorcyclesAsync(It.IsAny<MotorcycleSearchCriteria>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);

        // Act & Assert
        await Assert.ThrowsAsync<NoMotorcyclesAvailableException>(() => _orchestrator.RentMotorcycleAsync(MotorcycleRentalDto));
    }

    [Fact]
    public async Task RentMotorcycleAsync_CreatesRent_WhenNoActiveRents()
    {
        // Arrange
        var MotorcycleRentalDto = new MotorcycleRentalDto { RentalPeriod = new DateTimeRange(DateTime.UtcNow, DateTime.UtcNow.AddDays(7)) };
        var deliveryPartner = new DeliveryPartner { Email = "test@example.com", DriverLicense = new DriverLicense() { DriverLicenseType = EDriverLicenseType.A } };
        var motorcycle = new Motorcycle { Id = Guid.NewGuid(), Plate = "123ABC" };
        MotorcycleRental expectedRent = new()
        {
            FeeCost = 0,
            RentalCost = 100
        };

        _emailClaimProviderMock.Setup(x => x.GetUserEmail()).Returns("test@example.com");

        _userRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<DeliveryPartner, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(deliveryPartner);

        _rentRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<MotorcycleRental, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((MotorcycleRental?)null);  // No ongoing rent

        _motorcycleServiceOrchestratorMock.Setup(m => m.ListMotorcyclesAsync(It.IsAny<MotorcycleSearchCriteria>(), It.IsAny<CancellationToken>()))
                                          .ReturnsAsync(new List<MotorcycleDto> { new() { Id = motorcycle.Id, Plate = motorcycle.Plate } });

        _mapperMock.Setup(m => m.Map<Motorcycle>(It.IsAny<MotorcycleDto>())).Returns(motorcycle);
        _mapperMock.Setup(m => m.Map<MotorcycleRental>(It.IsAny<MotorcycleRentalDto>())).Returns(new MotorcycleRental());

        _rentRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<MotorcycleRental>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(expectedRent with { Id = new Guid()});

        _weeklyRentCostCalculatorServiceMock.Setup(w => w.CanCalculate(It.IsAny<ERentalPlan>())).Returns(true);
        _weeklyRentCostCalculatorServiceMock.Setup(w => w.CalculateRentalCost(It.IsAny<MotorcycleRental>())).Returns(expectedRent);

        // Act
        var result = await _orchestrator.RentMotorcycleAsync(MotorcycleRentalDto);

        // Assert        
        Assert.NotNull(result);
        Assert.Equal(expectedRent.FeeCost, result.FeeCost);
        Assert.Equal(expectedRent.RentalCost, result.RentalBaseCost);
        _rentRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<MotorcycleRental>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RentMotorcycleAsync_ThrowsEntityCreationException_WhenDatabaseAddingFails()
    {
        // Arrange
        var MotorcycleRentalDto = new MotorcycleRentalDto { RentalPeriod = new DateTimeRange(DateTime.Now, DateTime.Now.AddDays(7)) };
        var deliveryPartner = new DeliveryPartner { Email = "test@example.com", DriverLicense = new DriverLicense() { DriverLicenseType = EDriverLicenseType.A } };
        var motorcycle = new Motorcycle { Id = Guid.NewGuid(), Plate = "123ABC" };
        MotorcycleRental expectedRent = new()
        {
            FeeCost = 0,
            RentalCost = 100
        };

        _emailClaimProviderMock.Setup(x => x.GetUserEmail()).Returns("test@example.com");

        _userRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<DeliveryPartner, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(deliveryPartner);

        _rentRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<MotorcycleRental, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((MotorcycleRental?)null);  // No ongoing rent

        _motorcycleServiceOrchestratorMock.Setup(m => m.ListMotorcyclesAsync(It.IsAny<MotorcycleSearchCriteria>(), It.IsAny<CancellationToken>()))
                                          .ReturnsAsync(new List<MotorcycleDto> { new() { Id = motorcycle.Id, Plate = motorcycle.Plate } });

        _mapperMock.Setup(m => m.Map<Motorcycle>(It.IsAny<MotorcycleDto>())).Returns(motorcycle);
        _mapperMock.Setup(m => m.Map<MotorcycleRental>(It.IsAny<MotorcycleRentalDto>())).Returns(new MotorcycleRental());

        _rentRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<MotorcycleRental>(), It.IsAny<CancellationToken>()))
                           .Returns(Task.FromResult((MotorcycleRental?)null));

        _weeklyRentCostCalculatorServiceMock.Setup(w => w.CanCalculate(It.IsAny<ERentalPlan>())).Returns(true);
        _weeklyRentCostCalculatorServiceMock.Setup(w => w.CalculateRentalCost(It.IsAny<MotorcycleRental>())).Returns(expectedRent);

        // Act
        await Assert.ThrowsAsync<EntityCreationException>(() => _orchestrator.RentMotorcycleAsync(MotorcycleRentalDto));
        _rentRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<MotorcycleRental>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RentMotorcycleAsync_ThrowsInternalErrorException_WhenNoCalculatorServiceAvailable()
    {
        // Arrange
        var MotorcycleRentalDto = new MotorcycleRentalDto { RentalPeriod = new DateTimeRange(DateTime.Now, DateTime.Now.AddDays(7)) };
        var deliveryPartner = new DeliveryPartner { Email = "test@example.com", DriverLicense = new DriverLicense() { DriverLicenseType = EDriverLicenseType.A } };
        var motorcycle = new Motorcycle { Id = Guid.NewGuid(), Plate = "123ABC" };
        MotorcycleRental expectedRent = new()
        {
            FeeCost = 0,
            RentalCost = 100
        };

        _emailClaimProviderMock.Setup(x => x.GetUserEmail()).Returns("test@example.com");

        _userRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<DeliveryPartner, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(deliveryPartner);

        _rentRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<MotorcycleRental, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((MotorcycleRental?)null);  // No ongoing rent

        _motorcycleServiceOrchestratorMock.Setup(m => m.ListMotorcyclesAsync(It.IsAny<MotorcycleSearchCriteria>(), It.IsAny<CancellationToken>()))
                                          .ReturnsAsync(new List<MotorcycleDto> { new() { Id = motorcycle.Id, Plate = motorcycle.Plate } });

        _mapperMock.Setup(m => m.Map<Motorcycle>(It.IsAny<MotorcycleDto>())).Returns(motorcycle);
        _mapperMock.Setup(m => m.Map<MotorcycleRental>(It.IsAny<MotorcycleRentalDto>())).Returns(new MotorcycleRental());

        _rentRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<MotorcycleRental>(), It.IsAny<CancellationToken>()))
                           .Returns(Task.FromResult((MotorcycleRental?)null));        

        var orchestrator = new RentalServiceOrchestrator(
            _userRepositoryMock.Object,
            _rentRepositoryMock.Object,
            _loggerMock.Object,
            _emailClaimProviderMock.Object,
            [],
            _mapperMock.Object,
            _motorcycleServiceOrchestratorMock.Object);

        // Act
        await Assert.ThrowsAsync<InternalErrorException>(() => orchestrator.RentMotorcycleAsync(MotorcycleRentalDto));
        _rentRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<MotorcycleRental>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ReturnMotorcycleRentalAsync_FinalizesRent_WhenRentIsOngoing()
    {
        // Arrange
        var deliveryPartner = new DeliveryPartner { Email = "test@example.com", DriverLicense = new DriverLicense() { DriverLicenseType = EDriverLicenseType.A } };
        var ongoingRent = new MotorcycleRental { Status = ERentStatus.Ongoing, Motorcycle = new Motorcycle { Plate = "123ABC" } };

        _emailClaimProviderMock.Setup(x => x.GetUserEmail()).Returns("test@example.com");
        _userRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<DeliveryPartner, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(deliveryPartner);
        _rentRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<MotorcycleRental, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(ongoingRent);

        // Act
        await _orchestrator.ReturnMotorcycleRentalAsync();

        // Assert
        _rentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<MotorcycleRental>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
