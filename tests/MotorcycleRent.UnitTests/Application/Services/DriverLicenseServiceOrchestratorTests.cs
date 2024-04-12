namespace MotorcycleRent.UnitTests.Application.Services;

public sealed class DriverLicenseServiceOrchestratorTests
{
    private readonly Mock<IBaseRepository<DeliveryPartner>> _userRepositoryMock = new();
    private readonly Mock<IDriverLicenseImageHandlerService> _driverLicenseImageHandlerServiceMock = new();
    private readonly Mock<ILogger<DriverLicenseServiceOrchestrator>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IEmailClaimProvider> _emailClaimProviderMock = new();
    private readonly DriverLicenseServiceOrchestrator _orchestrator;

    public DriverLicenseServiceOrchestratorTests()
    {
        _orchestrator = new DriverLicenseServiceOrchestrator(
            _userRepositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object,
            _driverLicenseImageHandlerServiceMock.Object,
            _emailClaimProviderMock.Object);
    }

    [Fact]
    public async Task CreateDriverLicense_ThrowsInvalidOperationException_WhenDeliveryPartnerDoesNotExist()
    {
        // Arrange
        var driverLicenseDto = new DriverLicenseDto { DriverLicenseId = "12345" };
        _emailClaimProviderMock.Setup(x => x.GetUserEmail()).Returns("test@example.com");
        _userRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<DeliveryPartner, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((DeliveryPartner?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _orchestrator.CreateDriverLicense(driverLicenseDto));
    }

    [Fact]
    public async Task UpdateDriverLicense_ThrowsInvalidOperationException_WhenLicenseDoesNotExist()
    {
        // Arrange
        var driverLicenseDto = new DriverLicenseDto { DriverLicenseId = "12345" };
        _emailClaimProviderMock.Setup(x => x.GetUserEmail()).Returns("test@example.com");
        _userRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<DeliveryPartner, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync((DeliveryPartner?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _orchestrator.UpdateDriverLicense(driverLicenseDto));
    }

    [Fact]
    public async Task CreateDriverLicense_CreatesLicense_IfNotExist()
    {
        // Arrange
        var driverLicenseDto = new DriverLicenseDto { DriverLicenseId = "12345", DriverLicenseImage = new FormFile(Stream.Null, 0, 0, "Test", "Test.png") };

        var deliveryPartner = new DeliveryPartner { Email = "test@example.com" };

        _emailClaimProviderMock.Setup(x => x.GetUserEmail()).Returns(deliveryPartner.Email);

        _userRepositoryMock.Setup(x => x.GetByAsync(It.IsAny<Expression<Func<DeliveryPartner, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(deliveryPartner);

        _userRepositoryMock.Setup(x => x.GetByAsync(u => u.DriverLicense != null && u.DriverLicense.DriverLicenseId == driverLicenseDto.DriverLicenseId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((DeliveryPartner?)null);  // No existing license

        _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<DeliveryPartner>(), It.IsAny<CancellationToken>())).ReturnsAsync(new DeliveryPartner());

        _driverLicenseImageHandlerServiceMock.Setup(x => x.UploadImageAsync(It.IsAny<DriverLicenseDto>(), It.IsAny<CancellationToken>()))
                                             .ReturnsAsync("http://example.com/image.jpg");

        _mapperMock.Setup(m => m.Map<DriverLicense>(It.IsAny<DriverLicenseDto>()))
                   .Returns(new DriverLicense { DriverLicenseId = driverLicenseDto.DriverLicenseId });

        // Act
        await _orchestrator.CreateDriverLicense(driverLicenseDto);

        // Assert
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<DeliveryPartner>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
