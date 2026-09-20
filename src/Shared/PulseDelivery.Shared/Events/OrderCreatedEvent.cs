public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string RestaurantId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}