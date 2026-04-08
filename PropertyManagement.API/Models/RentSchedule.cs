using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.API.Models;

public class RentSchedule
{
    [Key]
    public int ScheduleId { get; set; }

    public int TenantId { get; set; }
    public DateOnly DueDate { get; set; }
    public string Status { get; set; } = "Unpaid";
    public decimal BaseRent { get; set; }
    public decimal LateFeeAccrued { get; set; }
    public int ReminderCount { get; set; }

    public Tenant? Tenant { get; set; }
    public ICollection<RentPayment> RentPayments { get; set; } = [];
    public ICollection<Invoice> Invoices { get; set; } = [];
}
