namespace MotorcycleRent.Application.Services;

public sealed class RentServiceOrchestrator : IRentServiceOrchestrator
{
    private readonly IBaseRepository<DeliveryPartner> _userRepository;
    private readonly IBaseRepository<Domain.Entities.MotorcycleRent> _rentRepository;
    private readonly IEmailClaimProvider _emailClaimProvider;
    private readonly ILogger<RentServiceOrchestrator> _logger;
    private readonly IEnumerable<IRentCostCalculatorService> _rentCostCalculatorServices;
    private readonly IMapper _mapper;
    private readonly IMotorcycleServiceOrchestrator _motorcycleServiceOrchestrator;

    public RentServiceOrchestrator(IBaseRepository<DeliveryPartner> userRepository,
                                   IBaseRepository<Domain.Entities.MotorcycleRent> rentRepository,
                                   ILogger<RentServiceOrchestrator> logger,
                                   IEmailClaimProvider emailClaimProvider,
                                   IEnumerable<IRentCostCalculatorService> rentCostCalculatorServices,
                                   IMapper mapper,
                                   IMotorcycleServiceOrchestrator motorcycleServiceOrchestrator)
    {
        _userRepository = userRepository;
        _rentRepository = rentRepository;
        _logger = logger;
        _emailClaimProvider = emailClaimProvider;
        _rentCostCalculatorServices = rentCostCalculatorServices;
        _mapper = mapper;
        _motorcycleServiceOrchestrator = motorcycleServiceOrchestrator;
    }

    public async Task<RentPriceDto> CreateMotorcycleRentAsync(MotorcycleRentDto motorcycleRentDto, CancellationToken cancellationToken = default)
    {
        var deliveryPartner = await GetDeliveryPartnerAsync(cancellationToken);
        var onGoingRent = await GetOnGoingRentForPartner(deliveryPartner, cancellationToken);

        if (onGoingRent is not null)
        {
            throw new OnGoingRentException(onGoingRent.RentPeriod);
        }

        var selectedMotorcycle = await SelectAvailableMotorcycleAsync(motorcycleRentDto, cancellationToken);

        _logger.LogInformation("{ResourceName} selected motorcycle plate {MotorcyclePlate} for a renting request to delivery partner {DeliveryPartner}",
            nameof(RentServiceOrchestrator),
            selectedMotorcycle.Plate,
            deliveryPartner.NationalId);

        Domain.Entities.MotorcycleRent incomingRent = BuildCalculatedIncomingRent(motorcycleRentDto) with
        {
            Motorcycle = selectedMotorcycle,
            DeliveryPartner = deliveryPartner
        };

        var createRentTask = _rentRepository.CreateAsync(incomingRent with { RentStatus = ERentStatus.Ongoing }, cancellationToken)
            ?? throw new EntityCreationException(typeof(Domain.Entities.MotorcycleRent));

        var updateMotorcycleStateTask = UpdateSelectedMotorcycleState(selectedMotorcycle, cancellationToken)
            ?? throw new InternalErrorException("An error occurred while updating a motorcycle state");

        await Task.WhenAll(createRentTask, updateMotorcycleStateTask);

        var rentPrice = new RentPriceDto()
        {
            FeeCost = createRentTask.Result!.FeeCost,
            RentCost = createRentTask.Result!.RentCost,
            RentPeriod = createRentTask.Result!.RentPeriod
        };

        return rentPrice;
    }

    private async Task<Domain.Entities.MotorcycleRent?> GetOnGoingRentForPartner(DeliveryPartner deliveryPartner, CancellationToken cancellationToken)
    {
        var onGoingRent = await _rentRepository.GetByAsync(r => r.DeliveryPartner!.Email == deliveryPartner.Email && r.RentStatus == ERentStatus.Ongoing, cancellationToken);
        return onGoingRent;
    }

    private Domain.Entities.MotorcycleRent BuildCalculatedIncomingRent(MotorcycleRentDto motorcycleRentDto)
    {
        var incomingRent = _mapper.Map<Domain.Entities.MotorcycleRent>(motorcycleRentDto);
        var calculatorService = _rentCostCalculatorServices.FirstOrDefault(c => c.CanCalculate(motorcycleRentDto.RentPlan));

        if (calculatorService is null)
        {
            _logger.LogError("No calculator service strategy implemented for rent plan {RentPlan}",
                motorcycleRentDto.RentPlan);

            throw new InternalErrorException("Calculator service unavailable at the moment");
        }

        var calculatedIncomingRent = calculatorService.CalculateRentCost(incomingRent);

        return calculatedIncomingRent;
    }

