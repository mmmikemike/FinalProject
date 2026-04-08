namespace PropertyManagement.API.Contracts;

public record PropertyLedgerDto(
    int PropertyId,
    string PropertyName,
    string Address,
    decimal TotalRentCharged,
    decimal TotalCollected,
    decimal OutstandingBalance,
    decimal MaintenanceExpenses,
    decimal NetIncome,
    IReadOnlyList<PropertyTenantBalanceDto> TenantBalances);
