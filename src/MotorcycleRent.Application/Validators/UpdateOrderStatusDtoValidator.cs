namespace MotorcycleRent.Application.Validators;

public sealed class UpdateOrderStatusDtoValidator : AbstractValidator<UpdateOrderStatusDto>
{
    public UpdateOrderStatusDtoValidator()
    {
        RuleFor(x => new PublicOrderIdDto() { PublicOrderId = x.PublicOrderId })
            .SetValidator(new PublicOrderIdDtoValidator()!);
    }
}
