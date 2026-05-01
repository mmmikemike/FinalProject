namespace PropertyManagement.Blazor.Models;

public class CommunicationLogModel
{
    public int CommunicationId { get; set; }
    public int TenantId { get; set; }
    public int? ScheduleId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public DateTime LoggedAt { get; set; }
    public string Channel { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
}
