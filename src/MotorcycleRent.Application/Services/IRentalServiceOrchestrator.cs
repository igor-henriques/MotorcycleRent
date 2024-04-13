namespace MotorcycleRent.Application.Services;

/// <summary>
/// Orchestrates rental services for motorcycles, handling the creation, management, and finalization of rentals.
/// </summary>
public interface IRentalServiceOrchestrator
{
    /// <summary>
    /// Creates a new motorcycle rent asynchronously.
    /// </summary>
    /// <param name="MotorcycleRentalDto">DTO containing the details for the new motorcycle rent.</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <returns>A DTO containing details about the cost and period of the rent.</returns>
    /// <exception cref="OnGoingRentalException">Thrown if there is an ongoing rent that conflicts with the new rent request.</exception>
    /// <exception cref="EntityCreationException">Thrown if the new rent could not be created in the database.</exception>
    /// <exception cref="InternalErrorException">Thrown if there is an error updating the motorcycle status.</exception>
    Task<RentalPriceDto> RentMotorcycleAsync(MotorcycleRentalDto MotorcycleRentalDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates the rental price for a motorcycle without creating a rental record.
    /// </summary>
    /// <param name="MotorcycleRentalDto">DTO containing the parameters needed to calculate the rent.</param>
    /// <returns>A DTO that details the calculated rent costs and period.</returns>
    /// <exception cref="InternalErrorException">Thrown if no suitable rent cost calculator service is available.</exception>
    RentalPriceDto PeekRentalPrice(MotorcycleRentalDto MotorcycleRentalDto);

    /// <summary>
    /// Finalizes the ongoing motorcycle rent for the current user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// <exception cref="InvalidOperationException">Thrown if there is no ongoing rent to finalize.</exception>
    Task ReturnMotorcycleRentalAsync(CancellationToken cancellationToken = default);
}