namespace MotorcycleRent.Application.Validators;

public sealed class MotorcycleRentalDtoValidator : AbstractValidator<MotorcycleRentalDto>
{
    public MotorcycleRentalDtoValidator()
    {
        RuleFor(x => x.RentalPeriod)
            .Must(x => x.NumberOfDays() >= 1)
            .WithMessage("At least one day must be selected");
    }
}
