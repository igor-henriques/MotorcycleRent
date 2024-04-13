namespace MotorcycleRent.UnitTests.Domain.Models;

public sealed class DateTimeRangeTests
{
    [Fact]
    public void Constructor_ValidatesEndDateGreaterThanStartDate_ThrowsException()
    {
        // Arrange
        var startDate = new DateTime(2021, 01, 10);
        var endDate = new DateTime(2021, 01, 05);

        // Act & Assert
        Assert.Throws<DateTimeInvalidRangeException>(() => new DateTimeRange(startDate, endDate));
    }

    [Fact]
    public void Equals_TwoIdenticalRanges_ReturnsTrue()
    {
        // Arrange
        var range1 = new DateTimeRange(new DateTime(2021, 01, 01), new DateTime(2021, 01, 31));
        var range2 = new DateTimeRange(new DateTime(2021, 01, 01), new DateTime(2021, 01, 31));

        // Act & Assert
        Assert.True(range1.Equals(range2));
    }

    [Fact]
    public void Contains_SubRange_ReturnsTrue()
    {
        // Arrange
        var range = new DateTimeRange(new DateTime(2021, 01, 01), new DateTime(2021, 01, 31));
        var subRange = new DateTimeRange(new DateTime(2021, 01, 10), new DateTime(2021, 01, 20));

        // Act & Assert
        Assert.True(range.Contains(subRange));
    }

    [Fact]
    public void Overlaps_OverlappingRanges_ReturnsTrue()
    {
        // Arrange
        var range1 = new DateTimeRange(new DateTime(2021, 01, 01), new DateTime(2021, 01, 15));
        var range2 = new DateTimeRange(new DateTime(2021, 01, 10), new DateTime(2021, 01, 20));

        // Act & Assert
        Assert.True(range1.Overlaps(range2));
    }
}
