namespace PropertyManagement.API.Contracts;

public record MaintenanceProjectDto(
    int ProjectId,
    int PropertyId,
    string PropertyName,
    string PropertyAddress,
    string UnitNumber,
    string ProjectTitle,
    decimal BidAmount,
    string Status,
    string AssignedVendor,
    int WorkLogCount,
    decimal LoggedHours,
    DateTime? LastWorkedAt);
