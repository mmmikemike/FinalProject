using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace PropertyManagement.Blazor.Services;

public sealed class SimpleAuthStateProvider(IJSRuntime jsRuntime) : AuthenticationStateProvider
{
    private const string StorageKey = "pm.admin.user";
    private static readonly AuthenticationState AnonymousState = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var storedUser = await jsRuntime.InvokeAsync<string?>("pmAuth.getUser", StorageKey);

            if (string.IsNullOrWhiteSpace(storedUser))
            {
                return AnonymousState;
            }

            return new AuthenticationState(CreatePrincipal(storedUser));
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
        if (!string.Equals(username?.Trim(), "ADMIN", StringComparison.Ordinal) ||
            !string.Equals(password, "ADMIN", StringComparison.Ordinal))
        {
            return false;
        }

        var normalizedUser = "ADMIN";
        await jsRuntime.InvokeVoidAsync("pmAuth.setUser", StorageKey, normalizedUser);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(CreatePrincipal(normalizedUser))));
        return true;
    }

    public async Task SignOutAsync()
    {
        await jsRuntime.InvokeVoidAsync("pmAuth.clearUser", StorageKey);
        NotifyAuthenticationStateChanged(Task.FromResult(AnonymousState));
    }

    private static ClaimsPrincipal CreatePrincipal(string username)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Administrator")
        ], "SessionAuth");

        return new ClaimsPrincipal(identity);
    }
}
