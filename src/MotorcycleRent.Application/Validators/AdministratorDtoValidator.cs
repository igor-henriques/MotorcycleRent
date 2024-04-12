namespace MotorcycleRent.Application.Validators;

public sealed class AdministratorDtoValidator : AbstractValidator<AdministratorDto>
{
    public AdministratorDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}
