using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.API.Entities;

public class Restaurant
{
    [BsonId] 
    [BsonRepresentation(BsonType.ObjectId)] 
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    
    
   
    public List<MenuCategory> MenuCategories { get; set; } = new();
}

public class MenuCategory
{
    public string CategoryName { get; set; } = string.Empty; 
    public List<Product> Products { get; set; } = new();
}

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}