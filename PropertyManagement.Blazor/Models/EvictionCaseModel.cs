namespace PropertyManagement.Blazor.Models;

public class EvictionCaseModel
{
    public int CaseId { get; set; }
    public int TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public string PropertyName { get; set; } = string.Empty;
    public DateTime OpenedDate { get; set; } = DateTime.Today;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string CurrentStep { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public bool LateRentNoticeComplete { get; set; }
    public bool PayOrQuitNoticeComplete { get; set; }
    public bool EvidenceAttached { get; set; }
    public bool AttorneyConsulted { get; set; }
    public bool FilingPrepared { get; set; }
    public bool Resolved { get; set; }

    public int CompletedStepCount =>
        new[]
        {
            LateRentNoticeComplete,
            PayOrQuitNoticeComplete,
            EvidenceAttached,
            AttorneyConsulted,
            FilingPrepared,
            Resolved
        }.Count(item => item);

    public string StatusCssClass => Status.ToLowerInvariant() switch
    {
        "resolved" => "status-pill status-paid",
        "paused" => "status-pill status-partial",
        _ => "status-pill status-late"
    };
}
