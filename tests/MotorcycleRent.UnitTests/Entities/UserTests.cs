namespace MotorcycleRent.UnitTests.Entities;

public sealed class UserTests
{
    [Fact]
    public void Administrator_Should_Inherit_From_User()
    {
        // Arrange & Act
        var admin = new Administrator
        {
            Email = "admin@example.com",
            HashedPassword = "hashedpassword123"
        };

        // Assert
        Assert.NotNull(admin);
        Assert.IsType<Administrator>(admin);
        Assert.IsAssignableFrom<User>(admin);
        Assert.Equal("admin@example.com", admin.Email);
        Assert.Equal("hashedpassword123", admin.HashedPassword);
    }

    [Fact]
    public void DeliveryPartner_Should_Inherit_From_User_And_Have_Extra_Properties()
    {
        // Arrange & Act
        var deliveryPartner = new DeliveryPartner
        {
            Email = "partner@example.com",
            HashedPassword = "hashedpassword123",
            FullName = "John Doe",
            NationalId = "1234567890",
            BirthDate = new DateTime(1980, 1, 1),
            DriverLicense = new DriverLicense
            {
                DriverLicenseType = EDriverLicenseType.A
            }
        };

        // Assert
        Assert.NotNull(deliveryPartner);
        Assert.IsType<DeliveryPartner>(deliveryPartner);
        Assert.IsAssignableFrom<User>(deliveryPartner);
        Assert.Equal("partner@example.com", deliveryPartner.Email);
        Assert.Equal("hashedpassword123", deliveryPartner.HashedPassword);
        Assert.Equal("John Doe", deliveryPartner.FullName);
        Assert.Equal("1234567890", deliveryPartner.NationalId);
        Assert.Equal(new DateTime(1980, 1, 1), deliveryPartner.BirthDate);
        Assert.NotNull(deliveryPartner.DriverLicense);
        Assert.True(deliveryPartner.IsPartnerAbleToRent);
    }
}
