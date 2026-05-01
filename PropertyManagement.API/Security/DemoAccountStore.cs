namespace PropertyManagement.API.Security;

public static class DemoAccountStore
{
    public const string AdministratorRole = "Administrator";
    public const string StaffRole = "Staff";
    public const string ContractorRole = "Contractor";
    public const string TenantRole = "Tenant";
    public const string TenantIdClaimType = "tenant_id";

    private static readonly IReadOnlyDictionary<string, DemoAccount> Accounts =
        new Dictionary<string, DemoAccount>(StringComparer.OrdinalIgnoreCase)
        {
            ["ADMIN"] = new("ADMIN", "ADMIN", AdministratorRole, null, "Admin"),
            ["STAFF"] = new("STAFF", "STAFF", StaffRole, null, "Staff"),
            ["CONTRACTOR"] = new("CONTRACTOR", "CONTRACTOR", ContractorRole, null, "Contractor"),
            ["TENANT1"] = new("TENANT1", "TENANT1", TenantRole, 1, "John Doe"),
            ["TENANT2"] = new("TENANT2", "TENANT2", TenantRole, 2, "Jane Smith"),
            ["TENANT3"] = new("TENANT3", "TENANT3", TenantRole, 3, "Mike Jones"),
            ["TENANT4"] = new("TENANT4", "TENANT4", TenantRole, 4, "Sarah Wilson"),
            ["TENANT5"] = new("TENANT5", "TENANT5", TenantRole, 5, "Alex Brown"),
            ["TENANT6"] = new("TENANT6", "TENANT6", TenantRole, 6, "Chris Davis")
        };

    public static bool TryValidate(string? userName, string? password, out DemoAccount account)
    {
        var normalizedUserName = userName?.Trim() ?? string.Empty;
        if (Accounts.TryGetValue(normalizedUserName, out var foundAccount) &&
            string.Equals(foundAccount.Password, password, StringComparison.Ordinal))
        {
            account = foundAccount;
            return true;
        }

        account = default!;
        return false;
    }
}
