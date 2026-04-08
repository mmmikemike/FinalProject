using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.API.Models;

public class WorkLog
{
    [Key]
    public int LogId { get; set; }

    public int ProjectId { get; set; }
    public DateTime ClockInTime { get; set; } = DateTime.UtcNow;
    public DateTime? ClockOutTime { get; set; }
    public string GpsLocation { get; set; } = string.Empty;
    public string? ProofPhotoUrl { get; set; }
    public string? MaterialsUsed { get; set; }
    public string? VendorSignature { get; set; }

    public MaintenanceProject? Project { get; set; }
}
