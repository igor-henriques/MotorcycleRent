namespace MotorcycleRent.Infrastructure.Services;

public sealed class PasswordHashingService : IPasswordHashingService
{
    public string HashPassword(string? password)
    {
        ArgumentValidator.ThrowIfNullOrEmpty(password, nameof(password));

        var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

        return hashedPassword;
    }

    public bool VerifyPassword(string? hashedPassword, string? providedPassword)
    {
        ArgumentValidator.ThrowIfNullOrEmpty(hashedPassword, nameof(hashedPassword));
        ArgumentValidator.ThrowIfNullOrEmpty(providedPassword, nameof(providedPassword));

        return BCrypt.Net.BCrypt.Verify(providedPassword, hashedPassword);
    }
}
