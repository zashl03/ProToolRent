using FluentValidation;
using ProToolRent.Application.Commands.CreateOrder;

namespace ProToolRent.Application.Commands.CreateOrderItem;

public class CreateOrderItemCommandValidator : AbstractValidator<CreateOrderItemCommand>
{
    public CreateOrderItemCommandValidator()
    {
        RuleFor(x => x.Quantity)
            .NotEmpty().WithMessage("Quantity is required")
            .GreaterThan(0).WithMessage("Quantity must be more than 0");
    }
}
