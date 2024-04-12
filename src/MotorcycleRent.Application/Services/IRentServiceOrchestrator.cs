namespace MotorcycleRent.Application.Services;

public interface IRentServiceOrchestrator
{
    Task<RentPriceDto> CreateMotorcycleRentAsync(MotorcycleRentDto motorcycleRentDto, CancellationToken cancellationToken = default);
    RentPriceDto PeekRentPrice(MotorcycleRentDto motorcycleRentDto);
    Task ReturnMotorcycleRentAsync(CancellationToken cancellationToken = default);
}