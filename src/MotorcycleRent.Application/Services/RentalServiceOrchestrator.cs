namespace MotorcycleRent.Application.Services;

/// <summary>
/// Orchestrates rental services for motorcycles, handling the creation, management, and finalization of rentals.
/// </summary>
public sealed class RentalServiceOrchestrator : IRentalServiceOrchestrator
{
    private readonly IBaseRepository<DeliveryPartner> _partnerRepository;
    private readonly IBaseRepository<MotorcycleRental> _rentalRepository;
    private readonly IEmailClaimProvider _emailClaimProvider;
    private readonly ILogger<RentalServiceOrchestrator> _logger;
    private readonly IEnumerable<IRentalCostCalculatorService> _rentalCostCalculatorServices;
    private readonly IMapper _mapper;
    private readonly IMotorcycleServiceOrchestrator _motorcycleServiceOrchestrator;

    public RentalServiceOrchestrator(IBaseRepository<DeliveryPartner> partnerRepository,
                                   IBaseRepository<MotorcycleRental> rentRepository,
                                   ILogger<RentalServiceOrchestrator> logger,
                                   IEmailClaimProvider emailClaimProvider,
                                   IEnumerable<IRentalCostCalculatorService> rentCostCalculatorServices,
                                   IMapper mapper,
                                   IMotorcycleServiceOrchestrator motorcycleServiceOrchestrator)
    {
        _partnerRepository = partnerRepository;
        _rentalRepository = rentRepository;
        _logger = logger;
        _emailClaimProvider = emailClaimProvider;
        _rentalCostCalculatorServices = rentCostCalculatorServices;
        _mapper = mapper;
        _motorcycleServiceOrchestrator = motorcycleServiceOrchestrator;
    }

    /// <summary>
    /// Rents a motorcycle asynchronously.
    /// </summary>
    /// <param name="MotorcycleRentalDto">DTO containing the details for the new motorcycle rent.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A DTO containing details about the cost and period of the rent.</returns>
    /// <exception cref="OnGoingRentalException">Thrown if there is an ongoing rent that conflicts with the new rent request.</exception>
    /// <exception cref="EntityCreationException">Thrown if the new rent could not be created in the database.</exception>
    /// <exception cref="InternalErrorException">Thrown if there is an error updating the motorcycle status.</exception>
    public async Task<RentalPriceDto> RentMotorcycleAsync(MotorcycleRentalDto MotorcycleRentalDto, CancellationToken cancellationToken = default)
    {
        var deliveryPartner = await GetDeliveryPartnerAbleToRentAsync(cancellationToken);
        var onGoingRental = await GetOnGoingRentalForPartner(deliveryPartner, cancellationToken);

        if (onGoingRental is not null)
        {
            throw new OnGoingRentalException(onGoingRental.RentalPeriod);
        }

        var selectedMotorcycle = await SelectAvailableMotorcycleAsync(MotorcycleRentalDto, cancellationToken);

        _logger.LogInformation("{ResourceName} selected motorcycle plate {MotorcyclePlate} for a renting request to delivery partner {DeliveryPartner}",
            nameof(RentalServiceOrchestrator),
            selectedMotorcycle.Plate,
            deliveryPartner.NationalId);

        MotorcycleRental incomingRent = BuildCalculatedIncomingRental(MotorcycleRentalDto) with
        {
            Motorcycle = selectedMotorcycle,
            DeliveryPartner = deliveryPartner,
            Status = ERentStatus.Ongoing
        };

        var rentMotorcycleTask = _rentalRepository.CreateAsync(incomingRent, cancellationToken);
        var updateMotorcycleStatusTask = UpdateSelectedMotorcycleStatus(selectedMotorcycle, cancellationToken);
        var partnerUpdateTask = _partnerRepository.UpdateAsync(deliveryPartner with { HasActiveRental = true, IsAvailable = true }, cancellationToken);

        await Task.WhenAll(rentMotorcycleTask, updateMotorcycleStatusTask, partnerUpdateTask);

        if (rentMotorcycleTask.Result is null)
        {
            throw new EntityCreationException(typeof(MotorcycleRental));
        }   

        var rentalPrice = new RentalPriceDto()
        {
            FeeCost = rentMotorcycleTask.Result!.FeeCost,
            RentalBaseCost = rentMotorcycleTask.Result!.RentalCost,
            RentPeriod = rentMotorcycleTask.Result!.RentalPeriod
        };

        return rentalPrice;
    }

