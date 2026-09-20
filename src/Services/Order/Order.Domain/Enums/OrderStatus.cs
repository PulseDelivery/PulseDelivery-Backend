namespace Order.Domain.Enums;

public enum OrderStatus
{
    Pending = 1,        // Pending (Customer placed the order, awaiting confirmation)
    Preparing = 2,      // Preparing (Restaurant accepted the order)
    OutForDelivery = 3, // Out for Delivery (Courier picked up the order)
    Delivered = 4,      // Delivered
    Canceled = 5        // Canceled
}