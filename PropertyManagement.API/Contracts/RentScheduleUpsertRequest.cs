using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.API.Contracts;

public class RentScheduleUpsertRequest
{
    [Required]
    [Range(1, int.MaxValue)]
    public int TenantId { get; set; }

    [Required]
    public DateOnly DueDate { get; set; }

    [Required]
    public string Status { get; set; } = "Unpaid";

    [Range(0, double.MaxValue)]
    public decimal BaseRent { get; set; }

    [Range(0, double.MaxValue)]
    public decimal LateFeeAccrued { get; set; }

    [Range(0, int.MaxValue)]
    public int ReminderCount { get; set; }
}
