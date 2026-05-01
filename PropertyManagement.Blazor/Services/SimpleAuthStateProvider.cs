using System.Security.Claims;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using PropertyManagement.Blazor.Models;

namespace PropertyManagement.Blazor.Services;

public sealed class SimpleAuthStateProvider(IJSRuntime jsRuntime, HttpClient httpClient) : AuthenticationStateProvider
{
    private const string StorageKey = "pm.admin.user";
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private static readonly AuthenticationState AnonymousState = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var storedUserJson = await jsRuntime.InvokeAsync<string?>("pmAuth.getUser", StorageKey);

            if (string.IsNullOrWhiteSpace(storedUserJson))
            {
                return AnonymousState;
            }

            var storedUser = ParseStoredUser(storedUserJson);
            if (string.IsNullOrWhiteSpace(storedUser.AccessToken) || storedUser.ExpiresAt <= DateTime.UtcNow)
            {
                await ClearStoredSessionAsync();
                return AnonymousState;
            }

            ApplyBearerToken(storedUser.AccessToken);
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
        var response = await httpClient.PostAsJsonAsync("api/auth/login", new LoginRequestModel
        {
            UserName = username?.Trim() ?? string.Empty,
            Password = password ?? string.Empty
        });

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var login = await response.Content.ReadFromJsonAsync<LoginResponseModel>(SerializerOptions);
        if (login is null || string.IsNullOrWhiteSpace(login.AccessToken))
        {
            return false;
        }

        var storedUser = new StoredUser(login.UserName, login.Role, login.TenantId, login.DisplayName, login.AccessToken, login.ExpiresAt);
        await jsRuntime.InvokeVoidAsync("pmAuth.setUser", StorageKey, JsonSerializer.Serialize(storedUser, SerializerOptions));
        ApplyBearerToken(storedUser.AccessToken);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(CreatePrincipal(storedUser))));
        return true;
    }

    public async Task SignOutAsync()
    {
        await ClearStoredSessionAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(AnonymousState));
    }

    public async Task<string?> GetAccessTokenAsync()
    {
        var storedUserJson = await jsRuntime.InvokeAsync<string?>("pmAuth.getUser", StorageKey);
        if (string.IsNullOrWhiteSpace(storedUserJson))
        {
            return null;
        }

        var storedUser = ParseStoredUser(storedUserJson);
        return storedUser.ExpiresAt > DateTime.UtcNow ? storedUser.AccessToken : null;
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
        }

        return new StoredUser(string.Empty, string.Empty, null, string.Empty, string.Empty, DateTime.MinValue);
    }

    private async Task ClearStoredSessionAsync()
    {
        httpClient.DefaultRequestHeaders.Authorization = null;
        await jsRuntime.InvokeVoidAsync("pmAuth.clearUser", StorageKey);
    }

    private void ApplyBearerToken(string token)
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private sealed record StoredUser(string UserName, string Role, int? TenantId, string DisplayName, string AccessToken, DateTime ExpiresAt);
}
