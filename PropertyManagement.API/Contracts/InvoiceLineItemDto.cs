namespace PropertyManagement.API.Contracts;

public record InvoiceLineItemDto(
    int LineItemId,
    int InvoiceId,
    string Description,
    string ItemType,
    decimal Quantity,
    decimal UnitPrice,
    decimal LineTotal);
