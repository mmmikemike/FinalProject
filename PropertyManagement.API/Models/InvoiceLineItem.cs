using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.API.Models;

public class InvoiceLineItem
{
    [Key]
    public int LineItemId { get; set; }

    public int InvoiceId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ItemType { get; set; } = "Labor";
    public decimal Quantity { get; set; } = 1m;
    public decimal UnitPrice { get; set; }

    public decimal LineTotal => Quantity * UnitPrice;

    public Invoice? Invoice { get; set; }
}
