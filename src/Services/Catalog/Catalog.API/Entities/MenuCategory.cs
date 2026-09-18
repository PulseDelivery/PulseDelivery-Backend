public class MenuCategory
{
    public string CategoryName { get; set; } = string.Empty; 
    public List<Product> Products { get; set; } = new();
}