using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace PropertyManagement.Blazor.Services;

public sealed class SimpleAuthStateProvider(IJSRuntime jsRuntime) : AuthenticationStateProvider
{
    private const string StorageKey = "pm.admin.user";
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private static readonly AuthenticationState AnonymousState = new(new ClaimsPrincipal(new ClaimsIdentity()));
    private static readonly IReadOnlyDictionary<string, LoginAccount> Accounts = new Dictionary<string, LoginAccount>(StringComparer.Ordinal)
    {
        ["ADMIN"] = new("ADMIN", "ADMIN", AccessControlService.AdministratorRole, null, "Admin"),
        ["STAFF"] = new("STAFF", "STAFF", AccessControlService.StaffRole, null, "Staff"),
        ["CONTRACTOR"] = new("CONTRACTOR", "CONTRACTOR", AccessControlService.ContractorRole, null, "Contractor"),
        ["TENANT1"] = new("TENANT1", "TENANT1", AccessControlService.TenantRole, 1, "John Doe"),
        ["TENANT2"] = new("TENANT2", "TENANT2", AccessControlService.TenantRole, 2, "Jane Smith"),
        ["TENANT3"] = new("TENANT3", "TENANT3", AccessControlService.TenantRole, 3, "Mike Jones"),
        ["TENANT4"] = new("TENANT4", "TENANT4", AccessControlService.TenantRole, 4, "Sarah Wilson"),
        ["TENANT5"] = new("TENANT5", "TENANT5", AccessControlService.TenantRole, 5, "Alex Brown"),
        ["TENANT6"] = new("TENANT6", "TENANT6", AccessControlService.TenantRole, 6, "Chris Davis")
     };

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var storedUserJson = await jsRuntime.InvokeAsync<string?>("pmAuth.getUser", StorageKey);

            if (string.IsNullOrWhiteSpace(storedUserJson))
            {
                return AnonymousState;
            }

            return new AuthenticationState(CreatePrincipal(ParseStoredUser(storedUserJson)));
        }
        catch (JSException)
        {
            return AnonymousState;
        }
        catch (InvalidOperationException)
        {
            return AnonymousState;
        }
    }

    public async Task<bool> SignInAsync(string? username, string? password)
    {
        var normalizedUser = username?.Trim().ToUpperInvariant() ?? string.Empty;
        if (!Accounts.TryGetValue(normalizedUser, out var account) ||
            !string.Equals(password, account.Password, StringComparison.Ordinal))
        {
            return false;
        }

        var storedUser = new StoredUser(account.UserName, account.Role, account.TenantId, account.DisplayName);
        await jsRuntime.InvokeVoidAsync("pmAuth.setUser", StorageKey, JsonSerializer.Serialize(storedUser, SerializerOptions));

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(CreatePrincipal(storedUser))));
        return true;
    }

    public async Task SignOutAsync()
    {
        await jsRuntime.InvokeVoidAsync("pmAuth.clearUser", StorageKey);
        NotifyAuthenticationStateChanged(Task.FromResult(AnonymousState));
    }

    private static ClaimsPrincipal CreatePrincipal(StoredUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.DisplayName),
            new("username", user.UserName),
            new(ClaimTypes.Role, user.Role)
        };

        if (user.TenantId.HasValue)
        {
            claims.Add(new Claim(AccessControlService.TenantIdClaimType, user.TenantId.Value.ToString()));
        }

        var identity = new ClaimsIdentity(claims, "SessionAuth");

        return new ClaimsPrincipal(identity);
    }

    private static StoredUser ParseStoredUser(string storedUserJson)
    {
        try
        {
            var storedUser = JsonSerializer.Deserialize<StoredUser>(storedUserJson, SerializerOptions);
            if (storedUser is not null)
            {
                return storedUser;
            }
        }
        catch (JsonException)
        {
            if (Accounts.TryGetValue(storedUserJson.Trim().ToUpperInvariant(), out var account))
            {
                return new StoredUser(account.UserName, account.Role, account.TenantId, account.DisplayName);
            }
        }

        return new StoredUser("ADMIN", AccessControlService.AdministratorRole, null, "Admin");
    }

    private sealed record LoginAccount(string UserName, string Password, string Role, int? TenantId, string DisplayName);
    private sealed record StoredUser(string UserName, string Role, int? TenantId, string DisplayName);
}
