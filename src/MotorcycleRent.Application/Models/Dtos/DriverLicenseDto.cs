namespace MotorcycleRent.Application.Models.Dtos;

public sealed record DriverLicenseDto : IDto
{
    public string? DriverLicenseId { get; init; }
    public EDriverLicenseType DriverLicenseType { get; init; }
    public IFormFile? DriverLicenseImage { get; init; }

    public static async ValueTask<DriverLicenseDto?> BindAsync(HttpContext context)
    {
        var form = await context.Request.ReadFormAsync();

        var driverLicenseImage = form.Files[nameof(DriverLicenseImage)];
        var driverLicenseId = form[nameof(DriverLicenseId)];
        var driverLicenseType = form[nameof(DriverLicenseType)];

        return new DriverLicenseDto
        {
            DriverLicenseId = driverLicenseId.ToString(),
            DriverLicenseType = Enum.Parse<EDriverLicenseType>(driverLicenseType.ToString() ?? string.Empty),
            DriverLicenseImage = driverLicenseImage
        };
    }
}
