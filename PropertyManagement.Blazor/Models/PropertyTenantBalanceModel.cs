namespace PropertyManagement.Blazor.Models;

public class PropertyTenantBalanceModel
{
    public int TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public decimal Charges { get; set; }
    public decimal Collected { get; set; }
    public decimal OutstandingBalance { get; set; }
}
