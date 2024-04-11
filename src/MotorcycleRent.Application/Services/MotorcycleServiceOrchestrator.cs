global using MotorcycleRent.Application.Models.Dtos;

namespace MotorcycleRent.Application.Services;

public sealed class MotorcycleServiceOrchestrator : IMotorcycleServiceOrchestrator
{
    private readonly IBaseRepository<Motorcycle> _motorcycleRepository;
    private readonly IBaseRepository<Domain.Entities.MotorcycleRent> _motorcycleRentRepository;
    private readonly ILogger<MotorcycleServiceOrchestrator> _logger;
    private readonly IMapper _mapper;

    public MotorcycleServiceOrchestrator(IBaseRepository<Motorcycle> motorcycleRepository,
                                         ILogger<MotorcycleServiceOrchestrator> logger,
                                         IMapper mapper,
                                         IBaseRepository<Domain.Entities.MotorcycleRent> motorcycleRentRepository)
    {
        _motorcycleRepository = motorcycleRepository;
        _logger = logger;
        _mapper = mapper;
        _motorcycleRentRepository = motorcycleRentRepository;
    }

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
        catch (MongoWriteException ex)
        {
            _logger.LogInformation("{ResourceName} caught a duplicate operation. Motorcycles should have an unique plate. Plate: {MotorcyclePlate}. Exception: {exception}",
                nameof(MotorcycleServiceOrchestrator),
                motorcycleDto.Plate,
                ex);

            throw;
        }
    }

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

        var filteredMotorcycles = await _motorcycleRepository.GetAllAsync(filter, cancellationToken);

        _logger.LogInformation("{ResourceName} fetched {MotorcyclesCount} motorcycles",
                nameof(MotorcycleServiceOrchestrator),
                filteredMotorcycles.Count());

        return filteredMotorcycles.Select(_mapper.Map<MotorcycleDto>);
    }

    public async Task UpdateMotorcyclePlateAsync(UpdateMotorcyclePlateDto plateUpdateDto, CancellationToken cancellationToken = default)
    {
        var motorcycle = await _motorcycleRepository.GetByAsync(m => m.Plate == plateUpdateDto.OldPlate, cancellationToken) 
            ?? throw new InvalidOperationException($"Invalid motorcycle plate: {plateUpdateDto.OldPlate}");

        try
        {
            await _motorcycleRepository.UpdateAsync(motorcycle with { Plate = plateUpdateDto.NewPlate }, cancellationToken);
        }
        catch (MongoWriteException ex)
        {
            _logger.LogInformation("{ResourceName} caught a duplicate operation while trying to perform a plate updating. Motorcycles should have an unique plate. Existing plate: {MotorcyclePlate}. Exception: {exception}",
                nameof(MotorcycleServiceOrchestrator),
                plateUpdateDto.NewPlate,
                ex);

            throw;
        }

        _logger.LogInformation("Motorcycle id {MotorcycleId} had a plate update from {OldPlate} to {NewPlate}",
            motorcycle.Id,
            plateUpdateDto.OldPlate,
            plateUpdateDto.NewPlate);
    }

    public async Task DeleteMotorcycleAsync(string motorcyclePlate, CancellationToken cancellationToken = default)
    {
        var motorcycle = await _motorcycleRepository.GetByAsync(m => m.Plate!.Equals(motorcyclePlate, StringComparison.CurrentCultureIgnoreCase), cancellationToken)
            ?? throw new InvalidOperationException($"Invalid motorcycle plate: {motorcyclePlate}");

        var rents = await _motorcycleRentRepository.GetAllByAsync(m => m.Motorcycle!.Plate == motorcycle.Plate, cancellationToken);

        if (!rents.Any())
        {
            await _motorcycleRepository.DeleteAsync(motorcycle.Id, cancellationToken);
            _logger.LogInformation("Motorcycle plate {MotorcyclePlate} successfully deleted", motorcycle.Plate);
        }
        else
        {
            _logger.LogWarning("Motorcycle plate {MotorcyclePlate} cannot be deleted as it has {RentCount} rent records.", motorcycle.Plate, rents.Count());
        }
    }
}
