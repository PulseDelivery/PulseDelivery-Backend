using MediatR;
using PulseDelivery.Shared.DTOs;

namespace Order.Application.Commands.CreateOrder;

// Returns ResponseDto<Guid> (the Order ID)
public class CreateOrderCommand : IRequest<ResponseDto<Guid>>
{
    public string CustomerId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RestaurantId { get; set; } = string.Empty;
    public decimal CartTotal { get; set; }
    
    // Specifies the discount type and value
    public string DiscountType { get; set; } = "None"; // None, Percentage, Student
    public decimal? DiscountValue { get; set; }
}