namespace PropertyManagement.API.Security;

public sealed record DemoAccount(string UserName, string Password, string Role, int? TenantId, string DisplayName);
