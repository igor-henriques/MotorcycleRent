namespace MotorcycleRent.Infrastructure.Services;

/// <summary>
/// Provides services for hashing and verifying passwords using BCrypt.
/// </summary>
public sealed class PasswordHashingService : IPasswordHashingService
{
    /// <summary>
    /// Hashes a password using BCrypt hashing algorithm.
    /// </summary>
    /// <param name="password">The password to hash. Cannot be null or empty.</param>
    /// <returns>A hashed version of the input password.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input password is null or empty.</exception>
    public string HashPassword(string? password)
    {
        ArgumentValidator.ThrowIfNullOrEmpty(password, nameof(password));

        var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

        return hashedPassword;
    }

    /// <summary>
    /// Verifies that a provided password matches a hashed password.
    /// </summary>
    /// <param name="hashedPassword">The hashed password to verify against. Cannot be null or empty.</param>
    /// <param name="providedPassword">The password provided for verification. Cannot be null or empty.</param>
    /// <returns>True if the provided password matches the hashed password; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when either the hashed password or provided password is null or empty.</exception>
    public bool VerifyPassword(string? hashedPassword, string? providedPassword)
    {
        ArgumentValidator.ThrowIfNullOrEmpty(hashedPassword, nameof(hashedPassword));
        ArgumentValidator.ThrowIfNullOrEmpty(providedPassword, nameof(providedPassword));

        return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
    }
}
