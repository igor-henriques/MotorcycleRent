namespace MotorcycleRent.Application.Exceptions;

public sealed class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException() : base("Authentication failed")
    {
        
    }
}
