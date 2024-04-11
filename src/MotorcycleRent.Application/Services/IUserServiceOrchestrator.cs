using MotorcycleRent.Application.Models.Dtos;

namespace MotorcycleRent.Application.Services;

public interface IUserServiceOrchestrator
{
    Task CreateAdministratorAsync(AdministratorDto administratorDto, CancellationToken cancellationToken = default);
    Task CreateDeliveryPartnerAsync(DeliveryPartnerDto deliveryPartnerDto, CancellationToken cancellationToken = default);
    Task<JwtToken> AuthenticateAsync(UserDto user, CancellationToken cancellationToken = default);
}
