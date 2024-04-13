namespace MotorcycleRent.Application.Services;

/// <summary>
/// Orchestrates operations related to driver licenses for delivery partners.
/// </summary>
public sealed class DriverLicenseServiceOrchestrator : IDriverLicenseServiceOrchestrator
{
    private readonly IBaseRepository<DeliveryPartner> _userRepository;
    private readonly IDriverLicenseImageHandlerService _driverLicenseImageHandlerService;
    private readonly ILogger<DriverLicenseServiceOrchestrator> _logger;
    private readonly IMapper _mapper;
    private readonly IEmailClaimProvider _emailClaimProvider;

    public DriverLicenseServiceOrchestrator(IBaseRepository<DeliveryPartner> userRepository,
                                            IMapper mapper,
                                            ILogger<DriverLicenseServiceOrchestrator> logger,
                                            IDriverLicenseImageHandlerService driverLicenseImageHandlerService,
                                            IEmailClaimProvider emailClaimProvider)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _driverLicenseImageHandlerService = driverLicenseImageHandlerService;
        _emailClaimProvider = emailClaimProvider;
    }

    /// <summary>
    /// Creates a new driver license for a delivery partner.
    /// </summary>
    /// <param name="driverLicenseDto">DTO containing the driver license data.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <exception cref="InvalidOperationException">Thrown if the delivery partner does not exist or a driver license already exists.</exception>
    /// <remarks>
    /// This method first retrieves the delivery partner by email, checks if the driver license already exists,
    /// and if not, proceeds to create and update the driver license record for the user. Images associated with
    /// the driver license are handled via an external service.
    /// </remarks>
    public async Task CreateDriverLicense(DriverLicenseDto driverLicenseDto, CancellationToken cancellationToken = default)
    {
        var deliveryPartnerEmail = _emailClaimProvider.GetUserEmail();

        var deliveryPartner = await _userRepository.GetByAsync(u => u.Email == deliveryPartnerEmail, cancellationToken)
            ?? throw new InvalidOperationException(Constants.Messages.InvalidDeliveryPartner);

        bool driverLicenseAlreadyExists = await _userRepository.GetByAsync(u => u.DriverLicense != null && u.DriverLicense.DriverLicenseId == driverLicenseDto.DriverLicenseId, cancellationToken) != null;
        if (driverLicenseAlreadyExists)
        {
            _logger.LogWarning("{ResourceName} caught attempt to create driver license that already exists for user {UserEmail}",
                nameof(DriverLicenseServiceOrchestrator),
                deliveryPartnerEmail);

            throw new InvalidOperationException(Constants.Messages.DriverLicenseExists);
        }

        var driverLicense = _mapper.Map<DriverLicense>(driverLicenseDto);
        var driverLicenseImageUrl = await _driverLicenseImageHandlerService.UploadImageAsync(driverLicenseDto, cancellationToken);

        var updatedUser = await _userRepository.UpdateAsync(deliveryPartner! with
        {
            DriverLicense = driverLicense with { DriverLicenseImageUrl = driverLicenseImageUrl }
        }, cancellationToken);

        _logger.LogInformation("Driver license for user {UserEmail} added: {DriverLicense}",
            updatedUser!.Email,
            updatedUser!.DriverLicense);
    }

    /// <summary>
    /// Updates an existing driver license for a delivery partner.
    /// </summary>
    /// <param name="driverLicenseDto">DTO containing the new driver license data.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <exception cref="InvalidOperationException">Thrown if the delivery partner or their existing driver license cannot be found.</exception>
    /// <remarks>
    /// This method fetches the existing driver license based on the driver license ID, updates it with new data,
    /// and saves the changes. If the driver license image is updated, it is re-uploaded and the URL is updated.
    /// </remarks>
    public async Task UpdateDriverLicense(DriverLicenseDto driverLicenseDto, CancellationToken cancellationToken = default)
    {
        var deliveryPartnerEmail = _emailClaimProvider.GetUserEmail();

        var deliveryPartner = await _userRepository.GetByAsync(u => u.DriverLicense != null && u.DriverLicense.DriverLicenseId == driverLicenseDto.DriverLicenseId, cancellationToken)
            ?? throw new InvalidOperationException(Constants.Messages.InvalidDriverLicense);

        var driverLicense = _mapper.Map<DriverLicense>(driverLicenseDto);
        var driverLicenseImageUrl = await _driverLicenseImageHandlerService.UploadImageAsync(driverLicenseDto, cancellationToken);

        var updatedUser = await _userRepository.UpdateAsync(deliveryPartner! with
        {
            DriverLicense = driverLicense with { DriverLicenseImageUrl = driverLicenseImageUrl }
        }, cancellationToken);

        _logger.LogInformation("Driver license for user {UserEmail} updated: {DriverLicense}",
            updatedUser!.Email,
            updatedUser!.DriverLicense);
    }
}
