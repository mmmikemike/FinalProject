namespace PropertyManagement.Blazor.Models;

public class InvoiceModel
{
    public int InvoiceId { get; set; }
    public int? ProjectId { get; set; }
    public int? ScheduleId { get; set; }
    public int? TenantId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsExported { get; set; }
    public string ReferenceName { get; set; } = string.Empty;
    public string PropertyName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;

    public string StatusCssClass => Status.ToLowerInvariant() switch
    {
        "paid" => "status-pill status-paid",
        "overdue" => "status-pill status-late",
        "sent" => "status-pill status-partial",
        _ => "status-pill status-unpaid"
    };
}
