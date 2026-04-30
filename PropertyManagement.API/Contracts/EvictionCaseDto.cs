namespace PropertyManagement.API.Contracts;

public record EvictionCaseDto(
    int CaseId,
    int TenantId,
    string TenantName,
    string PropertyName,
    DateTime OpenedDate,
    string Reason,
    string Status,
    string CurrentStep,
    string Notes,
    bool LateRentNoticeComplete,
    bool PayOrQuitNoticeComplete,
    bool EvidenceAttached,
    bool AttorneyConsulted,
    bool FilingPrepared,
    bool Resolved);
