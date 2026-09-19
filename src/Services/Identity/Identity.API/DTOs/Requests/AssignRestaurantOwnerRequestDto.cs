namespace Identity.API.DTOs.Requests;

public class AssignRestaurantOwnerRequestDto
{
    public string UserId { get; set; } = string.Empty; 
    public string RestaurantId { get; set; } = string.Empty; 
}