    /// <summary>
    /// Retrieves the ongoing rent for a specified delivery partner if it exists.
    /// </summary>
    /// <param name="deliveryPartner">The delivery partner whose ongoing rent is to be retrieved.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The ongoing rent if it exists, otherwise null.</returns>
    private async Task<MotorcycleRental?> GetOnGoingRentalForPartner(DeliveryPartner deliveryPartner, CancellationToken cancellationToken)
    {
        var onGoingRental = await _rentalRepository.GetByAsync(r => r.DeliveryPartner!.Email == deliveryPartner.Email && r.Status == ERentStatus.Ongoing, cancellationToken);
        return onGoingRental;
    }

    /// <summary>
    /// Builds a rental entity from a rent DTO and calculates the rent cost using the appropriate service.
    /// </summary>
    /// <param name="MotorcycleRentalDto">The motorcycle rent DTO to build the rent entity from.</param>
    /// <returns>The built and cost-calculated rent entity.</returns>
    private MotorcycleRental BuildCalculatedIncomingRental(MotorcycleRentalDto motorcycleRentalDto)
    {
        var incomingRental = _mapper.Map<MotorcycleRental>(motorcycleRentalDto);
        var calculatorService = _rentalCostCalculatorServices.FirstOrDefault(c => c.CanCalculate(motorcycleRentalDto.RentalPlan));

        if (calculatorService is null)
        {
            _logger.LogError("No calculator service strategy implemented for rent plan {RentPlan}",
                motorcycleRentalDto.RentalPlan);

            throw new InternalErrorException(Constants.Messages.CalculatorServiceNotFound);
        }

        var calculatedIncomingRental = calculatorService.CalculateRentalCost(incomingRental);

        return calculatedIncomingRental;
    }

    /// <summary>
    /// Updates the status of the selected motorcycle based on the rental operation.
    /// </summary>
    /// <param name="selectedMotorcycle">The motorcycle to update.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The updated motorcycle entity.</returns>
    private async Task<Motorcycle> UpdateSelectedMotorcycleStatus(Motorcycle selectedMotorcycle, CancellationToken cancellationToken)
    {
        var updateMotorcycleDto = new UpdateMotorcycleStatusDto() { Status = EMotorcycleStatus.Rented, Plate = selectedMotorcycle.Plate };
        await _motorcycleServiceOrchestrator.UpdateMotorcycleStatusAsync(updateMotorcycleDto, cancellationToken);
        return selectedMotorcycle;
    }

    /// <summary>
    /// Fetches the delivery partner using the email provided by the email claim provider.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The fetched delivery partner.</returns>
    /// <exception cref="InternalErrorException">Thrown if the delivery partner could not be found.</exception>
    private async Task<DeliveryPartner> GetDeliveryPartnerAbleToRentAsync(CancellationToken cancellationToken)
    {
        var email = _emailClaimProvider.GetUserEmail();

        var deliveryPartner = await _partnerRepository.GetByAsync(u => u.Email == email, cancellationToken)
            ?? throw new InternalErrorException(Constants.Messages.InvalidDeliveryPartner);

        if (deliveryPartner == null || !deliveryPartner.IsPartnerAbleToRent)
        {
            _logger.LogWarning("Partner '{UserEmail}' unable to rent", email);
            throw new PartnerUnableToRentException();
        }

        return deliveryPartner;
    }

    /// <summary>
    /// Fetches the delivery partner using the email provided by the email claim provider.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The fetched delivery partner.</returns>
    /// <exception cref="InternalErrorException">Thrown if the delivery partner could not be found.</exception>
    private async Task<DeliveryPartner> GetDeliveryPartnerAbleToReturnAsync(CancellationToken cancellationToken)
    {
        var email = _emailClaimProvider.GetUserEmail();

        var deliveryPartner = await _partnerRepository.GetByAsync(u => u.Email == email && u.HasActiveRental, cancellationToken)
            ?? throw new InternalErrorException(Constants.Messages.InvalidDeliveryPartner);

        return deliveryPartner;
    }

