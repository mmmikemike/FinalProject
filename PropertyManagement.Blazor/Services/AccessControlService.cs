using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace PropertyManagement.Blazor.Services;

public sealed class AccessControlService(AuthenticationStateProvider authenticationStateProvider)
{
    public const string AdministratorRole = "Administrator";
    public const string StaffRole = "Staff";
    public const string ContractorRole = "Contractor";
    public const string TenantRole = "Tenant";
    public const string TenantIdClaimType = "tenant_id";

    public async Task<UserAccessProfile> GetUserAsync()
    {
        var state = await authenticationStateProvider.GetAuthenticationStateAsync();
        return UserAccessProfile.FromPrincipal(state.User);
    }
}

public sealed record UserAccessProfile(string UserName, string Role, int? TenantId)
{
    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(UserName);
    public bool IsAdmin => Role == AccessControlService.AdministratorRole;
    public bool IsStaff => Role == AccessControlService.StaffRole;
    public bool IsContractor => Role == AccessControlService.ContractorRole;
    public bool IsTenant => Role == AccessControlService.TenantRole;
    public bool CanEditEverything => IsAdmin;

    public bool CanViewProperties => IsAdmin || IsStaff;
    public bool CanEditProperties => IsAdmin;

    public bool CanViewTenants => IsAdmin || IsStaff;
    public bool CanEditTenants => IsAdmin;

    public bool CanViewRentSchedules => IsAdmin || IsStaff || IsTenant;
    public bool CanEditRentSchedules => IsAdmin;

    public bool CanViewRentPayments => IsAdmin || IsStaff || IsTenant;
    public bool CanEditRentPayments => IsAdmin;

    public bool CanViewRentRecords => IsAdmin || IsStaff || IsTenant;
    public bool CanViewPropertyLedgers => IsAdmin || IsStaff;

    public bool CanViewMaintenance => IsAdmin || IsStaff || IsContractor;
    public bool CanEditMaintenance => IsAdmin || IsContractor;

    public bool CanViewInvoices => IsAdmin || IsStaff || IsContractor || IsTenant;
    public bool CanEditInvoices => IsAdmin || IsContractor;

    public bool CanViewApplications => IsAdmin || IsStaff;
    public bool CanEditApplications => IsAdmin;

    public static UserAccessProfile Anonymous { get; } = new(string.Empty, string.Empty, null);

    public static UserAccessProfile FromPrincipal(ClaimsPrincipal principal)
    {
        if (principal.Identity?.IsAuthenticated != true)
        {
            return Anonymous;
        }

        var tenantIdValue = principal.FindFirst(AccessControlService.TenantIdClaimType)?.Value;
        var tenantId = int.TryParse(tenantIdValue, out var parsedTenantId) ? parsedTenantId : (int?)null;

        return new UserAccessProfile(
            principal.Identity.Name ?? string.Empty,
            principal.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
            tenantId);
    }
}
