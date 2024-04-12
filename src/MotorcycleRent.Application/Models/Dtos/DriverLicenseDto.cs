namespace MotorcycleRent.Application.Models.Dtos;

public sealed record DriverLicenseDto : IDto
{
    public string? DriverLicenseId { get; init; }
    public EDriverLicenseType DriverLicenseType { get; init; }
    public IFormFile? DriverLicenseImage { get; init; }

    public static async ValueTask<DriverLicenseDto?> BindAsync(HttpContext context)
    {
        var form = await context.Request.ReadFormAsync();

        var driverLicenseImage = form.Files["DriverLicenseImage"];
        var driverLicenseId = form["DriverLicenseId"];
        var driverLicenseType = form["DriverLicenseType"];

        return new DriverLicenseDto
        {
            DriverLicenseId = driverLicenseId.ToString(),
            DriverLicenseType = Enum.Parse<EDriverLicenseType>(driverLicenseType.ToString() ?? string.Empty),
            DriverLicenseImage = driverLicenseImage
        };
    }
}
