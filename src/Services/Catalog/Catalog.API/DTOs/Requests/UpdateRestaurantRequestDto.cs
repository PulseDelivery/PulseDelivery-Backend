namespace Catalog.API.DTOs.Requests;

public class UpdateRestaurantRequestDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public List<MenuCategoryRequestDto> MenuCategories { get; set; } = new();
}