using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PropertyManagement.API.Security;

public sealed class DemoJwtTokenService(IConfiguration configuration)
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public (string Token, DateTime ExpiresAt) CreateToken(DemoAccount account)
    {
        var expiresAt = DateTime.UtcNow.AddHours(8);
        var claims = new Dictionary<string, object?>
        {
            ["sub"] = account.UserName,
            ["name"] = account.DisplayName,
            [ClaimTypes.Name] = account.DisplayName,
            [ClaimTypes.Role] = account.Role,
            ["role"] = account.Role,
            ["username"] = account.UserName,
            ["exp"] = new DateTimeOffset(expiresAt).ToUnixTimeSeconds()
        };

        if (account.TenantId.HasValue)
        {
            claims[DemoAccountStore.TenantIdClaimType] = account.TenantId.Value;
        }

        var header = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(new Dictionary<string, object>
        {
            ["alg"] = "HS256",
            ["typ"] = "JWT"
        }, SerializerOptions));
        var payload = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(claims, SerializerOptions));
        var signature = Sign($"{header}.{payload}");

        return ($"{header}.{payload}.{signature}", expiresAt);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var parts = token.Split('.');
        if (parts.Length != 3)
        {
            return null;
        }

        var expectedSignature = Sign($"{parts[0]}.{parts[1]}");
        if (!CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expectedSignature),
                Encoding.UTF8.GetBytes(parts[2])))
        {
            return null;
        }

        var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
        using var document = JsonDocument.Parse(payloadJson);
        var root = document.RootElement;

        if (!root.TryGetProperty("exp", out var expProperty) ||
            DateTimeOffset.FromUnixTimeSeconds(expProperty.GetInt64()) <= DateTimeOffset.UtcNow)
        {
            return null;
        }

        var claims = new List<Claim>();
        foreach (var property in root.EnumerateObject())
        {
            switch (property.Value.ValueKind)
            {
                case JsonValueKind.String:
                    claims.Add(new Claim(property.Name, property.Value.GetString() ?? string.Empty));
                    break;
                case JsonValueKind.Number:
                    claims.Add(new Claim(property.Name, property.Value.GetRawText()));
                    break;
            }
        }

        var identity = new ClaimsIdentity(claims, "DemoJwt");
        return new ClaimsPrincipal(identity);
    }

    private string Sign(string value)
    {
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:SigningKey"] ?? "development-only-demo-signing-key-change-me");
        using var hmac = new HMACSHA256(key);
        return Base64UrlEncode(hmac.ComputeHash(Encoding.UTF8.GetBytes(value)));
    }

    private static string Base64UrlEncode(byte[] bytes)
    {
        return Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static byte[] Base64UrlDecode(string value)
    {
        var padded = value.Replace('-', '+').Replace('_', '/');
        padded = padded.PadRight(padded.Length + (4 - padded.Length % 4) % 4, '=');
        return Convert.FromBase64String(padded);
    }
}
