namespace MotorcycleRent.Application.Services;

public interface IDriverLicenseImageHandlerService
{
    Task<string> UploadImageAsync(DriverLicenseDto driverLicenseDto, CancellationToken cancellationToken = default);
}
