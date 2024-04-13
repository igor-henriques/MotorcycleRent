namespace MotorcycleRent.Core.Shared;

public static class MethodExtensions
{
    private static readonly JsonSerializerOptions _defaultSerializer = new() { WriteIndented = true };

    public static string AsJson<T>(this T obj, JsonSerializerOptions? serializer = null) where T : class
    {
        serializer ??= _defaultSerializer;
        return JsonSerializer.Serialize(obj, serializer);
    }
}