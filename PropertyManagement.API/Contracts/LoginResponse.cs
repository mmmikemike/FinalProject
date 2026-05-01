namespace PropertyManagement.API.Contracts;

public record LoginResponse(
    string UserName,
    string DisplayName,
    string Role,
    int? TenantId,
    string AccessToken,
    DateTime ExpiresAt);
