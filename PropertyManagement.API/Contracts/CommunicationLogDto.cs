namespace PropertyManagement.API.Contracts;

public record CommunicationLogDto(
    int CommunicationId,
    int TenantId,
    int? ScheduleId,
    string TenantName,
    DateTime LoggedAt,
    string Channel,
    string Subject,
    string Message,
    string CreatedBy);
