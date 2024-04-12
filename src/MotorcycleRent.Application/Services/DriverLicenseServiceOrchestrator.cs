namespace MotorcycleRent.Application.Services;

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

    public async Task CreateDriverLicense(DriverLicenseDto driverLicenseDto, CancellationToken cancellationToken = default)
    {
        var deliveryPartnerEmail = _emailClaimProvider.GetUserEmail();

        var deliveryPartner = await _userRepository.GetByAsync(u => u.Email == deliveryPartnerEmail, cancellationToken)
            ?? throw new InvalidOperationException("Invalid delivery partner");

        bool driverLicenseAlreadyExists = await _userRepository.GetByAsync(u => u.DriverLicense != null && u.DriverLicense.DriverLicenseId == driverLicenseDto.DriverLicenseId, cancellationToken) != null;
        if (driverLicenseAlreadyExists)
        {
            _logger.LogWarning("{ResourceName} caught attempt to create driver license that already exists for user {UserEmail}",
                nameof(DriverLicenseServiceOrchestrator),
                deliveryPartnerEmail);

            throw new InvalidOperationException("Driver license already exists");
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

    public async Task UpdateDriverLicense(DriverLicenseDto driverLicenseDto, CancellationToken cancellationToken = default)
    {
        var deliveryPartnerEmail = _emailClaimProvider.GetUserEmail();

        var deliveryPartner = await _userRepository.GetByAsync(u => u.DriverLicense != null && u.DriverLicense.DriverLicenseId == driverLicenseDto.DriverLicenseId, cancellationToken)
            ?? throw new InvalidOperationException("Delivery partner does not exist or does not have driver license registered");

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
