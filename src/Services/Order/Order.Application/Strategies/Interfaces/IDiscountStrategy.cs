namespace Order.Application.Strategies;

public interface IDiscountStrategy
{
    decimal ApplyDiscount(decimal totalAmount);
}