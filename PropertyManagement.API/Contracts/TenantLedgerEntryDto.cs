namespace PropertyManagement.API.Contracts;

public record TenantLedgerEntryDto(
    DateTime EntryDate,
    string EntryType,
    string Description,
    decimal ChargeAmount,
    decimal PaymentAmount,
    decimal RunningBalance);
