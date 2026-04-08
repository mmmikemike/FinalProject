using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.API.Contracts;

public class TenantUpsertRequest
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

    [Required]
    [Range(1, int.MaxValue)]
    public int PropertyId { get; set; }
}
