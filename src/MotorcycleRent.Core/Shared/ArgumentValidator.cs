namespace MotorcycleRent.Core.Shared;

public static class ArgumentValidator
{
    public static void ThrowIfNullOrEmpty(string? argument, string? paramName = null)
    {
        if (string.IsNullOrEmpty(argument))
        {
            throw new ArgumentNullException($"{paramName ?? "Argument"} cannot be null or empty", paramName);
        }
    }

    public static void ThrowIfNegative<T>(T argument, string? paramName = null) where T : INumber<T>
    {
        if (argument < T.Zero)
        {
            throw new ArgumentException($"{paramName ?? "Argument"} cannot be negative");
        }
    }

    public static void ThrowIfZeroOrNegative<T>(T argument, string? paramName = null) where T : INumber<T>
    {
        if (argument <= T.Zero)
        {
            throw new ArgumentException($"{paramName ?? "Argument"} cannot be zero or negative");
        }
    }

    public static void ThrowIfZero<T>(T argument, string? paramName = null) where T : INumber<T>
    {
        if (argument == T.Zero)
        {
            throw new ArgumentException($"{paramName ?? "Argument"} cannot be zero");
        }
    }

    public static void ThrowIfNullOrDefault<T>(T obj, string? paramName = null)
    {
        if (obj == null || obj.Equals(default(T)))
        {
            throw new ArgumentNullException($"{paramName ?? "Argument"} cannot be null or default");
        }
    }
}
