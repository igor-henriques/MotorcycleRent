namespace MotorcycleRent.UnitTests.Application.Services;

public sealed class RentServiceOrchestratorTests
{
    private readonly Mock<IBaseRepository<DeliveryPartner>> _userRepositoryMock = new();
    private readonly Mock<IBaseRepository<MotorcycleRent.Domain.Entities.MotorcycleRent>> _rentRepositoryMock = new();
    private readonly Mock<IEmailClaimProvider> _emailClaimProviderMock = new();
    private readonly Mock<ILogger<RentServiceOrchestrator>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IMotorcycleServiceOrchestrator> _motorcycleServiceOrchestratorMock = new();
    private readonly Mock<IRentCostCalculatorService> _weeklyRentCostCalculatorServiceMock = new();

    private readonly RentServiceOrchestrator _orchestrator;

    public RentServiceOrchestratorTests()
    {
        _orchestrator = new RentServiceOrchestrator(
            _userRepositoryMock.Object,
            _rentRepositoryMock.Object,
            _loggerMock.Object,
            _emailClaimProviderMock.Object,
            [_weeklyRentCostCalculatorServiceMock.Object],
            _mapperMock.Object,
            _motorcycleServiceOrchestratorMock.Object);
    }

    [Fact]
    public async Task CreateMotorcycleRentAsync_ThrowsOnGoingRentException_WhenRentIsActive()
    {
        // Arrange
        var motorcycleRentDto = new MotorcycleRentDto();
        var deliveryPartner = new DeliveryPartner { Email = "test@example.com", DriverLicense = new DriverLicense() { DriverLicenseType = EDriverLicenseType.A } };

        _emailClaimProviderMock.Setup(x => x.GetUserEmail()).Returns("test@example.com");

        _userRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<DeliveryPartner, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(deliveryPartner);

        _rentRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<MotorcycleRent.Domain.Entities.MotorcycleRent, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new MotorcycleRent.Domain.Entities.MotorcycleRent { RentStatus = ERentStatus.Ongoing });

