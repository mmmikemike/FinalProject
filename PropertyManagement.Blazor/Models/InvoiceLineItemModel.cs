namespace PropertyManagement.Blazor.Models;

public class InvoiceLineItemModel
{
    public int LineItemId { get; set; }
    public int InvoiceId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string ItemType { get; set; } = "Labor";
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}
