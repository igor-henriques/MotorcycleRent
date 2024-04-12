namespace MotorcycleRent.UnitTests.Application.Services;

public sealed class UserServiceOrchestratorTests
{
    private readonly Mock<IBaseRepository<User>> _userRepositoryMock = new();
    private readonly Mock<ILogger<UserServiceOrchestrator>> _loggerMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly Mock<IPasswordHashingService> _passwordHashingServiceMock = new();
    private readonly Mock<ITokenGeneratorService> _tokenGeneratorServiceMock = new();
    private readonly UserServiceOrchestrator _orchestrator;

    public UserServiceOrchestratorTests()
    {
        _orchestrator = new UserServiceOrchestrator(
            _passwordHashingServiceMock.Object,
            _userRepositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object,
            _tokenGeneratorServiceMock.Object);
    }

    [Fact]
    public async Task CreateAdministratorAsync_CreatesUserSuccessfully()
    {
        // Arrange
        var adminDto = new AdministratorDto { Email = "admin@example.com", Password = "password123" };
        var admin = new Administrator { Email = adminDto.Email };

        _mapperMock.Setup(m => m.Map<Administrator>(adminDto)).Returns(admin);
        _passwordHashingServiceMock.Setup(p => p.HashPassword(adminDto.Password)).Returns("hashedPassword");
        _userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Administrator>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(admin);

        // Act
        await _orchestrator.CreateAdministratorAsync(adminDto);

        // Assert
        _userRepositoryMock.Verify(r => r.CreateAsync(It.Is<Administrator>(u => u.Email == admin.Email && u.HashedPassword == "hashedPassword"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateDeliveryPartnerAsync_ThrowsInvalidOperationException_WhenUserExists()
    {
        // Arrange
        var partnerDto = new DeliveryPartnerDto { Email = "partner@example.com", Password = "password123" };
        var partner = new DeliveryPartner { Email = partnerDto.Email };

        _mapperMock.Setup(m => m.Map<DeliveryPartner>(partnerDto)).Returns(partner);

        _passwordHashingServiceMock.Setup(p => p.HashPassword(partnerDto.Password)).Returns("hashedPassword");

        _userRepositoryMock.Setup(r => r.CreateIndexAsync(It.IsAny<CreateIndexModel<User>>(), It.IsAny<CancellationToken>()))
                           .Returns(Task.CompletedTask);

        _userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<DeliveryPartner>(), It.IsAny<CancellationToken>()))
                           .ThrowsAsync(new InvalidOperationException());

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _orchestrator.CreateDeliveryPartnerAsync(partnerDto));
    }

    [Fact]
    public async Task AuthenticateAsync_ReturnsToken_WhenCredentialsAreValid()
    {
        // Arrange
        var userDto = new UserDto { Email = "user@example.com", Password = "password123" };
        var user = new User { Email = userDto.Email, HashedPassword = "hashedPassword", Claims = new List<Claim>() };
        var token = new JwtToken { Token = "token123", ExpiresAt = DateTime.UtcNow.AddHours(1) };

        _userRepositoryMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);

        _passwordHashingServiceMock.Setup(p => p.VerifyPassword("hashedPassword", userDto.Password)).Returns(true);

        _tokenGeneratorServiceMock.Setup(t => t.GenerateToken(It.IsAny<IEnumerable<Claim>>())).Returns(token);

        // Act
        var result = await _orchestrator.AuthenticateAsync(userDto);

        // Assert
        Assert.Equal("token123", result.Token);
        _tokenGeneratorServiceMock.Verify(t => t.GenerateToken(It.IsAny<IEnumerable<Claim>>()), Times.Once);
    }

    [Fact]
    public async Task AuthenticateAsync_ThrowsInvalidCredentialsException_WhenUserDontExist()
    {
        // Arrange
        var userDto = new UserDto { Email = "user@example.com", Password = "password123" };
        User? user = null;

        _userRepositoryMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);

        _passwordHashingServiceMock.Setup(p => p.VerifyPassword("hashedPassword", userDto.Password)).Returns(true);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidCredentialsException>(() => _orchestrator.AuthenticateAsync(userDto));
    }

    [Fact]
    public async Task AuthenticateAsync_ThrowsInvalidCredentialsException_WhenCredentialIsInvalid()
    {
        // Arrange
        var userDto = new UserDto { Email = "user@example.com", Password = "password123" };
        User user = new();

        _userRepositoryMock.Setup(r => r.GetByAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);

        _passwordHashingServiceMock.Setup(p => p.VerifyPassword("hashedPassword", userDto.Password)).Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidCredentialsException>(() => _orchestrator.AuthenticateAsync(userDto));
    }
}
