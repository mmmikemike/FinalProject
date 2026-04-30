using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Blazor.Models;

public class EvictionCaseFormModel
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

    public static EvictionCaseFormModel FromCase(EvictionCaseModel evictionCase) =>
        new()
        {
            TenantId = evictionCase.TenantId,
            OpenedDate = evictionCase.OpenedDate,
            Reason = evictionCase.Reason,
            Status = evictionCase.Status,
            CurrentStep = evictionCase.CurrentStep,
            Notes = evictionCase.Notes,
            LateRentNoticeComplete = evictionCase.LateRentNoticeComplete,
            PayOrQuitNoticeComplete = evictionCase.PayOrQuitNoticeComplete,
            EvidenceAttached = evictionCase.EvidenceAttached,
            AttorneyConsulted = evictionCase.AttorneyConsulted,
            FilingPrepared = evictionCase.FilingPrepared,
            Resolved = evictionCase.Resolved
        };
}
