namespace PropertyManagement.API.Contracts;

public record PropertyApplicationDto(
    int ApplicationId,
    int PropertyId,
    string PropertyLabel,
    string PropertyAddress,
    string ApplicantFirstName,
    string ApplicantLastName,
    string Email,
    string PhoneNumber,
    DateOnly PreferredMoveInDate,
    decimal MonthlyIncome,
    int HouseholdSize,
    string CurrentEmployer,
    string PetsDescription,
    string Notes,
    string Status,
    DateTime SubmittedAt);
