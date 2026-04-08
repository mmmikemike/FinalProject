namespace PropertyManagement.API.Contracts;

public record WorkLogDto(
    int LogId,
    int ProjectId,
    string ProjectTitle,
    DateTime ClockInTime,
    DateTime? ClockOutTime,
    string GpsLocation,
    string? ProofPhotoUrl,
    string? MaterialsUsed,
    string? VendorSignature,
    decimal LoggedHours);
