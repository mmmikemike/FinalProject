using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.API.Contracts;

public class MaintenanceProjectUpsertRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Select a property.")]
    public int PropertyId { get; set; }

    [Required]
    public string ProjectTitle { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal BidAmount { get; set; }

    [Required]
    public string Status { get; set; } = "Bid";

    [Required]
    public string AssignedVendor { get; set; } = string.Empty;
}
