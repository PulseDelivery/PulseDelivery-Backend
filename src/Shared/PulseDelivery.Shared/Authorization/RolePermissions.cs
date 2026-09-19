namespace PulseDelivery.Shared.Authorization; 
 
public static class RolePermissions 
{ 
    public static readonly Dictionary<string, string[]> PermissionsByRole = new() 
    { 
        // 1. PULSEDELIVERY ADMIN (Has full access)
        ["PlatformAdmin"] = 
        [ 
            Permissions.SystemAdmin, 
            Permissions.CatalogRead, 
            Permissions.CatalogWrite, 
            Permissions.OrderCreate, 
            Permissions.OrderRead, 
            Permissions.OrderUpdate, 
            Permissions.DeliveryRead, 
            Permissions.DeliveryUpdate 
        ], 
 
        // 2. RESTAURANT OWNER OR MANAGER (Manages catalog and orders)
        ["RestaurantOwner"] = 
        [ 
            Permissions.CatalogRead, 
            Permissions.CatalogWrite, // Adds/removes products and updates stock
            Permissions.OrderRead,    // Views orders
            Permissions.OrderUpdate   // Updates order status
        ], 
 
        // 3. RESTAURANT EMPLOYEE (Manages orders only, cannot modify the menu)
        ["RestaurantEmployee"] = 
        [ 
            Permissions.CatalogRead,
            Permissions.OrderRead,    // Views incoming orders
            Permissions.OrderUpdate   // Changes order status to "Preparing"
        ], 
 
        // 4. COURIER (Manages assigned deliveries only)
        ["Courier"] = 
        [ 
            Permissions.OrderRead, 
            Permissions.DeliveryRead, 
            Permissions.DeliveryUpdate // Changes status to "On the Way" or "Delivered"
        ], 
 
        // 5. CUSTOMER (End user of the application)
        ["Customer"] = 
        [ 
            Permissions.CatalogRead,  // Lists restaurants and menus
            Permissions.OrderCreate,  // Confirms the cart and places an order
            Permissions.OrderRead     // Views their own order history
        ] 
    };
}