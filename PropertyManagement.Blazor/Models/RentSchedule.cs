namespace PropertyManagement.Blazor.Models
{
    public class RentSchedule
    {
        public int ScheduleID { get; set; }
        public int TenantID { get; set; }
        public DateTime DueDate { get; set; } = DateTime.Today;
        public string? Status { get; set; } = "Unpaid";
        public decimal BaseRent { get; set; }
        public decimal LateFeeAccrued { get; set; }
        public int ReminderCount { get; set; }
        public Tenant? Tenant { get; set; }
    }
}