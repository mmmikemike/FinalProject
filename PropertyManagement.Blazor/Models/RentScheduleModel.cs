namespace PropertyManagement.Blazor.Models;

public class RentScheduleModel
{
    public int ScheduleId { get; set; }
    public int TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public DateOnly DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal BaseRent { get; set; }
    public decimal LateFeeAccrued { get; set; }
    public int ReminderCount { get; set; }
    public decimal TotalDue { get; set; }
    public decimal TotalPaid { get; set; }

    public string SummaryLabel => $"{TenantName} - {DueDate:yyyy-MM-dd} ({Status})";
    public string StatusCssClass => Status.ToLowerInvariant() switch
    {
        "paid" => "status-pill status-paid",
        "late" => "status-pill status-late",
        "partial" => "status-pill status-partial",
        _ => "status-pill status-unpaid"
    };
}