    private async Task<Domain.Entities.Motorcycle> UpdateSelectedMotorcycleState(Domain.Entities.Motorcycle selectedMotorcycle, CancellationToken cancellationToken)
    {
        var updateMotorcycleDto = new UpdateMotorcycleStateDto() { State = EMotorcycleState.Rented, Plate = selectedMotorcycle.Plate };
        await _motorcycleServiceOrchestrator.UpdateMotorcycleStateAsync(updateMotorcycleDto, cancellationToken);
        return selectedMotorcycle;
    }

    private async Task<DeliveryPartner> GetDeliveryPartnerAsync(CancellationToken cancellationToken)
    {
        var email = _emailClaimProvider.GetUserEmail();

        var user = await _userRepository.GetByAsync(u => u.Email == email, cancellationToken)
            ?? throw new InternalErrorException($"An error occurred while searching for partner data");

        var deliveryPartner = (DeliveryPartner)user;

        if (deliveryPartner == null || !deliveryPartner.IsPartnerAbleToRent)
        {
            _logger.LogWarning("Partner '{UserEmail}' unable to rent", email);
            throw new PartnerUnableToRentException();
        }

        return deliveryPartner;
    }

    private async Task<Domain.Entities.Motorcycle> SelectAvailableMotorcycleAsync(MotorcycleRentDto motorcycleRentDto, CancellationToken cancellationToken)
    {
        var searchCriteria = new MotorcycleSearchCriteria() { State = EMotorcycleState.Available };
        IEnumerable<MotorcycleDto> availableMotorcycles = await _motorcycleServiceOrchestrator.ListMotorcyclesAsync(searchCriteria, cancellationToken);
        if (!availableMotorcycles.Any())
        {
            _logger.LogWarning("No motorcycles available at the moment to attend the following rent request: {RentRequest}", motorcycleRentDto);
            throw new NoMotorcyclesAvailableException();
        }

        var selectedMotorcycle = availableMotorcycles.First();

        return _mapper.Map<Domain.Entities.Motorcycle>(selectedMotorcycle);
    }


    public RentPriceDto PeekRentPrice(MotorcycleRentDto motorcycleRentDto)
    {
        var motorcycleRentPricePeek = _mapper.Map<Domain.Entities.MotorcycleRent>(motorcycleRentDto);

        var calculatorService = _rentCostCalculatorServices.FirstOrDefault(c => c.CanCalculate(motorcycleRentDto.RentPlan));

        if (calculatorService is null)
        {
            _logger.LogError("No calculator service strategy implemented for rent plan {RentPlan}",
                motorcycleRentDto.RentPlan);

            throw new InternalErrorException("Calculator service unavailable at the moment");
        }

        var rent = calculatorService.CalculateRentCost(motorcycleRentPricePeek);

        var rentPrice = new RentPriceDto()
        {
            FeeCost = rent.FeeCost,
            RentCost = rent.RentCost,
            RentPeriod = motorcycleRentDto.RentPeriod
        };

        _logger.LogInformation("{ResourceName} calculated price for a rent peek operation: {RentPrice} and following parameters: {RentParameters}",
            nameof(RentServiceOrchestrator),
            rentPrice,
            motorcycleRentDto);

        return rentPrice;
    }

    public async Task ReturnMotorcycleRentAsync(CancellationToken cancellationToken = default)
    {
        var deliveryPartner = await GetDeliveryPartnerAsync(cancellationToken);
        var onGoingRent = await GetOnGoingRentForPartner(deliveryPartner, cancellationToken)
            ?? throw new InvalidOperationException("No on going rent was found for the current partner");

        var updateMotorcycleDto = new UpdateMotorcycleStateDto() { State = EMotorcycleState.Available, Plate = onGoingRent.Motorcycle!.Plate };

        var rentUpdateTask = _rentRepository.UpdateAsync(onGoingRent with { RentStatus = ERentStatus.Finished }, cancellationToken);
        var motorcycleUpdateTask = _motorcycleServiceOrchestrator.UpdateMotorcycleStateAsync(updateMotorcycleDto, cancellationToken);

        await Task.WhenAll(rentUpdateTask, motorcycleUpdateTask);
    }
}