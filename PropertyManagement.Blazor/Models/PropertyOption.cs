namespace PropertyManagement.Blazor.Models;

public class PropertyOption
{
    public int PropertyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string UnitNumber { get; set; } = string.Empty;
    public decimal MonthlyRent { get; set; }
    public int ActiveTenantCount { get; set; }
    public int OpenProjectCount { get; set; }

    public string DisplayLabel => $"{Name} {UnitNumber}".Trim();
    public string FullAddressLabel => $"{DisplayLabel} - {Address}";
}
