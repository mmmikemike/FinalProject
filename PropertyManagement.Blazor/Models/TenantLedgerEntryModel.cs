namespace PropertyManagement.Blazor.Models;

public class TenantLedgerEntryModel
{
    public DateTime EntryDate { get; set; }
    public string EntryType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal ChargeAmount { get; set; }
    public decimal PaymentAmount { get; set; }
    public decimal RunningBalance { get; set; }
}
