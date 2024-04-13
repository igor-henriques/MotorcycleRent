namespace MotorcycleRent.Application.Services;

/// <summary>
/// Orchestrates motorcycle-related services including creation, updating, listing, and deleting of motorcycles.
/// </summary>
public interface IMotorcycleServiceOrchestrator
{
    /// <summary>
    /// Creates a new motorcycle asynchronously.
    /// </summary>
    /// <param name="motorcycleDto">The motorcycle DTO containing the data to create a new motorcycle.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The ID of the newly created motorcycle.</returns>
    /// <exception cref="InvalidOperationException">Thrown if there is a duplicate plate exception.</exception>
    Task<Guid> CreateMotorcycleAsync(MotorcycleDto motorcycleDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a list of motorcycles based on provided search criteria.
    /// </summary>
    /// <param name="criteria">The search criteria to filter motorcycles.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A list of motorcycle DTOs matching the criteria.</returns>
    Task<IEnumerable<MotorcycleDto>> ListMotorcyclesAsync(MotorcycleSearchCriteria criteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the plate number of an existing motorcycle.
    /// </summary>
    /// <param name="plateUpdateDto">The DTO containing the old and new plate numbers.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="InvalidOperationException">Thrown if the motorcycle does not exist or the new plate already exists.</exception>
    Task UpdateMotorcyclePlateAsync(UpdateMotorcyclePlateDto plateUpdateDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a motorcycle by its plate number.
    /// </summary>
    /// <param name="motorcyclePlate">The plate number of the motorcycle to delete.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="InvalidOperationException">Thrown if the motorcycle cannot be deleted due to existing rent records or other constraints.</exception>
    Task DeleteMotorcycleAsync(string motorcyclePlate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the status of a motorcycle.
    /// </summary>
    /// <param name="plateUpdateDto">The DTO containing the motorcycle plate and the new status.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <exception cref="InvalidOperationException">Thrown if the motorcycle plate is invalid or an error occurs during the update.</exception>
    Task UpdateMotorcycleStatusAsync(UpdateMotorcycleStatusDto plateUpdateDto, CancellationToken cancellationToken = default);
}
