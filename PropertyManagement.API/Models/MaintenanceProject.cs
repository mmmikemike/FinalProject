using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.API.Models;

public class MaintenanceProject
{
    [Key]
    public int ProjectId { get; set; }

    public int PropertyId { get; set; }
    public string ProjectTitle { get; set; } = string.Empty;
    public decimal BidAmount { get; set; }
    public string Status { get; set; } = "Bid";
    public string AssignedVendor { get; set; } = string.Empty;

    public Property? Property { get; set; }
    public ICollection<WorkLog> WorkLogs { get; set; } = [];
    public ICollection<Invoice> Invoices { get; set; } = [];
}
