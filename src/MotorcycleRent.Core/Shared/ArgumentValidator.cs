namespace MotorcycleRent.Core.Shared;

/// <summary>
/// Provides static methods for validating method arguments.
/// </summary>
public static class ArgumentValidator
{
    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the provided string argument is null or empty.
    /// </summary>
    /// <param name="argument">The string argument to validate.</param>
    /// <param name="paramName">Optional parameter name to include in the exception message. 
    /// If not provided, defaults to "Argument".</param>
    /// <exception cref="ArgumentNullException">Throws if the argument is null or empty.</exception>
    public static void ThrowIfNullOrEmpty(string? argument, string? paramName = null)
    {
        if (string.IsNullOrEmpty(argument))
        {
            throw new ArgumentNullException($"{paramName ?? "Argument"} cannot be null or empty", paramName);
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if the provided numeric argument is a negative value.
    /// The argument type must implement the <see cref="INumber{T}"/> interface, ensuring it is a numeric type.
    /// </summary>
    /// <typeparam name="T">The type of the argument, which must be a numeric type implementing INumber{T}.</typeparam>
    /// <param name="argument">The numeric argument to validate.</param>
    /// <param name="paramName">Optional parameter name to include in the exception message. 
    /// If not provided, defaults to "Argument".</param>
    /// <exception cref="ArgumentException">Thrown when the argument is negative.</exception>
    public static void ThrowIfNegative<T>(T argument, string? paramName = null) where T : INumber<T>
    {
        if (argument < T.Zero)
        {
            throw new ArgumentException($"{paramName ?? "Argument"} cannot be negative");
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if the provided argument is zero or negative.
    /// The argument type must implement the <see cref="INumber{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The type of the argument, which must be a numeric type implementing INumber{T}.</typeparam>
    /// <param name="argument">The argument to validate.</param>
    /// <param name="paramName">Optional parameter name to include in the exception message. 
    /// If not provided, defaults to "Argument".</param>
    /// <exception cref="ArgumentException">Throws if the argument is zero or negative.</exception>
    public static void ThrowIfZeroOrNegative<T>(T argument, string? paramName = null) where T : INumber<T>
    {
        if (argument <= T.Zero)
        {
            throw new ArgumentException($"{paramName ?? "Argument"} cannot be zero or negative");
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentException"/> if the provided numeric argument is zero.
    /// The argument type must implement the <see cref="INumber{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The type of the argument, which must be a numeric type implementing INumber{T}.</typeparam>
    /// <param name="argument">The argument to validate.</param>
    /// <param name="paramName">Optional parameter name to include in the exception message. 
    /// If not provided, defaults to "Argument".</param>
    /// <exception cref="ArgumentException">Throws if the argument is zero.</exception>
    public static void ThrowIfZero<T>(T argument, string? paramName = null) where T : INumber<T>
    {
        if (argument == T.Zero)
        {
            throw new ArgumentException($"{paramName ?? "Argument"} cannot be zero");
        }
    }

    /// <summary>
    /// Throws an <see cref="ArgumentNullException"/> if the provided argument is null or the default value for its type.
    /// This is a generic method that works with any type that can be checked for default values.
    /// </summary>
    /// <typeparam name="T">The type of the argument.</typeparam>
    /// <param name="obj">The argument to validate.</param>
    /// <param name="paramName">Optional parameter name to include in the exception message. 
    /// If not provided, defaults to "Argument".</param>
    /// <exception cref="ArgumentNullException">Throws if the argument is null or the default value for its type.</exception>
    public static void ThrowIfNullOrDefault<T>(T obj, string? paramName = null)
    {
        if (obj == null || obj.Equals(default(T)))
        {
            throw new ArgumentNullException($"{paramName ?? "Argument"} cannot be null or default");
        }
    }
}
