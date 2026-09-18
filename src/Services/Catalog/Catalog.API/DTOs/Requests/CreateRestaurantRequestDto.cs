namespace Catalog.API.DTOs.Requests;

public class CreateRestaurantRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public List<MenuCategoryRequestDto> MenuCategories { get; set; } = new();
}

public class MenuCategoryRequestDto
{
    public string CategoryName { get; set; } = string.Empty;
    public List<ProductRequestDto> Products { get; set; } = new();
}

public class ProductRequestDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}