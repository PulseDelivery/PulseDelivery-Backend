namespace Order.Application.Strategies;

// 1. No Discount
public class NoDiscountStrategy : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal totalAmount) => totalAmount;
}

// 2. Percentage Discount (e.g., 20% Welcome Discount)
public class PercentageDiscountStrategy : IDiscountStrategy
{
    private readonly decimal _percentage;

    public PercentageDiscountStrategy(decimal percentage) => _percentage = percentage;

    public decimal ApplyDiscount(decimal totalAmount)
    {
        return totalAmount - (totalAmount * (_percentage / 100));
    }
}

// 3. Student Discount (Fixed 15% Discount)
public class StudentDiscountStrategy : IDiscountStrategy
{
    public decimal ApplyDiscount(decimal totalAmount)
    {
        return totalAmount - (totalAmount * 0.15m);
    }
}