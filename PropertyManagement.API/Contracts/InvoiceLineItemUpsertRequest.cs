using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.API.Contracts;

public class InvoiceLineItemUpsertRequest
{
    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string ItemType { get; set; } = "Labor";

    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; } = 1m;

    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }
}
