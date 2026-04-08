using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Blazor.Models;

public class TenantFormModel
{
    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Select a property.")]
    public int PropertyId { get; set; }

    public static TenantFormModel FromTenant(TenantModel tenant) =>
        new()
        {
            FirstName = tenant.FirstName,
            LastName = tenant.LastName,
            Email = tenant.Email,
            PhoneNumber = tenant.PhoneNumber,
            PropertyId = tenant.PropertyId
        };
}
