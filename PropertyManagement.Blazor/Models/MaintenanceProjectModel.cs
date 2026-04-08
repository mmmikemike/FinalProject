namespace PropertyManagement.Blazor.Models;

public class MaintenanceProjectModel
{
    public int ProjectId { get; set; }
    public int PropertyId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public string PropertyAddress { get; set; } = string.Empty;
    public string UnitNumber { get; set; } = string.Empty;
    public string ProjectTitle { get; set; } = string.Empty;
    public decimal BidAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string AssignedVendor { get; set; } = string.Empty;
    public int WorkLogCount { get; set; }
    public decimal LoggedHours { get; set; }
    public DateTime? LastWorkedAt { get; set; }

    public string PropertyLabel => $"{PropertyName} {UnitNumber}".Trim();

    public string StatusCssClass => Status.ToLowerInvariant() switch
    {
        "closed" => "status-pill status-paid",
        "invoiced" => "status-pill status-partial",
        "work order" => "status-pill status-late",
        "approved" => "status-pill status-unpaid",
        _ => "status-pill status-bid"
    };
}
