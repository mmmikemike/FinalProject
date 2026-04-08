namespace PropertyManagement.API.Contracts;

public record PropertyLookupDto(
    int PropertyId,
    string Name,
    string Address,
    string UnitNumber,
    decimal MonthlyRent,
    int ActiveTenantCount,
    int OpenProjectCount);
