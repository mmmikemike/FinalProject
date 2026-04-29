namespace PropertyManagement.API.Contracts;

public record InvoiceDto(
    int InvoiceId,
    int? ProjectId,
    int? ScheduleId,
    int? TenantId,
    DateTime InvoiceDate,
    decimal TotalAmount,
    string Status,
    bool IsExported,
    string ReferenceName,
    string PropertyName,
    string CustomerName);
