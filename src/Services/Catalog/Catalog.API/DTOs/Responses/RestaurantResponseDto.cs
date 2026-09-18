namespace Catalog.API.DTOs.Responses;

public class RestaurantResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public List<MenuCategoryResponseDto> MenuCategories { get; set; } = new();
}

public class MenuCategoryResponseDto
{
    public string CategoryName { get; set; } = string.Empty;
    public List<ProductResponseDto> Products { get; set; } = new();
}

public class ProductResponseDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}