using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.API.Models;

public class RentPayment
{
    [Key]
    public int PaymentId { get; set; }

    public int ScheduleId { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.Today;
    public decimal AmountPaid { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? TransactionRef { get; set; }

    public RentSchedule? RentSchedule { get; set; }
}
