namespace MotorcycleRent.Application.Services;

/// <summary>
/// Orchestrates user-related operations such as creation and authentication.
/// </summary>
public interface IUserServiceOrchestrator
{
    /// <summary>
    /// Asynchronously creates a new administrator in the system.
    /// </summary>
    /// <param name="administratorDto">The data transfer object containing the administrator details.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to request cancellation of the operation.</param>
    /// <exception cref="EntityCreationException">Thrown if the administrator could not be created.</exception>
    /// <remarks>
    /// This method maps the administrator DTO to an Administrator entity, hashes the password, and attempts to create a new administrator in the repository.
    /// If creation fails, it logs the failure and throws an EntityCreationException.
    /// </remarks>
    Task CreateAdministratorAsync(AdministratorDto administratorDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously creates a new delivery partner in the system.
    /// </summary>
    /// <param name="deliveryPartnerDto">The data transfer object containing the delivery partner details.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to request cancellation of the operation.</param>
    /// <exception cref="InvalidOperationException">Thrown if the delivery partner already exists.</exception>
    /// <remarks>
    /// This method handles the creation of a delivery partner. It ensures the uniqueness of the National ID by creating an index and logs detailed information about the creation process.
    /// Exceptions for duplicate entries are caught and rethrown as InvalidOperationException.
    /// </remarks>
    Task CreateDeliveryPartnerAsync(DeliveryPartnerDto deliveryPartnerDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user and generates a JWT token if authentication is successful.
    /// </summary>
    /// <param name="userDto">The data transfer object containing user credentials.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to request cancellation of the operation.</param>
    /// <returns>A JwtToken object containing the new JWT token and its expiration details.</returns>
    /// <exception cref="InvalidCredentialsException">Thrown if the user cannot be authenticated.</exception>
    /// <remarks>
    /// This method attempts to authenticate a user by verifying the provided credentials against the stored ones. If successful, it generates a JWT token; otherwise, it logs the failure and throws an InvalidCredentialsException.
    /// </remarks>
    Task<JwtToken> AuthenticateAsync(UserDto user, CancellationToken cancellationToken = default);
}
