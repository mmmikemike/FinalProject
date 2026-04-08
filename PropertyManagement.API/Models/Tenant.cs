using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.API.Models;

public class Tenant
{
    [Key]
    public int TenantId { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int PropertyId { get; set; }

    public Property? Property { get; set; }
    public ICollection<RentSchedule> RentSchedules { get; set; } = [];
}
