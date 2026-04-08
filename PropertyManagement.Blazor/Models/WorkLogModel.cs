namespace PropertyManagement.Blazor.Models;

public class WorkLogModel
{
    public int LogId { get; set; }
    public int ProjectId { get; set; }
    public string ProjectTitle { get; set; } = string.Empty;
    public DateTime ClockInTime { get; set; }
    public DateTime? ClockOutTime { get; set; }
    public string GpsLocation { get; set; } = string.Empty;
    public string? ProofPhotoUrl { get; set; }
    public string? MaterialsUsed { get; set; }
    public string? VendorSignature { get; set; }
    public decimal LoggedHours { get; set; }
}
