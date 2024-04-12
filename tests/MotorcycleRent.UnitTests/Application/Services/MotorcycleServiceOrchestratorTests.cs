namespace MotorcycleRent.UnitTests.Application.Services;

public sealed class MotorcycleServiceOrchestratorTests
{
    private readonly Mock<IBaseRepository<Motorcycle>> _motorcycleRepositoryMock = new();
    private readonly Mock<IBaseRepository<MotorcycleRent.Domain.Entities.MotorcycleRent>> _motorcycleRentRepositoryMock = new();
    private readonly Mock<ILogger<MotorcycleServiceOrchestrator>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly MotorcycleServiceOrchestrator _orchestrator;

    public MotorcycleServiceOrchestratorTests()
    {
        _orchestrator = new MotorcycleServiceOrchestrator(
            _motorcycleRepositoryMock.Object,
            _loggerMock.Object,
            _mapperMock.Object,
            _motorcycleRentRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateMotorcycleAsync_ShouldCreateMotorcycle_WhenValidDataProvided()
    {
        // Arrange
        var motorcycleDto = new MotorcycleDto { Plate = "ABC123" };

        var motorcycle = new Motorcycle { Plate = motorcycleDto.Plate };

        _mapperMock.Setup(m => m.Map<Motorcycle>(motorcycleDto)).Returns(motorcycle);

        _motorcycleRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Motorcycle>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);

        // Act
        var result = await _orchestrator.CreateMotorcycleAsync(motorcycleDto);

        // Assert
        Assert.Equal(motorcycle.Id, result);
    }

    [Fact]
    public async Task ListMotorcyclesAsync_ShouldReturnMotorcycles_WhenCriteriaMatch()
    {
        // Arrange
        var motorcycles = new List<Motorcycle> { new() { Plate = "ABC123" } };

        _motorcycleRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<FilterDefinition<Motorcycle>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycles);

        // Act
        var result = await _orchestrator.ListMotorcyclesAsync(new MotorcycleSearchCriteria(), CancellationToken.None);

        // Assert
        Assert.Single(result);
    }

    [Fact]
    public async Task UpdateMotorcyclePlateAsync_ShouldUpdatePlate_WhenNoActiveRentExists()
    {
        // Arrange
        var updateDto = new UpdateMotorcyclePlateDto { OldPlate = "ABC123", NewPlate = "XYZ789" };
        var motorcycle = new Motorcycle { Plate = updateDto.OldPlate, Id = Guid.NewGuid() };

        _motorcycleRepositoryMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Motorcycle, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);

        _motorcycleRentRepositoryMock.Setup(r => r.GetAllByAsync(It.IsAny<Expression<Func<MotorcycleRent.Domain.Entities.MotorcycleRent, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<MotorcycleRent.Domain.Entities.MotorcycleRent>());

        _motorcycleRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Motorcycle>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Motorcycle());

        // Act
        await _orchestrator.UpdateMotorcyclePlateAsync(updateDto, CancellationToken.None);

        // Assert
        _motorcycleRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Motorcycle>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteMotorcycleAsync_ShouldDeleteMotorcycle_WhenNoRentExists()
    {
        // Arrange
        var plate = "ABC123";

        var motorcycle = new Motorcycle { Plate = plate, Id = Guid.NewGuid() };

        _motorcycleRepositoryMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<Motorcycle, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);

        _motorcycleRentRepositoryMock.Setup(r => r.GetAllByAsync(It.IsAny<Expression<Func<MotorcycleRent.Domain.Entities.MotorcycleRent, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Enumerable.Empty<MotorcycleRent.Domain.Entities.MotorcycleRent>());

        // Act
        await _orchestrator.DeleteMotorcycleAsync(plate, CancellationToken.None);

        // Assert
        _motorcycleRepositoryMock.Verify(r => r.DeleteAsync(motorcycle.Id, CancellationToken.None), Times.Once);
    }
}