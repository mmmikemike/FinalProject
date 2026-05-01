using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.API.Models;

public class CommunicationLog
{
    [Key]
    public int CommunicationId { get; set; }

    public int TenantId { get; set; }
    public int? ScheduleId { get; set; }
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
    public string Channel { get; set; } = "Note";
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = "System";

    public Tenant? Tenant { get; set; }
    public RentSchedule? Schedule { get; set; }
}
