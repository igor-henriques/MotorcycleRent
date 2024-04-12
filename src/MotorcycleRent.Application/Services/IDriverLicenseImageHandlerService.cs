namespace MotorcycleRent.Application.Services;

/// <summary>
/// Handles the uploading of driver's license images to storaging service.
/// </summary>
public interface IDriverLicenseImageHandlerService
{
    /// <summary>
    /// Asynchronously uploads a driver's license image to blob storage and returns the URL of the uploaded image.
    /// </summary>
    /// <param name="driverLicenseDto">Data transfer object containing the driver's license details and image file.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the request.</param>
    /// <returns>The URI of the uploaded driver's license image.</returns>
    /// <exception cref="BlobException">Thrown when the upload fails.</exception>
    Task<string> UploadImageAsync(DriverLicenseDto driverLicenseDto, CancellationToken cancellationToken = default);
}
