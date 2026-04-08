namespace PropertyManagement.API.Contracts;

public record TenantDto(
    int TenantId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    int PropertyId,
    string PropertyName);
