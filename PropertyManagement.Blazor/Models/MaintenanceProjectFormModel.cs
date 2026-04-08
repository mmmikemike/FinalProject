using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Blazor.Models;

public class MaintenanceProjectFormModel
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

    public static MaintenanceProjectFormModel FromProject(MaintenanceProjectModel project) =>
        new()
        {
            PropertyId = project.PropertyId,
            ProjectTitle = project.ProjectTitle,
            BidAmount = project.BidAmount,
            Status = project.Status,
            AssignedVendor = project.AssignedVendor
        };
}
