namespace MotorcycleRent.UnitTests.Domain.Helpers;

public sealed class FriendlyIdGeneratorTests
{
    [Fact]
    public void CreateFriendlyId_WithSpecificGuid_ReturnsConsistentId()
    {
        for (int i = 0; i < 1000; i++)
        {
            var specificGuid = Guid.NewGuid();

            var expectedFriendlyId = FriendlyIdGenerator.CreateFriendlyId(specificGuid);
            var actualFriendlyId = FriendlyIdGenerator.CreateFriendlyId(specificGuid);

            Assert.Equal(expectedFriendlyId, actualFriendlyId);
            Assert.Equal(FriendlyIdGenerator.FULL_ID_LENGTH, actualFriendlyId.Length);
        }        
    }

    [Fact]
    public void CreateFriendlyId_GeneratesUniformLengthIds()
    {
        var id1 = FriendlyIdGenerator.CreateFriendlyId(Guid.NewGuid());
        var id2 = FriendlyIdGenerator.CreateFriendlyId(Guid.NewGuid());
        var id3 = FriendlyIdGenerator.CreateFriendlyId(Guid.NewGuid());

        Assert.Equal(id1.Length, id2.Length);
        Assert.Equal(id2.Length, id3.Length);
    }

    [Fact]
    public void CreateFriendlyId_GeneratesUniformLengthIds_ForSpecificGuid()
    {
        var guid = Guid.Parse("0ad9a815-4728-4fe0-aebb-67d4b1734f16");
        var id = FriendlyIdGenerator.CreateFriendlyId(guid);
        Assert.DoesNotContain("0000", id);
    }
}