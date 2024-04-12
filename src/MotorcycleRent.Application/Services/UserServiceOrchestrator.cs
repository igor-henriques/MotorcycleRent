namespace MotorcycleRent.Application.Services;

public sealed class UserServiceOrchestrator : IUserServiceOrchestrator
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly ILogger<UserServiceOrchestrator> _logger;
    private readonly IMapper _mapper;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly ITokenGeneratorService _tokenGeneratorService;

    public UserServiceOrchestrator(IPasswordHashingService passwordHashingService,
                                   IBaseRepository<User> adminRepo,
                                   IMapper mapper,
                                   ILogger<UserServiceOrchestrator> logger,
                                   ITokenGeneratorService tokenGeneratorService)
    {
        _passwordHashingService = passwordHashingService;
        _userRepository = adminRepo;
        _mapper = mapper;
        _logger = logger;
        _tokenGeneratorService = tokenGeneratorService;
    }

    public async Task CreateAdministratorAsync(AdministratorDto administratorDto, CancellationToken cancellationToken = default)
    {
        Administrator incomingAdministratorUser = _mapper.Map<Administrator>(administratorDto);

        User? createdAdministratorUser = await _userRepository.CreateAsync(incomingAdministratorUser with
        {
            Claims = [
                new Claim(ClaimTypes.Role, nameof(Administrator)),
                new Claim(ClaimTypes.Email, administratorDto.Email!)
            ],
            HashedPassword = _passwordHashingService.HashPassword(administratorDto.Password)
        }, cancellationToken);

        if (createdAdministratorUser is null)
        {
            throw new EntityCreationException(typeof(User), administratorDto.Email);
        }

        _logger.LogInformation("{ResourceName} created a new administrator user: {UserEmail}",
            nameof(UserServiceOrchestrator),
            createdAdministratorUser.Email);
    }

    public async Task CreateDeliveryPartnerAsync(DeliveryPartnerDto deliveryPartnerDto, CancellationToken cancellationToken = default)
    {
        DeliveryPartner incomingDeliveryPartnerUser = _mapper.Map<DeliveryPartner>(deliveryPartnerDto);

        // Create an index with the plate identification to ensure uniqueness.
        // Only creates the index case it don't exist.                                                                   
        await _userRepository.CreateIndexAsync(
            new CreateIndexModel<User>(
                Builders<User>.IndexKeys.Descending("NationalId"),
                new CreateIndexOptions { Unique = true, Name = "NationalIdConstraint" }),
            cancellationToken);

        try
        {
            User? createdDeliveryPartner = await _userRepository.CreateAsync(incomingDeliveryPartnerUser with
            {
                Claims = [
                    new Claim(ClaimTypes.Role, nameof(DeliveryPartner)),
                new Claim(ClaimTypes.Email, deliveryPartnerDto.Email!)
                ],
                HashedPassword = _passwordHashingService.HashPassword(deliveryPartnerDto.Password)
            }, cancellationToken);

            if (createdDeliveryPartner is null)
            {
                throw new EntityCreationException(typeof(User), deliveryPartnerDto.Email);
            }

            _logger.LogInformation("{ResourceName} created a new delivery partner user: {UserEmail}",
            nameof(UserServiceOrchestrator),
            createdDeliveryPartner.Email);
        }
        catch (MongoWriteException)
        {
            _logger.LogError("{ResourceName} caught a duplicate operation while trying to perform a plate updating. Existing plate: {MotorcyclePlate}.",
                nameof(UserServiceOrchestrator),
                incomingDeliveryPartnerUser.Email);

            throw new InvalidOperationException("User already exists");
        }
    }

    public async Task<JwtToken> AuthenticateAsync(UserDto userDto, CancellationToken cancellationToken = default)
    {
        User? user = await _userRepository.GetByAsync(user => user.Email == userDto.Email, cancellationToken);

        if (user is null)
        {
            _logger.LogWarning("{ResourceName} failed to fetch user with email '{UserEmail}' from the database. Record does not exist.",
                nameof(UserServiceOrchestrator),
                userDto.Email);

            throw new InvalidCredentialsException();
        }

        if (!_passwordHashingService.VerifyPassword(user.HashedPassword, userDto.Password))
        {
            _logger.LogWarning("{ResourceName} failed to authenticate user with email '{UserEmail}'. Unmatching passwords.",
                nameof(UserServiceOrchestrator),
                userDto.Email);

            throw new InvalidCredentialsException();
        }

        var token = _tokenGeneratorService.GenerateToken(user.Claims);

        _logger.LogInformation("{ResourceName} successfully generated a new token for the user email '{UserEmail}': {JwtToken}",
            nameof(UserServiceOrchestrator),
            user.Email,
            token);

        return token;
    }
}
