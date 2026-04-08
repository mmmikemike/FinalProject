namespace PropertyManagement.Blazor.Models;

public class PropertyLedgerModel
{
    public int PropertyId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal TotalRentCharged { get; set; }
    public decimal TotalCollected { get; set; }
    public decimal OutstandingBalance { get; set; }
    public decimal MaintenanceExpenses { get; set; }
    public decimal NetIncome { get; set; }
    public List<PropertyTenantBalanceModel> TenantBalances { get; set; } = [];
}
