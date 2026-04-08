namespace PropertyManagement.Blazor.Models
{
    public class Property
    {
        public int PropertyID { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? UnitNumber { get; set; }
        public decimal MonthlyRent { get; set; }
    }
}