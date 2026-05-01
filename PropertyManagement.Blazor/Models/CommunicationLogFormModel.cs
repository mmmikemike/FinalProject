using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Blazor.Models;

public class CommunicationLogFormModel
{
    [Range(1, int.MaxValue)]
    public int TenantId { get; set; }

    public int? ScheduleId { get; set; }

    [Required]
    public DateTime LoggedAt { get; set; } = DateTime.Now;

    [Required]
    public string Channel { get; set; } = "Text";

    [Required]
    public string Subject { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    [Required]
    public string CreatedBy { get; set; } = "Admin";
}
