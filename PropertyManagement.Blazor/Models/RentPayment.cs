namespace PropertyManagement.Blazor.Models;

public class RentPayment
{
    public int PaymentId { get; set; }
    public int ScheduleId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public string ScheduleLabel { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; } = DateTime.Today;
    public decimal AmountPaid { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? TransactionRef { get; set; }
}
