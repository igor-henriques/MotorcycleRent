namespace MotorcycleRent.Application.Interfaces;

/// <summary>
/// Provides services for hashing and verifying passwords using BCrypt.
/// </summary>
public interface IPasswordHashingService
{
    /// <summary>
    /// Hashes a password using BCrypt hashing algorithm.
    /// </summary>
    /// <param name="password">The password to hash. Cannot be null or empty.</param>
    /// <returns>A hashed version of the input password.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input password is null or empty.</exception>
    string HashPassword(string? password);

    /// <summary>
    /// Verifies that a provided password matches a hashed password.
    /// </summary>
    /// <param name="hashedPassword">The hashed password to verify against. Cannot be null or empty.</param>
    /// <param name="providedPassword">The password provided for verification. Cannot be null or empty.</param>
    /// <returns>True if the provided password matches the hashed password; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either the hashed password or provided password is null or empty.</exception>
    bool VerifyPassword(string? providedPassword, string? hashedPassword);
}
