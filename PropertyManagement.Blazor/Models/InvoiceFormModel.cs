using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Blazor.Models;

public class InvoiceFormModel
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
    public List<InvoiceLineItemFormModel> LineItems { get; set; } = [new()];

    public decimal LineItemsTotal => LineItems.Sum(item => item.LineTotal);

    public static InvoiceFormModel FromInvoice(InvoiceModel invoice) =>
        new()
        {
            ProjectId = invoice.ProjectId,
            ScheduleId = invoice.ScheduleId,
            InvoiceDate = invoice.InvoiceDate,
            TotalAmount = invoice.TotalAmount,
            Status = invoice.Status,
            IsExported = invoice.IsExported,
            LineItems = invoice.LineItems.Count == 0
                ? [new InvoiceLineItemFormModel { Description = invoice.ReferenceName, Quantity = 1m, UnitPrice = invoice.TotalAmount }]
                : invoice.LineItems.Select(InvoiceLineItemFormModel.FromLineItem).ToList()
        };
}
