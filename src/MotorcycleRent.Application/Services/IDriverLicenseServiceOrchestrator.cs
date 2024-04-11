namespace MotorcycleRent.Application.Services;

public interface IDriverLicenseServiceOrchestrator
{
    Task CreateDriverLicense(DriverLicenseDto driverLicenseDto, CancellationToken cancellationToken = default);
    Task UpdateDriverLicense(DriverLicenseDto driverLicenseDto, CancellationToken cancellationToken = default);
}