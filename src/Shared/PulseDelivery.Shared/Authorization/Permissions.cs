namespace PulseDelivery.Shared.Authorization;

public static class Permissions
{
    // System Administrator (Platform Owner)
    public const string SystemAdmin = "System.Admin"; 

    // Catalog and Menu Operations
    public const string CatalogRead = "Catalog.Read";       
    public const string CatalogWrite = "Catalog.Write";     
    
    // Order Operations
    public const string OrderCreate = "Order.Create";       
    public const string OrderRead = "Order.Read";           
    public const string OrderUpdate = "Order.Update";      
    
    // Delivery Operations 
    public const string DeliveryRead = "Delivery.Read";    
    public const string DeliveryUpdate = "Delivery.Update"; 
}
