namespace MotorcycleRent.Infrastructure.Services;

public sealed class DriverLicenseImageHandlerService : IDriverLicenseImageHandlerService
{
    private readonly ILogger<DriverLicenseImageHandlerService> _logger;
    private readonly IValidator<IFormFile> _validator;
    private readonly StoragingOptions _options;
    private readonly BlobClientOptions _blobOptions;

    public DriverLicenseImageHandlerService(
        ILogger<DriverLicenseImageHandlerService> logger,
        IValidator<IFormFile> validator,
        IOptions<StoragingOptions> options)
    {
        _logger = logger;
        _validator = validator;
        _options = options.Value;

        _blobOptions = new BlobClientOptions();
        _blobOptions.Retry.Mode = Azure.Core.RetryMode.Fixed;
        _blobOptions.Retry.MaxRetries = 3;
        _blobOptions.Retry.NetworkTimeout = TimeSpan.FromSeconds(20);
        _blobOptions.Retry.Delay = TimeSpan.FromSeconds(5);
    }

    public async Task<string> UploadImageAsync(DriverLicenseDto driverLicenseDto, CancellationToken cancellationToken = default)
    {
        if (driverLicenseDto.DriverLicenseImage is null)
        {
            throw new InvalidOperationException("Driver license image is required");
        }

        await _validator.ValidateAndThrowAsync(driverLicenseDto.DriverLicenseImage!, cancellationToken);

        var blobName = $"{driverLicenseDto.DriverLicenseId}{Path.GetExtension(driverLicenseDto.DriverLicenseImage.FileName) }"; // e.g: 06991056988.jpg
        var blobClient = new BlobClient(_options.ConnectionString, _options.ContainerName, blobName, _blobOptions);

        using var stream = driverLicenseDto.DriverLicenseImage.OpenReadStream();
        await blobClient.UploadAsync(stream, cancellationToken);

        _logger.LogInformation("Driver license image successfully uploaded. Url: {DriverLicenseImageUrl}", blobClient.Uri.AbsoluteUri);

        return blobClient.Uri.AbsoluteUri;
    }
}
