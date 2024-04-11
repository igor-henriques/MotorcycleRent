using Microsoft.AspNetCore.Http;

namespace MotorcycleRent.Application.Models.Dtos;

public sealed record DriverLicenseDto
{
    public string? DeliveryPartnerEmail { get; init; }
    public string? DriverLicenseId { get; init; }
    public EDriverLicenseType DriverLicenseType { get; init; }
    public IFormFile? DriverLicenseImage { get; init; }

    public static async ValueTask<DriverLicenseDto?> BindAsync(HttpContext context)
    {
        var form = await context.Request.ReadFormAsync();

        var driverLicenseImage = form.Files["DriverLicenseImage"];
        var deliveryPartnerEmail = form["DeliveryPartnerEmail"];
        var driverLicenseId = form["DriverLicenseId"];
        var driverLicenseType = form["DriverLicenseType"];

        return new DriverLicenseDto
        {
            DeliveryPartnerEmail = deliveryPartnerEmail.ToString(),
            DriverLicenseId = driverLicenseId.ToString(),
            DriverLicenseType = Enum.Parse<EDriverLicenseType>(driverLicenseType),
            DriverLicenseImage = driverLicenseImage
        };
    }
}
