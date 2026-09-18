namespace PulseDelivery.Shared.Authorization;

public static class RolePermissions
{
    public static readonly Dictionary<string, string[]> PermissionsByRole = new()
    {
        ["Admin"] =
        [
            Permissions.AdminAccess,
            Permissions.ReadAccess,
            Permissions.WriteAccess
        ],

        ["Manager"] =
        [
            Permissions.ReadAccess,
            Permissions.WriteAccess
        ],

        ["User"] =
        [
            Permissions.ReadAccess
        ]
    };
}