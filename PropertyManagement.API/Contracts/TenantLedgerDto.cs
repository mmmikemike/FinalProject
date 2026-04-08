namespace PropertyManagement.API.Contracts;

public record TenantLedgerDto(
    int TenantId,
    string TenantName,
    string PropertyName,
    decimal CurrentBalance,
    IReadOnlyList<TenantLedgerEntryDto> Entries);
