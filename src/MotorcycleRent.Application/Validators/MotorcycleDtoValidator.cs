namespace MotorcycleRent.Application.Validators;

public sealed class MotorcycleDtoValidator : AbstractValidator<MotorcycleDto>
{
    public MotorcycleDtoValidator()
    {
        RuleFor(m => m.Model)
            .NotEmpty().WithMessage(m => $"{nameof(m.Model)} name is required")
            .MaximumLength(50).WithMessage("Model can't be longer than 50 characters");

        RuleFor(m => m.Year)
            .NotEmpty().WithMessage(m => $"{nameof(m.Year)} is required")
            .InclusiveBetween(1900, DateTime.UtcNow.Year).WithMessage("Year must be between 1900 and current year");

        RuleFor(m => m.Plate)
            .NotEmpty().WithMessage(m => $"{nameof(m.Plate)} is required");
    }
}
