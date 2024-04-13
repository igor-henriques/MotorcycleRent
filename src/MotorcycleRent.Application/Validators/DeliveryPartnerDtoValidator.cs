namespace MotorcycleRent.Application.Validators;

public sealed class DeliveryPartnerDtoValidator : AbstractValidator<DeliveryPartnerDto>
{
    public DeliveryPartnerDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);

        RuleFor(x => x.FullName)
            .NotEmpty()
            .Must(x => x!.Contains(' '));

        RuleFor(x => x.BirthDate)
            .NotEmpty()
            .LessThan(DateTime.UtcNow.AddYears(-18));

        RuleFor(x => x.NationalId)
            .NotEmpty()
            .MaximumLength(15);
    }
}
