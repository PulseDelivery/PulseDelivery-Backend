using FluentValidation;

namespace Order.Application.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer ID cannot be empty.");

        RuleFor(x => x.RestaurantId)
            .NotEmpty().WithMessage("Restaurant ID cannot be empty.");

        RuleFor(x => x.CartTotal)
            .GreaterThan(0).WithMessage("Cart total must be greater than 0.");

        RuleFor(x => x.DiscountType)
            .Must(type => type == "None" || type == "Percentage" || type == "Student")
            .WithMessage("Invalid discount type. It must be None, Percentage, or Student.");
    }
}