using System.Security.Claims;

namespace PropertyManagement.API.Security;

public static class UserAccessExtensions
{
    public static bool IsTenantUser(this ClaimsPrincipal user) =>
        user.IsInRole(DemoAccountStore.TenantRole);

    public static bool IsContractorUser(this ClaimsPrincipal user) =>
        user.IsInRole(DemoAccountStore.ContractorRole);

    public static int? GetTenantId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(DemoAccountStore.TenantIdClaimType)?.Value;
        return int.TryParse(value, out var tenantId) ? tenantId : null;
    }
}
