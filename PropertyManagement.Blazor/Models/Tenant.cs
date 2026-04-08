namespace PropertyManagement.Blazor.Models
{
    public class Tenant
    {
        public int TenantID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int PropertyID { get; set; }
        public Property? Property { get; set; }
    }
}