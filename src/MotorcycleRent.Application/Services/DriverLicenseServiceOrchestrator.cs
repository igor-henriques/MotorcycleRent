namespace MotorcycleRent.Application.Services;

public sealed class DriverLicenseServiceOrchestrator : IDriverLicenseServiceOrchestrator
{
    private readonly IBaseRepository<DeliveryPartner> _userRepository;
    private readonly IDriverLicenseImageHandlerService _driverLicenseImageHandlerService;
    private readonly ILogger<DriverLicenseServiceOrchestrator> _logger;
    private readonly IMapper _mapper;

    public DriverLicenseServiceOrchestrator(IBaseRepository<DeliveryPartner> userRepository,
                                            IMapper mapper,
                                            ILogger<DriverLicenseServiceOrchestrator> logger,
                                            IDriverLicenseImageHandlerService driverLicenseImageHandlerService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _driverLicenseImageHandlerService = driverLicenseImageHandlerService;
    }

    public async Task CreateDriverLicense(DriverLicenseDto driverLicenseDto, CancellationToken cancellationToken = default)
    {
        var deliveryPartner = await _userRepository.GetByAsync(u => u.Email == driverLicenseDto.DeliveryPartnerEmail, cancellationToken)
            ?? throw new InvalidOperationException($"Invalid delivery partner email: {driverLicenseDto.DeliveryPartnerEmail}");

        bool driverLicenseAlreadyExists = await _userRepository.GetByAsync(u => u.DriverLicense != null && u.DriverLicense.DriverLicenseId == driverLicenseDto.DriverLicenseId, cancellationToken) != null;
        if (driverLicenseAlreadyExists)
        {
            throw new InvalidOperationException($"Driver license id {driverLicenseDto.DriverLicenseId} already exists");
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
        var deliveryPartner = await _userRepository.GetByAsync(u => u.DriverLicense != null && u.DriverLicense.DriverLicenseId == driverLicenseDto.DriverLicenseId, cancellationToken)
            ?? throw new InvalidOperationException($"Delivery partner does not exist or does not have driver license registered. Email: {driverLicenseDto.DeliveryPartnerEmail}");

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
