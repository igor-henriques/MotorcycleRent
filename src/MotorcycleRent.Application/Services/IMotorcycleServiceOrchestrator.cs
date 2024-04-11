using MotorcycleRent.Application.Models.Dtos;

namespace MotorcycleRent.Application.Services;

public interface IMotorcycleServiceOrchestrator
{
    Task<Guid> CreateMotorcycleAsync(MotorcycleDto motorcycleDto, CancellationToken cancellationToken = default);
    Task<IEnumerable<MotorcycleDto>> ListMotorcyclesAsync(MotorcycleSearchCriteria criteria, CancellationToken cancellationToken = default);
    Task UpdateMotorcyclePlateAsync(UpdateMotorcyclePlateDto plateUpdateDto, CancellationToken cancellationToken = default);
    Task DeleteMotorcycleAsync(string motorcyclePlate, CancellationToken cancellationToken = default);
}
