namespace MotorcycleRent.UnitTests.Domain.Helpers;

public sealed class FriendlyIdGeneratorTests
{
    [Fact]
    public void CreateFriendlyId_WithSpecificGuid_ReturnsConsistentId()
    {
        for (int i = 0; i < 100; i++)
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
}