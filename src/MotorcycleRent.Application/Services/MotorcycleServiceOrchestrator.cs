namespace MotorcycleRent.Application.Services;

/// <summary>
/// Orchestrates motorcycle-related services including creation, updating, listing, and deleting of motorcycles.
/// </summary>
public sealed class MotorcycleServiceOrchestrator : IMotorcycleServiceOrchestrator
{
    private readonly IBaseRepository<Motorcycle> _motorcycleRepository;
    private readonly IBaseRepository<MotorcycleRental> _MotorcycleRentalRepository;
    private readonly ILogger<MotorcycleServiceOrchestrator> _logger;
    private readonly IMapper _mapper;

    public MotorcycleServiceOrchestrator(IBaseRepository<Motorcycle> motorcycleRepository,
                                         ILogger<MotorcycleServiceOrchestrator> logger,
                                         IMapper mapper,
                                         IBaseRepository<MotorcycleRental> MotorcycleRentalRepository)
    {
        _motorcycleRepository = motorcycleRepository;
        _logger = logger;
        _mapper = mapper;
        _MotorcycleRentalRepository = MotorcycleRentalRepository;
    }

    /// <summary>
    /// Creates a new motorcycle asynchronously.
    /// </summary>
    /// <param name="motorcycleDto">The motorcycle DTO containing the data to create a new motorcycle.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The ID of the newly created motorcycle.</returns>
    /// <exception cref="InvalidOperationException">Thrown if there is a duplicate plate exception.</exception>
    public async Task<Guid> CreateMotorcycleAsync(MotorcycleDto motorcycleDto, CancellationToken cancellationToken = default)
    {
        var incomingMotorcycle = _mapper.Map<Motorcycle>(motorcycleDto);

        try
        {
            // Create an index with the plate identification to ensure uniqueness.
            // Only creates the index case it don't exist.                                                                   
            await _motorcycleRepository.CreateIndexAsync(
                new CreateIndexModel<Motorcycle>(
                    Builders<Motorcycle>.IndexKeys.Descending(d => d.Plate),
                    new CreateIndexOptions { Unique = true, Name = "UniquePlateConstraint" }),
                cancellationToken);

            var motorcycle = await _motorcycleRepository.CreateAsync(incomingMotorcycle, cancellationToken)
                ?? throw new EntityCreationException(typeof(Motorcycle), motorcycleDto.Plate);

            _logger.LogInformation("{ResourceName} created a new motorcycle: {MotorcyclePlate}",
                nameof(MotorcycleServiceOrchestrator),
                motorcycleDto.Plate);

            return motorcycle!.Id;
        }
        catch (MongoWriteException)
        {
            _logger.LogInformation("{ResourceName} caught a duplicate operation. Motorcycles should have an unique plate. Plate: {MotorcyclePlate}.",
                nameof(MotorcycleServiceOrchestrator),
                motorcycleDto.Plate);

            throw new InvalidOperationException(Constants.Messages.InvalidPlateOperation);
        }
    }

    /// <summary>
    /// Retrieves a list of motorcycles based on provided search criteria.
    /// </summary>
    /// <param name="criteria">The search criteria to filter motorcycles.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A list of motorcycle DTOs matching the criteria.</returns>
    public async Task<IEnumerable<MotorcycleDto>> ListMotorcyclesAsync(MotorcycleSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Motorcycle>.Filter.Empty;

        if (criteria.Id.HasValue)
        {
            filter &= Builders<Motorcycle>.Filter.Eq(m => m.Id, criteria.Id.Value);
        }

        if (criteria.Year.HasValue)
        {
            filter &= Builders<Motorcycle>.Filter.Eq(m => m.Year, criteria.Year.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Model))
        {
            filter &= Builders<Motorcycle>.Filter.Eq(m => m.Model, criteria.Model);
        }

        if (!string.IsNullOrWhiteSpace(criteria.Plate))
        {
            filter &= Builders<Motorcycle>.Filter.Eq(m => m.Plate, criteria.Plate);
        }

        if (criteria.Status.HasValue)
        {
            filter &= Builders<Motorcycle>.Filter.Eq(m => m.Status, criteria.Status.Value);
        }

        var filteredMotorcycles = await _motorcycleRepository.GetAllAsync(filter, cancellationToken);

        _logger.LogInformation("{ResourceName} fetched {MotorcyclesCount} motorcycles",
                nameof(MotorcycleServiceOrchestrator),
                filteredMotorcycles.Count());

        return filteredMotorcycles.Select(_mapper.Map<MotorcycleDto>);
    }

