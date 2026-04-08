namespace PropertyManagement.API.Contracts;

public record PropertyTenantBalanceDto(
    int TenantId,
    string TenantName,
    decimal Charges,
    decimal Collected,
    decimal OutstandingBalance);
