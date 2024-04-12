namespace MotorcycleRent.UnitTests.Entities;

public sealed class DeliveryPartnerTests
{
    [Fact]
    public void IsPartnerAbleToRent_ReturnsTrue_WhenDriverLicenseTypeIsA()
    {
        // Arrange
        var partner = new DeliveryPartner
        {
            DriverLicense = new DriverLicense
            {
                DriverLicenseType = EDriverLicenseType.A
            }
        };

        // Act
        bool result = partner.IsPartnerAbleToRent;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsPartnerAbleToRent_ReturnsTrue_WhenDriverLicenseTypeIsAB()
    {
        // Arrange
        var partner = new DeliveryPartner
        {
            DriverLicense = new DriverLicense
            {
                DriverLicenseType = EDriverLicenseType.AB
            }
        };

        // Act
        bool result = partner.IsPartnerAbleToRent;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsPartnerAbleToRent_ReturnsFalse_WhenDriverLicenseIsNull()
    {
        // Arrange
        var partner = new DeliveryPartner
        {
            DriverLicense = null
        };

        // Act
        bool result = partner.IsPartnerAbleToRent;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsPartnerAbleToRent_ReturnsFalse_WhenDriverLicenseTypeIsOther()
    {
        // Arrange
        var partner = new DeliveryPartner
        {
            DriverLicense = new DriverLicense
            {
                DriverLicenseType = EDriverLicenseType.Invalid
            }
        };

        // Act
        bool result = partner.IsPartnerAbleToRent;

        // Assert
        Assert.False(result);
    }
}
