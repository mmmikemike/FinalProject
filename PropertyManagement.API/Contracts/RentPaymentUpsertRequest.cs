using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.API.Contracts;

public class RentPaymentUpsertRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ScheduleId { get; set; }

    [Required]
    public DateTime PaymentDate { get; set; } = DateTime.Today;

    [Range(0.01, double.MaxValue)]
    public decimal AmountPaid { get; set; }

    [Required]
    public string PaymentMethod { get; set; } = string.Empty;

    public string? TransactionRef { get; set; }
}