        // Act & Assert
        await Assert.ThrowsAsync<OnGoingRentException>(() => _orchestrator.CreateMotorcycleRentAsync(motorcycleRentDto));
    }

    [Fact]
    public async Task CreateMotorcycleRentAsync_ThrowsPartnerUnableToRentException_WhenInvalidDriverLicense()
    {
        // Arrange
        var motorcycleRentDto = new MotorcycleRentDto();
        var deliveryPartner = new DeliveryPartner { Email = "test@example.com", DriverLicense = new DriverLicense() { DriverLicenseType = EDriverLicenseType.Invalid } };

        _emailClaimProviderMock.Setup(x => x.GetUserEmail()).Returns("test@example.com");

        _userRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<DeliveryPartner, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(deliveryPartner);

        _rentRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<MotorcycleRent.Domain.Entities.MotorcycleRent, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(new MotorcycleRent.Domain.Entities.MotorcycleRent { RentStatus = ERentStatus.Ongoing });

        // Act & Assert
        await Assert.ThrowsAsync<PartnerUnableToRentException>(() => _orchestrator.CreateMotorcycleRentAsync(motorcycleRentDto));
    }

    [Fact]
    public async Task CreateMotorcycleRentAsync_ThrowsInternalErrorException_WhenPartnerNotFound()
    {
        // Arrange
        var motorcycleRentDto = new MotorcycleRentDto();
        var deliveryPartner = new DeliveryPartner { Email = "test@example.com", DriverLicense = new DriverLicense() { DriverLicenseType = EDriverLicenseType.Invalid } };

        _emailClaimProviderMock.Setup(x => x.GetUserEmail()).Returns("test@example.com");

        _userRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<DeliveryPartner, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((DeliveryPartner?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InternalErrorException>(() => _orchestrator.CreateMotorcycleRentAsync(motorcycleRentDto));
    }

    [Fact]
    public async Task CreateMotorcycleRentAsync_ThrowsNoMotorcyclesAvailableException_WhenNoMotorcyclesAvailable()
    {
        // Arrange
        var motorcycleRentDto = new MotorcycleRentDto();
        var deliveryPartner = new DeliveryPartner { Email = "test@example.com", DriverLicense = new DriverLicense() { DriverLicenseType = EDriverLicenseType.A } };

        _emailClaimProviderMock.Setup(x => x.GetUserEmail()).Returns("test@example.com");

        _userRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<DeliveryPartner, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(deliveryPartner);

        _rentRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<MotorcycleRent.Domain.Entities.MotorcycleRent, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((MotorcycleRent.Domain.Entities.MotorcycleRent?)null);

        _motorcycleServiceOrchestratorMock.Setup(m => m.ListMotorcyclesAsync(It.IsAny<MotorcycleSearchCriteria>(), It.IsAny<CancellationToken>())).ReturnsAsync([]);

        // Act & Assert
        await Assert.ThrowsAsync<NoMotorcyclesAvailableException>(() => _orchestrator.CreateMotorcycleRentAsync(motorcycleRentDto));
    }

    [Fact]
    public async Task CreateMotorcycleRentAsync_CreatesRent_WhenNoActiveRents()
    {
        // Arrange
        var motorcycleRentDto = new MotorcycleRentDto { RentPeriod = new DateTimeRange(DateTime.Now, DateTime.Now.AddDays(7)) };
        var deliveryPartner = new DeliveryPartner { Email = "test@example.com", DriverLicense = new DriverLicense() { DriverLicenseType = EDriverLicenseType.A } };
        var motorcycle = new Motorcycle { Id = Guid.NewGuid(), Plate = "123ABC" };
        MotorcycleRent.Domain.Entities.MotorcycleRent expectedRent = new MotorcycleRent.Domain.Entities.MotorcycleRent()
        {
            FeeCost = 0,
            RentCost = 100
        };

        _emailClaimProviderMock.Setup(x => x.GetUserEmail()).Returns("test@example.com");

        _userRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<DeliveryPartner, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(deliveryPartner);

        _rentRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<MotorcycleRent.Domain.Entities.MotorcycleRent, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((MotorcycleRent.Domain.Entities.MotorcycleRent?)null);  // No ongoing rent

        _motorcycleServiceOrchestratorMock.Setup(m => m.ListMotorcyclesAsync(It.IsAny<MotorcycleSearchCriteria>(), It.IsAny<CancellationToken>()))
                                          .ReturnsAsync(new List<MotorcycleDto> { new MotorcycleDto { Id = motorcycle.Id, Plate = motorcycle.Plate } });

        _mapperMock.Setup(m => m.Map<Motorcycle>(It.IsAny<MotorcycleDto>())).Returns(motorcycle);
        _mapperMock.Setup(m => m.Map<MotorcycleRent.Domain.Entities.MotorcycleRent>(It.IsAny<MotorcycleRentDto>())).Returns(new MotorcycleRent.Domain.Entities.MotorcycleRent());

        _rentRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<MotorcycleRent.Domain.Entities.MotorcycleRent>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(expectedRent with { Id = new Guid()});

        _weeklyRentCostCalculatorServiceMock.Setup(w => w.CanCalculate(It.IsAny<ERentPlan>())).Returns(true);
        _weeklyRentCostCalculatorServiceMock.Setup(w => w.CalculateRentCost(It.IsAny<MotorcycleRent.Domain.Entities.MotorcycleRent>())).Returns(expectedRent);

        // Act
        var result = await _orchestrator.CreateMotorcycleRentAsync(motorcycleRentDto);

        // Assert        
        Assert.NotNull(result);
        Assert.Equal(expectedRent.FeeCost, result.FeeCost);
        Assert.Equal(expectedRent.RentCost, result.RentCost);
        _rentRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<MotorcycleRent.Domain.Entities.MotorcycleRent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateMotorcycleRentAsync_ThrowsEntityCreationException_WhenDatabaseAddingFails()
    {
        // Arrange
        var motorcycleRentDto = new MotorcycleRentDto { RentPeriod = new DateTimeRange(DateTime.Now, DateTime.Now.AddDays(7)) };
        var deliveryPartner = new DeliveryPartner { Email = "test@example.com", DriverLicense = new DriverLicense() { DriverLicenseType = EDriverLicenseType.A } };
        var motorcycle = new Motorcycle { Id = Guid.NewGuid(), Plate = "123ABC" };
        MotorcycleRent.Domain.Entities.MotorcycleRent expectedRent = new MotorcycleRent.Domain.Entities.MotorcycleRent()
        {
            FeeCost = 0,
            RentCost = 100
        };

        _emailClaimProviderMock.Setup(x => x.GetUserEmail()).Returns("test@example.com");

        _userRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<DeliveryPartner, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(deliveryPartner);

        _rentRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<MotorcycleRent.Domain.Entities.MotorcycleRent, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((MotorcycleRent.Domain.Entities.MotorcycleRent?)null);  // No ongoing rent

        _motorcycleServiceOrchestratorMock.Setup(m => m.ListMotorcyclesAsync(It.IsAny<MotorcycleSearchCriteria>(), It.IsAny<CancellationToken>()))
                                          .ReturnsAsync(new List<MotorcycleDto> { new MotorcycleDto { Id = motorcycle.Id, Plate = motorcycle.Plate } });

        _mapperMock.Setup(m => m.Map<Motorcycle>(It.IsAny<MotorcycleDto>())).Returns(motorcycle);
        _mapperMock.Setup(m => m.Map<MotorcycleRent.Domain.Entities.MotorcycleRent>(It.IsAny<MotorcycleRentDto>())).Returns(new MotorcycleRent.Domain.Entities.MotorcycleRent());

        _rentRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<MotorcycleRent.Domain.Entities.MotorcycleRent>(), It.IsAny<CancellationToken>()))
                           .Returns(Task.FromResult((MotorcycleRent.Domain.Entities.MotorcycleRent?)null));

        _weeklyRentCostCalculatorServiceMock.Setup(w => w.CanCalculate(It.IsAny<ERentPlan>())).Returns(true);
        _weeklyRentCostCalculatorServiceMock.Setup(w => w.CalculateRentCost(It.IsAny<MotorcycleRent.Domain.Entities.MotorcycleRent>())).Returns(expectedRent);

        // Act
        await Assert.ThrowsAsync<EntityCreationException>(() => _orchestrator.CreateMotorcycleRentAsync(motorcycleRentDto));
        _rentRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<MotorcycleRent.Domain.Entities.MotorcycleRent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateMotorcycleRentAsync_ThrowsInternalErrorException_WhenNoCalculatorServiceAvailable()
    {
        // Arrange
        var motorcycleRentDto = new MotorcycleRentDto { RentPeriod = new DateTimeRange(DateTime.Now, DateTime.Now.AddDays(7)) };
        var deliveryPartner = new DeliveryPartner { Email = "test@example.com", DriverLicense = new DriverLicense() { DriverLicenseType = EDriverLicenseType.A } };
        var motorcycle = new Motorcycle { Id = Guid.NewGuid(), Plate = "123ABC" };
        MotorcycleRent.Domain.Entities.MotorcycleRent expectedRent = new MotorcycleRent.Domain.Entities.MotorcycleRent()
        {
            FeeCost = 0,
            RentCost = 100
        };

        _emailClaimProviderMock.Setup(x => x.GetUserEmail()).Returns("test@example.com");

        _userRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<DeliveryPartner, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(deliveryPartner);

        _rentRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<MotorcycleRent.Domain.Entities.MotorcycleRent, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((MotorcycleRent.Domain.Entities.MotorcycleRent?)null);  // No ongoing rent

        _motorcycleServiceOrchestratorMock.Setup(m => m.ListMotorcyclesAsync(It.IsAny<MotorcycleSearchCriteria>(), It.IsAny<CancellationToken>()))
                                          .ReturnsAsync(new List<MotorcycleDto> { new MotorcycleDto { Id = motorcycle.Id, Plate = motorcycle.Plate } });

        _mapperMock.Setup(m => m.Map<Motorcycle>(It.IsAny<MotorcycleDto>())).Returns(motorcycle);
        _mapperMock.Setup(m => m.Map<MotorcycleRent.Domain.Entities.MotorcycleRent>(It.IsAny<MotorcycleRentDto>())).Returns(new MotorcycleRent.Domain.Entities.MotorcycleRent());

        _rentRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<MotorcycleRent.Domain.Entities.MotorcycleRent>(), It.IsAny<CancellationToken>()))
                           .Returns(Task.FromResult((MotorcycleRent.Domain.Entities.MotorcycleRent?)null));        

        var orchestrator = new RentServiceOrchestrator(
            _userRepositoryMock.Object,
            _rentRepositoryMock.Object,
            _loggerMock.Object,
            _emailClaimProviderMock.Object,
            [],
            _mapperMock.Object,
            _motorcycleServiceOrchestratorMock.Object);

        // Act
        await Assert.ThrowsAsync<InternalErrorException>(() => orchestrator.CreateMotorcycleRentAsync(motorcycleRentDto));
        _rentRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<MotorcycleRent.Domain.Entities.MotorcycleRent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ReturnMotorcycleRentAsync_FinalizesRent_WhenRentIsOngoing()
    {
        // Arrange
        var deliveryPartner = new DeliveryPartner { Email = "test@example.com", DriverLicense = new DriverLicense() { DriverLicenseType = EDriverLicenseType.A } };
        var ongoingRent = new MotorcycleRent.Domain.Entities.MotorcycleRent { RentStatus = ERentStatus.Ongoing, Motorcycle = new Motorcycle { Plate = "123ABC" } };

        _emailClaimProviderMock.Setup(x => x.GetUserEmail()).Returns("test@example.com");
        _userRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<DeliveryPartner, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(deliveryPartner);
        _rentRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<MotorcycleRent.Domain.Entities.MotorcycleRent, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(ongoingRent);

        // Act
        await _orchestrator.ReturnMotorcycleRentAsync();

        // Assert
        _rentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<MotorcycleRent.Domain.Entities.MotorcycleRent>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
