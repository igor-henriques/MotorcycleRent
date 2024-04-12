namespace MotorcycleRent.Application.Validators;

public sealed class DriverLicenseDtoValidator : AbstractValidator<DriverLicenseDto>
{
    public DriverLicenseDtoValidator(IOptions<DriverLicenseOptions> options)
    {
        RuleFor(x => x.DriverLicenseId)
            .NotEmpty()
            .Length(11);

        RuleFor(x => x.DriverLicenseType)
            .Must(x => x != EDriverLicenseType.Invalid)
            .WithMessage("Driver license must have category A included");

        RuleFor(x => x.DriverLicenseImage)
            .SetValidator(new DriverLicenseImageValidator(options)!);
    }
}
