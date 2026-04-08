namespace PropertyManagement.API.Models;

public class Property
{
    public int PropertyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string UnitNumber { get; set; } = string.Empty;
    public decimal MonthlyRent { get; set; }
    public ICollection<Tenant> Tenants { get; set; } = [];
    public ICollection<MaintenanceProject> MaintenanceProjects { get; set; } = [];
    public ICollection<PropertyApplication> Applications { get; set; } = [];
}
