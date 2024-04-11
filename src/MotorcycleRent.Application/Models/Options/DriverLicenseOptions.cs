namespace MotorcycleRent.Application.Models.Options;

public sealed record DriverLicenseOptions
{
    public int MaxMbSize { get; init; }
    public int MaxBytesSize => MaxMbSize * 1024 * 1024;
    public string[] AllowedImageTypes { get; init; } = [];
}
