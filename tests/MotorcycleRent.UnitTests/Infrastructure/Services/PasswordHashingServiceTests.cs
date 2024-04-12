namespace MotorcycleRent.UnitTests.Infrastructure.Services;

public sealed class PasswordHashingServiceTests
{
    private readonly PasswordHashingService _service;

    public PasswordHashingServiceTests()
    {
        _service = new PasswordHashingService();
    }

    [Fact]
    public void HashPassword_ShouldReturnHashedPassword_WhenPasswordIsProvided()
    {
        // Arrange
        var password = "testPassword";

        // Act
        var hashedPassword = _service.HashPassword(password);

        // Assert
        Assert.NotNull(hashedPassword);
        Assert.NotEqual(password, hashedPassword);
        Assert.True(BCrypt.Net.BCrypt.Verify(password, hashedPassword));
    }

    [Fact]
    public void HashPassword_ShouldThrowArgumentException_WhenNullIsProvided()
    {
        // Arrange, Act and Assert
        Assert.Throws<ArgumentNullException>(() => _service.HashPassword(null));
    }

    [Fact]
    public void VerifyPassword_ShouldReturnTrue_WhenCorrectPasswordIsProvided()
    {
        // Arrange
        var password = "testPassword";
        var hashedPassword = _service.HashPassword(password);

        // Act
        var isCorrect = _service.VerifyPassword(hashedPassword, password);

        // Assert
        Assert.True(isCorrect);
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalse_WhenIncorrectPasswordIsProvided()
    {
        // Arrange
        var password = "testPassword";
        var incorrectPassword = "incorrectPassword";
        var hashedPassword = _service.HashPassword(password);

        // Act
        var isCorrect = _service.VerifyPassword(hashedPassword, incorrectPassword);

        // Assert
        Assert.False(isCorrect);
    }

    [Fact]
    public void VerifyPassword_ShouldThrowArgumentException_WhenNullHashedPasswordIsProvided()
    {
        // Arrange, Act and Assert
        Assert.Throws<ArgumentNullException>(() => _service.VerifyPassword(null, "providedPassword"));
    }

    [Fact]
    public void VerifyPassword_ShouldThrowArgumentException_WhenNullProvidedPasswordIsProvided()
    {
        // Arrange, Act and Assert
        Assert.Throws<ArgumentNullException>(() => _service.VerifyPassword("hashedPassword", null));
    }
}
