using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.API.Models;

public class Invoice
{
    [Key]
    public int InvoiceId { get; set; }

    public int? ProjectId { get; set; }
    public int? ScheduleId { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Draft";
    public bool IsExported { get; set; }

    public MaintenanceProject? Project { get; set; }
    public RentSchedule? Schedule { get; set; }
    public ICollection<InvoiceLineItem> LineItems { get; set; } = [];
}
