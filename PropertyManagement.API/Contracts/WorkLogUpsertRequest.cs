using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.API.Contracts;

public class WorkLogUpsertRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Select a project.")]
    public int ProjectId { get; set; }

    [Required]
    public DateTime ClockInTime { get; set; } = DateTime.Today;

    public DateTime? ClockOutTime { get; set; }

    [Required]
    public string GpsLocation { get; set; } = string.Empty;

    public string? ProofPhotoUrl { get; set; }
    public string? MaterialsUsed { get; set; }
    public string? VendorSignature { get; set; }
}
