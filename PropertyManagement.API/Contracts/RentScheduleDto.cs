namespace PropertyManagement.API.Contracts;

public record RentScheduleDto(
    int ScheduleId,
    int TenantId,
    string TenantName,
    DateOnly DueDate,
    string Status,
    decimal BaseRent,
    decimal LateFeeAccrued,
    int ReminderCount,
    decimal TotalDue,
    decimal TotalPaid);
