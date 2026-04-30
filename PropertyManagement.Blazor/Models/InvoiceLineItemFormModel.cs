using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Blazor.Models;

public class InvoiceLineItemFormModel
{
    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string ItemType { get; set; } = "Labor";

    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; } = 1m;

    [Range(0, double.MaxValue)]
    public decimal UnitPrice { get; set; }

    public decimal LineTotal => Quantity * UnitPrice;

    public static InvoiceLineItemFormModel FromLineItem(InvoiceLineItemModel item) =>
        new()
        {
            Description = item.Description,
            ItemType = item.ItemType,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice
        };
}
