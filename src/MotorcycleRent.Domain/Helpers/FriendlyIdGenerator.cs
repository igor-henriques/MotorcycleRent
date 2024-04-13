namespace MotorcycleRent.Domain.Helpers;

public static class FriendlyIdGenerator
{
    private const int BASE32_LENGTH = 24;
    public const int CHUNK_SIZE = 4;
    public const int FULL_ID_LENGTH = BASE32_LENGTH + CHUNK_SIZE + 1;

    private static readonly char[] Base62Chars =
        "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();

    public static string CreateFriendlyId(Guid guid)
    {
        var bytes = guid.ToByteArray();
        return ToBase62String(bytes);
    }

    private static string ToBase62String(byte[] bytes)
    {
        StringBuilder stringBuilder = new();
        var number = new BigInteger(bytes);

        // Ensure positive number for BigInteger by adding an extra zero byte if needed
        if (bytes[0] > 0x7f)
        {
            number = new BigInteger(bytes.Concat(new byte[] { 0 }).ToArray());
        }

        // Convert to Base62
        while (number > 0)
        {
            var remainder = number % 62;
            stringBuilder.Insert(0, Base62Chars[(int)remainder]);
            number /= 62;
        }

        // Padding to ensure uniform length
        while (stringBuilder.Length < BASE32_LENGTH)
        {
            stringBuilder.Insert(0, '0');
        }

        List<string> idChunks = stringBuilder
            .ToString()
            .ToUpper()
            .Chunk(CHUNK_SIZE)
            .Select(chunk => new string(chunk))
            .ToList();

        return string.Join('-', idChunks);
    }
}
