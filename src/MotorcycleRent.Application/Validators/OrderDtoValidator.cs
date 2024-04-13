namespace MotorcycleRent.Application.Validators;

public sealed class OrderDtoValidator : AbstractValidator<OrderDto>
{
    public OrderDtoValidator()
    {
        RuleFor(x => x.DeliveryCost)
            .NotEmpty()
            .WithMessage("An order must have a cost");
    }
}
