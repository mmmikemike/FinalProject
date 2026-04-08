namespace PropertyManagement.Blazor.Models;

public class TenantModel
{
    public int TenantId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int PropertyId { get; set; }
    public string PropertyName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";
}