    /// <summary>
    /// Selects an available motorcycle for rent based on specified criteria.
    /// </summary>
    /// <param name="MotorcycleRentalDto">DTO containing criteria for selecting a motorcycle.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>The selected motorcycle.</returns>
    /// <exception cref="NoMotorcyclesAvailableException">Thrown if no motorcycles are available that meet the criteria.</exception>
    private async Task<Motorcycle> SelectAvailableMotorcycleAsync(MotorcycleRentalDto motorcycleRentalDto, CancellationToken cancellationToken)
    {
        var searchCriteria = new MotorcycleSearchCriteria() { Status = EMotorcycleStatus.Available };
        IEnumerable<MotorcycleDto> availableMotorcycles = await _motorcycleServiceOrchestrator.ListMotorcyclesAsync(searchCriteria, cancellationToken);
        if (!availableMotorcycles.Any())
        {
            _logger.LogWarning("No motorcycles available at the moment to attend the following rent request: {RentRequest}", motorcycleRentalDto);
            throw new NoMotorcyclesAvailableException();
        }

        var selectedMotorcycle = availableMotorcycles.First();

        return _mapper.Map<Motorcycle>(selectedMotorcycle);
    }

    /// <summary>
    /// Calculates the rent price for a motorcycle without creating a rental record.
    /// </summary>
    /// <param name="MotorcycleRentalDto">DTO containing the parameters needed to calculate the rent.</param>
    /// <returns>A DTO that details the calculated rent costs and period.</returns>
    /// <exception cref="InternalErrorException">Thrown if no suitable rent cost calculator service is available.</exception>
    public RentalPriceDto PeekRentalPrice(MotorcycleRentalDto motorcycleRentalDto)
    {
        var motorcycleRentalPricePeek = _mapper.Map<MotorcycleRental>(motorcycleRentalDto);

        var calculatorService = _rentalCostCalculatorServices.FirstOrDefault(c => c.CanCalculate(motorcycleRentalDto.RentalPlan));

        if (calculatorService is null)
        {
            _logger.LogError("No calculator service strategy implemented for rent plan {RentPlan}",
                motorcycleRentalDto.RentalPlan);

            throw new InternalErrorException(Constants.Messages.CalculatorServiceNotFound);
        }

        var rent = calculatorService.CalculateRentalCost(motorcycleRentalPricePeek);

        var rentPrice = new RentalPriceDto()
        {
            FeeCost = rent.FeeCost,
            RentalBaseCost = rent.RentalCost,
            RentPeriod = motorcycleRentalDto.RentalPeriod
        };

        _logger.LogInformation("{ResourceName} calculated price for a rent peek operation: {RentPrice} and following parameters: {RentParameters}",
            nameof(RentalServiceOrchestrator),
            rentPrice,
            motorcycleRentalDto);

        return rentPrice;
    }

    /// <summary>
    /// Finalizes the ongoing motorcycle rent for the current user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <exception cref="InvalidOperationException">Thrown if there is no ongoing rent to finalize.</exception>
    public async Task ReturnMotorcycleRentalAsync(CancellationToken cancellationToken = default)
    {
        var deliveryPartner = await GetDeliveryPartnerAbleToReturnAsync(cancellationToken);
        var onGoingRent = await GetOnGoingRentalForPartner(deliveryPartner, cancellationToken)
            ?? throw new InvalidOperationException(Constants.Messages.NoOngoingRentalForPartner);

        var updateMotorcycleDto = new UpdateMotorcycleStatusDto() { Status = EMotorcycleStatus.Available, Plate = onGoingRent.Motorcycle!.Plate };

        var rentUpdateTask = _rentalRepository.UpdateAsync(onGoingRent with { Status = ERentStatus.Finished }, cancellationToken);
        var motorcycleUpdateTask = _motorcycleServiceOrchestrator.UpdateMotorcycleStatusAsync(updateMotorcycleDto, cancellationToken);
        var partnerUpdateTask = _partnerRepository.UpdateAsync(deliveryPartner with { HasActiveRental = false, IsAvailable = false }, cancellationToken);

        await Task.WhenAll(rentUpdateTask, motorcycleUpdateTask, partnerUpdateTask);
    }
}