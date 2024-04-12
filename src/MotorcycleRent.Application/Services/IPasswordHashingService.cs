namespace MotorcycleRent.Application.Interfaces;

public interface IPasswordHashingService
{
    string HashPassword(string? password);
    bool VerifyPassword(string? providedPassword, string? hashedPassword);
}
