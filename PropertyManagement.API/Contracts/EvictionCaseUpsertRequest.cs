using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.API.Contracts;

public class EvictionCaseUpsertRequest
{
    [Range(1, int.MaxValue)]
    public int TenantId { get; set; }

    [Required]
    public DateTime OpenedDate { get; set; } = DateTime.Today;

    [Required]
    public string Reason { get; set; } = "Late Rent";

    [Required]
    public string Status { get; set; } = "Open";

    [Required]
    public string CurrentStep { get; set; } = "Late Rent Notice";

    public string Notes { get; set; } = string.Empty;
    public bool LateRentNoticeComplete { get; set; }
    public bool PayOrQuitNoticeComplete { get; set; }
    public bool EvidenceAttached { get; set; }
    public bool AttorneyConsulted { get; set; }
    public bool FilingPrepared { get; set; }
    public bool Resolved { get; set; }
}
