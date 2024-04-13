namespace MotorcycleRent.Application.Validators;

public sealed class DriverLicenseDtoValidator : AbstractValidator<DriverLicenseDto>
{
    public DriverLicenseDtoValidator(IOptions<DriverLicenseOptions> options)
    {
        RuleFor(x => x.DriverLicenseImage)
            .NotNull()
            .WithMessage("A driver license image is required")
            .SetValidator(new DriverLicenseImageValidator(options)!);

        RuleFor(x => x.DriverLicenseId)
            .NotEmpty()
            .Length(11);

        RuleFor(x => x.DriverLicenseType)
            .Must(x => x != EDriverLicenseType.Invalid)
            .WithMessage("Driver license must have category A included");
    }
}
