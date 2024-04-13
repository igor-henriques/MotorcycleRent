namespace MotorcycleRent.Infrastructure.Services;

/// <summary>
/// Handles the uploading of driver's license images to storaging service.
/// </summary>
public sealed class DriverLicenseImageHandlerService : IDriverLicenseImageHandlerService
{
    private readonly ILogger<DriverLicenseImageHandlerService> _logger;
    private readonly StoragingOptions _options;
    private readonly BlobClientOptions _blobOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="DriverLicenseImageHandlerService"/> class.
    /// </summary>
    /// <param name="logger">Logger for recording the operation logs.</param>
    /// <param name="options">Configuration options for the storage where images are uploaded.</param>
    public DriverLicenseImageHandlerService(
        ILogger<DriverLicenseImageHandlerService> logger,
        IOptions<StoragingOptions> options)
    {
        _logger = logger;
        _options = options.Value;

        _blobOptions = new BlobClientOptions();
        _blobOptions.Retry.Mode = Azure.Core.RetryMode.Fixed;
        _blobOptions.Retry.MaxRetries = 3;
        _blobOptions.Retry.NetworkTimeout = TimeSpan.FromSeconds(20);
        _blobOptions.Retry.Delay = TimeSpan.FromSeconds(5);
    }

    /// <summary>
    /// Asynchronously uploads a driver's license image to blob storage and returns the URL of the uploaded image.
    /// </summary>
    /// <param name="driverLicenseDto">Data transfer object containing the driver's license details and image file.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the request.</param>
    /// <returns>The URI of the uploaded driver's license image.</returns>
    /// <exception cref="BlobException">Thrown when the upload fails.</exception>
    public async Task<string> UploadImageAsync(DriverLicenseDto driverLicenseDto, CancellationToken cancellationToken = default)
    {
        var blobName = $"{driverLicenseDto.DriverLicenseId}{Path.GetExtension(driverLicenseDto.DriverLicenseImage!.FileName)}"; // e.g: 06991056988.jpg
        var blobClient = new BlobClient(_options.ConnectionString, _options.ContainerName, blobName, _blobOptions);

        using var stream = driverLicenseDto.DriverLicenseImage.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true, cancellationToken: cancellationToken);

        _logger.LogInformation("Driver license image successfully uploaded. Url: {DriverLicenseImageUrl}", blobClient.Uri.AbsoluteUri);

        return blobClient.Uri.AbsoluteUri;
    }
}
