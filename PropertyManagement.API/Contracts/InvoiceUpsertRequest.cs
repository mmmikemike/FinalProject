using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.API.Contracts;

public class InvoiceUpsertRequest
{
    public int? ProjectId { get; set; }
    public int? ScheduleId { get; set; }

    [Required]
    public DateTime InvoiceDate { get; set; } = DateTime.Today;

    [Range(0, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    [Required]
    public string Status { get; set; } = "Draft";

    public bool IsExported { get; set; }
}