    /// <summary>
    /// Updates the plate number of an existing motorcycle.
    /// </summary>
    /// <param name="plateUpdateDto">The DTO containing the old and new plate numbers.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="InvalidOperationException">Thrown if the motorcycle does not exist or the new plate already exists.</exception>
    public async Task UpdateMotorcyclePlateAsync(UpdateMotorcyclePlateDto plateUpdateDto, CancellationToken cancellationToken = default)
    {
        var motorcycle = await _motorcycleRepository.GetByAsync(m => m.Plate == plateUpdateDto.OldPlate, cancellationToken)
            ?? throw new InvalidOperationException(Constants.Messages.InvalidMotorcyclePlate);

        var rent = await _MotorcycleRentalRepository.GetByAsync(m => m.Motorcycle!.Plate == motorcycle.Plate && m.Status == ERentStatus.Ongoing, cancellationToken);

        if (rent is not null)
        {
            throw new OnGoingRentalException(rent.RentalPeriod, rent.Motorcycle!.Plate!);
        }

        try
        {
            _ = await _motorcycleRepository.UpdateAsync(motorcycle with { Plate = plateUpdateDto.NewPlate }, cancellationToken)
                ?? throw new InternalErrorException(Constants.Messages.InvalidMotorcycleUpdate);
        }
        catch (MongoWriteException)
        {
            _logger.LogError("{ResourceName} caught a duplicate operation while trying to perform a plate updating. Existing plate: {MotorcyclePlate}.",
                nameof(MotorcycleServiceOrchestrator),
                plateUpdateDto.NewPlate);

            throw new InvalidOperationException(Constants.Messages.InvalidMotorcycleUpdate);
        }

        _logger.LogInformation("Motorcycle id {MotorcycleId} had a plate update from {OldPlate} to {NewPlate}",
            motorcycle.Id,
            plateUpdateDto.OldPlate,
            plateUpdateDto.NewPlate);
    }

    /// <summary>
    /// Updates the status of a motorcycle.
    /// </summary>
    /// <param name="plateUpdateDto">The DTO containing the motorcycle plate and the new status.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="InvalidOperationException">Thrown if the motorcycle plate is invalid or an error occurs during the update.</exception>
    public async Task UpdateMotorcycleStatusAsync(UpdateMotorcycleStatusDto plateUpdateDto, CancellationToken cancellationToken = default)
    {
        var motorcycle = await _motorcycleRepository.GetByAsync(m => m.Plate!.Equals(plateUpdateDto.Plate, StringComparison.CurrentCultureIgnoreCase), cancellationToken)
            ?? throw new InvalidOperationException(Constants.Messages.InvalidMotorcyclePlate);

        _ = await _motorcycleRepository.UpdateAsync(motorcycle with { Status = plateUpdateDto.Status }, cancellationToken)
            ?? throw new InternalErrorException(Constants.Messages.InvalidMotorcycleUpdate);

        _logger.LogInformation("Motorcycle plate {MotorcyclePlate} had a status update from {OldStatus} to {NewStatus}",
            motorcycle.Plate,
            motorcycle.Status,
            plateUpdateDto.Status);
    }

    /// <summary>
    /// Deletes a motorcycle by its plate number.
    /// </summary>
    /// <param name="motorcyclePlate">The plate number of the motorcycle to delete.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="InvalidOperationException">Thrown if the motorcycle cannot be deleted due to existing rent records or other constraints.</exception>
    public async Task DeleteMotorcycleAsync(string motorcyclePlate, CancellationToken cancellationToken = default)
    {
        var motorcycle = await _motorcycleRepository.GetByAsync(m => m.Plate!.Equals(motorcyclePlate, StringComparison.CurrentCultureIgnoreCase), cancellationToken)
            ?? throw new InvalidOperationException(Constants.Messages.InvalidMotorcyclePlate);

        var rent = await _MotorcycleRentalRepository.GetAllByAsync(m => m.Motorcycle!.Plate == motorcycle.Plate, cancellationToken);
        var lastRent = rent.LastOrDefault();

        if (lastRent is null)
        {
            await _motorcycleRepository.DeleteAsync(motorcycle.Id, cancellationToken);
            _logger.LogInformation("Motorcycle plate {MotorcyclePlate} successfully deleted", motorcycle.Plate);
        }
        else
        {
            _logger.LogWarning("Motorcycle plate {MotorcyclePlate} cannot be deleted as it was rented once", motorcycle.Plate);
            throw new InvalidOperationException(Constants.Messages.MotorcycleRentedOnce);
        }
    }
}
