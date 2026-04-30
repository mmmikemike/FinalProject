using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.API.Models;

public class EvictionCase
{
    [Key]
    public int CaseId { get; set; }

    public int TenantId { get; set; }
    public DateTime OpenedDate { get; set; } = DateTime.UtcNow;
    public string Reason { get; set; } = "Late Rent";
    public string Status { get; set; } = "Open";
    public string CurrentStep { get; set; } = "Late Rent Notice";
    public string Notes { get; set; } = string.Empty;
    public bool LateRentNoticeComplete { get; set; }
    public bool PayOrQuitNoticeComplete { get; set; }
    public bool EvidenceAttached { get; set; }
    public bool AttorneyConsulted { get; set; }
    public bool FilingPrepared { get; set; }
    public bool Resolved { get; set; }

    public Tenant? Tenant { get; set; }
}
