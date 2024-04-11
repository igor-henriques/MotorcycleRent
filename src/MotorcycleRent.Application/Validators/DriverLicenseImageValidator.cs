namespace MotorcycleRent.Application.Validators;

public sealed class DriverLicenseImageValidator : AbstractValidator<IFormFile>
{
    public DriverLicenseImageValidator(IOptions<DriverLicenseOptions> options)
    {
        RuleFor(x => x.Length)
            .LessThanOrEqualTo(options.Value.MaxBytesSize);

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .Must(x => options.Value.AllowedImageTypes.Contains(x.Split('/')[1]))
            .WithMessage($"Image must be {string.Join(" or ", options.Value.AllowedImageTypes)} format");
    }
}
