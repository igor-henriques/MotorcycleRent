namespace MotorcycleRent.Application.Validators;

public sealed class UpdateOrderStatusDtoValidator : AbstractValidator<UpdateOrderStatusDto>
{
    public UpdateOrderStatusDtoValidator()
    {
        RuleFor(x => x.PublicOrderId)
            .SetValidator(new PublicOrderIdDtoValidator()!);
    }
}
