namespace MotorcycleRent.Application.Validators;

public sealed class PublicOrderIdDtoValidator : AbstractValidator<PublicOrderIdDto>
{
    public PublicOrderIdDtoValidator()
    {
        RuleFor(x => x.PublicOrderId)
            .NotEmpty()
            .Must(MatchCorrectShape!)
            .WithMessage("Invalid public order id");
    }

    private bool MatchCorrectShape(string arg)
    {
        return arg.Count(c => c is '-') == FriendlyIdGenerator.CHUNK_SIZE + 1;
    }
}
