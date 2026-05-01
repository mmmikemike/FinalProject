using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.API.Contracts;

public class CommunicationLogUpsertRequest
{
    [Range(1, int.MaxValue)]
    public int TenantId { get; set; }

    public int? ScheduleId { get; set; }

    [Required]
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string Channel { get; set; } = "Note";

    [Required]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    [Required]
    public string CreatedBy { get; set; } = "Admin";
}
