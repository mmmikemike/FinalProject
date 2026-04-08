namespace PropertyManagement.Blazor.Models;

public class TenantLedgerModel
{
    public int TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public string PropertyName { get; set; } = string.Empty;
    public decimal CurrentBalance { get; set; }
    public List<TenantLedgerEntryModel> Entries { get; set; } = [];
}
