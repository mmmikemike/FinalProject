namespace PropertyManagement.API.Contracts;

public record RentPaymentDto(
    int PaymentId,
    int ScheduleId,
    string TenantName,
    string ScheduleLabel,
    DateTime PaymentDate,
    decimal AmountPaid,
    string PaymentMethod,
    string? TransactionRef);
