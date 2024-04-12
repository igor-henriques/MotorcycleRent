namespace MotorcycleRent.Application.Services;

/// <summary>
/// Orchestrates operations related to driver licenses for delivery partners.
/// </summary>
public interface IDriverLicenseServiceOrchestrator
{
    /// <summary>
    /// Creates a new driver license for a delivery partner.
    /// </summary>
    /// <param name="driverLicenseDto">DTO containing the driver license data.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <exception cref="InvalidOperationException">Thrown if the delivery partner does not exist or a driver license already exists.</exception>
    /// <remarks>
    /// This method first retrieves the delivery partner by email, checks if the driver license already exists,
    /// and if not, proceeds to create and update the driver license record for the user. Images associated with
    /// the driver license are handled via an external service.
    /// </remarks>
    Task CreateDriverLicense(DriverLicenseDto driverLicenseDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing driver license for a delivery partner.
    /// </summary>
    /// <param name="driverLicenseDto">DTO containing the new driver license data.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <exception cref="InvalidOperationException">Thrown if the delivery partner or their existing driver license cannot be found.</exception>
    /// <remarks>
    /// This method fetches the existing driver license based on the driver license ID, updates it with new data,
    /// and saves the changes. If the driver license image is updated, it is re-uploaded and the URL is updated.
    /// </remarks>
    Task UpdateDriverLicense(DriverLicenseDto driverLicenseDto, CancellationToken cancellationToken = default);
}