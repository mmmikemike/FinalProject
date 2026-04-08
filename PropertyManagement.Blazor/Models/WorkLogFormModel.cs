using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Blazor.Models;

public class WorkLogFormModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Select a project.")]
    public int ProjectId { get; set; }

    [Required]
    public DateTime ClockInTime { get; set; } = DateTime.Today.AddHours(9);

    public DateTime? ClockOutTime { get; set; } = DateTime.Today.AddHours(10);

    [Required]
    public string GpsLocation { get; set; } = string.Empty;

    public string? ProofPhotoUrl { get; set; }
    public string? MaterialsUsed { get; set; }
    public string? VendorSignature { get; set; }

    public static WorkLogFormModel FromWorkLog(WorkLogModel log) =>
        new()
        {
            ProjectId = log.ProjectId,
            ClockInTime = log.ClockInTime,
            ClockOutTime = log.ClockOutTime,
            GpsLocation = log.GpsLocation,
            ProofPhotoUrl = log.ProofPhotoUrl,
            MaterialsUsed = log.MaterialsUsed,
            VendorSignature = log.VendorSignature
        };
}
