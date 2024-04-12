namespace MotorcycleRent.Application.Services;

/// <summary>
/// Orchestrates rental services for motorcycles, handling the creation, management, and finalization of rentals.
/// </summary>
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

    /// <summary>
    /// Creates a new motorcycle rent asynchronously.
    /// </summary>
    /// <param name="motorcycleRentDto">DTO containing the details for the new motorcycle rent.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A DTO containing details about the cost and period of the rent.</returns>
    /// <exception cref="OnGoingRentException">Thrown if there is an ongoing rent that conflicts with the new rent request.</exception>
    /// <exception cref="EntityCreationException">Thrown if the new rent could not be created in the database.</exception>
    /// <exception cref="InternalErrorException">Thrown if there is an error updating the motorcycle state.</exception>
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
            DeliveryPartner = deliveryPartner,
            RentStatus = ERentStatus.Ongoing
        };

        var createRentTask = _rentRepository.CreateAsync(incomingRent, cancellationToken);
        var updateMotorcycleStateTask = UpdateSelectedMotorcycleState(selectedMotorcycle, cancellationToken);

        await Task.WhenAll(createRentTask, updateMotorcycleStateTask);

        if (createRentTask.Result is null)
        {
            throw new EntityCreationException(typeof(Domain.Entities.MotorcycleRent));
        }   

        var rentPrice = new RentPriceDto()
        {
            FeeCost = createRentTask.Result!.FeeCost,
            RentCost = createRentTask.Result!.RentCost,
            RentPeriod = createRentTask.Result!.RentPeriod
        };

        return rentPrice;
    }

    /// <summary>
    /// Retrieves the ongoing rent for a specified delivery partner if it exists.
    /// </summary>
    /// <param name="deliveryPartner">The delivery partner whose ongoing rent is to be retrieved.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The ongoing rent if it exists, otherwise null.</returns>
    private async Task<Domain.Entities.MotorcycleRent?> GetOnGoingRentForPartner(DeliveryPartner deliveryPartner, CancellationToken cancellationToken)
    {
        var onGoingRent = await _rentRepository.GetByAsync(r => r.DeliveryPartner!.Email == deliveryPartner.Email && r.RentStatus == ERentStatus.Ongoing, cancellationToken);
        return onGoingRent;
    }

    /// <summary>
    /// Builds a rent entity from a rent DTO and calculates the rent cost using the appropriate service.
    /// </summary>
    /// <param name="motorcycleRentDto">The motorcycle rent DTO to build the rent entity from.</param>
    /// <returns>The built and cost-calculated rent entity.</returns>
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

    /// <summary>
    /// Updates the state of the selected motorcycle based on the rental operation.
    /// </summary>
    /// <param name="selectedMotorcycle">The motorcycle to update.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The updated motorcycle entity.</returns>
    private async Task<Domain.Entities.Motorcycle> UpdateSelectedMotorcycleState(Domain.Entities.Motorcycle selectedMotorcycle, CancellationToken cancellationToken)
    {
        var updateMotorcycleDto = new UpdateMotorcycleStateDto() { State = EMotorcycleState.Rented, Plate = selectedMotorcycle.Plate };
        await _motorcycleServiceOrchestrator.UpdateMotorcycleStateAsync(updateMotorcycleDto, cancellationToken);
        return selectedMotorcycle;
    }

    /// <summary>
    /// Fetches the delivery partner using the email provided by the email claim provider.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The fetched delivery partner.</returns>
    /// <exception cref="InternalErrorException">Thrown if the delivery partner could not be found.</exception>
    private async Task<DeliveryPartner> GetDeliveryPartnerAsync(CancellationToken cancellationToken)
    {
        var email = _emailClaimProvider.GetUserEmail();

        var deliveryPartner = await _userRepository.GetByAsync(u => u.Email == email, cancellationToken)
            ?? throw new InternalErrorException($"An error occurred while searching for partner data");

        if (deliveryPartner == null || !deliveryPartner.IsPartnerAbleToRent)
        {
            _logger.LogWarning("Partner '{UserEmail}' unable to rent", email);
            throw new PartnerUnableToRentException();
        }

        return deliveryPartner;
    }

    /// <summary>
    /// Selects an available motorcycle for rent based on specified criteria.
    /// </summary>
    /// <param name="motorcycleRentDto">DTO containing criteria for selecting a motorcycle.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The selected motorcycle.</returns>
    /// <exception cref="NoMotorcyclesAvailableException">Thrown if no motorcycles are available that meet the criteria.</exception>
    private async Task<Motorcycle> SelectAvailableMotorcycleAsync(MotorcycleRentDto motorcycleRentDto, CancellationToken cancellationToken)
    {
        var searchCriteria = new MotorcycleSearchCriteria() { State = EMotorcycleState.Available };
        IEnumerable<MotorcycleDto> availableMotorcycles = await _motorcycleServiceOrchestrator.ListMotorcyclesAsync(searchCriteria, cancellationToken);
        if (!availableMotorcycles.Any())
        {
            _logger.LogWarning("No motorcycles available at the moment to attend the following rent request: {RentRequest}", motorcycleRentDto);
            throw new NoMotorcyclesAvailableException();
        }

        var selectedMotorcycle = availableMotorcycles.First();

        return _mapper.Map<Motorcycle>(selectedMotorcycle);
    }

    /// <summary>
    /// Calculates the rent price for a motorcycle without creating a rental record.
    /// </summary>
    /// <param name="motorcycleRentDto">DTO containing the parameters needed to calculate the rent.</param>
    /// <returns>A DTO that details the calculated rent costs and period.</returns>
    /// <exception cref="InternalErrorException">Thrown if no suitable rent cost calculator service is available.</exception>
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

    /// <summary>
    /// Finalizes the ongoing motorcycle rent for the current user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <exception cref="InvalidOperationException">Thrown if there is no ongoing rent to finalize.</exception>
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