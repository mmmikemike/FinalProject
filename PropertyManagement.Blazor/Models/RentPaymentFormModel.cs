using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Blazor.Models;

public class RentPaymentFormModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Select a rent schedule.")]
    public int ScheduleId { get; set; }

    [Required]
    public DateTime PaymentDate { get; set; } = DateTime.Today;

    [Range(0.01, double.MaxValue)]
    public decimal AmountPaid { get; set; }

    [Required]
    public string PaymentMethod { get; set; } = "ACH";

    public string? TransactionRef { get; set; }

    public static RentPaymentFormModel FromPayment(RentPayment payment) =>
        new()
        {
            ScheduleId = payment.ScheduleId,
            PaymentDate = payment.PaymentDate,
            AmountPaid = payment.AmountPaid,
            PaymentMethod = payment.PaymentMethod,
            TransactionRef = payment.TransactionRef
        };
}
