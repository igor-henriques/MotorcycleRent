namespace MotorcycleRent.UnitTests.Core.Shared;

public sealed class ArgumentValidatorTests
{
    [Fact]
    public void Should_ThrowException_When_StringIsNull()
    {
        _ = Assert.Throws<ArgumentNullException>(() => ArgumentValidator.ThrowIfNullOrEmpty(null));
    }

    [Fact]
    public void Should_ThrowException_When_StringIsEmpty()
    {
        _ = Assert.Throws<ArgumentNullException>(() => ArgumentValidator.ThrowIfNullOrEmpty(""));
    }

    [Fact]
    public void Should_ThrowException_When_ValueIsNegative()
    {
        _ = Assert.Throws<ArgumentException>(() => ArgumentValidator.ThrowIfNegative(-1));
    }

    [Fact]
    public void Should_ThrowException_When_ValueIsZeroInThrowIfZeroOrNegative()
    {
        _ = Assert.Throws<ArgumentException>(() => ArgumentValidator.ThrowIfZeroOrNegative(0));
    }

    [Fact]
    public void Should_ThrowException_When_ValueIsNegativeInThrowIfZeroOrNegative()
    {
        _ = Assert.Throws<ArgumentException>(() => ArgumentValidator.ThrowIfZeroOrNegative(-1));
    }

    [Fact]
    public void Should_ThrowException_When_ValueIsZeroInThrowIfZero()
    {
        _ = Assert.Throws<ArgumentException>(() => ArgumentValidator.ThrowIfZero(0));
    }

    [Fact]
    public void Should_ThrowException_When_ObjectIsNull()
    {
        object? obj = null;
        _ = Assert.Throws<ArgumentNullException>(() => ArgumentValidator.ThrowIfNullOrDefault(obj));
    }

    [Fact]
    public void Should_ThrowException_When_ObjectIsDefault()
    {
        int defaultValue = default;
        _ = Assert.Throws<ArgumentNullException>(() => ArgumentValidator.ThrowIfNullOrDefault(defaultValue));
    }
}
