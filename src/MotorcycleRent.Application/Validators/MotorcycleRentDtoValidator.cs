namespace MotorcycleRent.Application.Validators;

public sealed class MotorcycleRentDtoValidator : AbstractValidator<MotorcycleRentDto>
{
    public MotorcycleRentDtoValidator()
    {
        RuleFor(x => x.RentPeriod)
            .Must(x => x.NumberOfDays() >= 1)
            .WithMessage("At least one day must be selected");
    }
}
