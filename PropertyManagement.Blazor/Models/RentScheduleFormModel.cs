using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Blazor.Models;

public class RentScheduleFormModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Select a tenant.")]
    public int TenantId { get; set; }

    [Required]
    public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required]
    public string Status { get; set; } = "Unpaid";

    [Range(0.01, double.MaxValue)]
    public decimal BaseRent { get; set; }

    [Range(0, double.MaxValue)]
    public decimal LateFeeAccrued { get; set; }

    [Range(0, int.MaxValue)]
    public int ReminderCount { get; set; }

    public static RentScheduleFormModel FromSchedule(RentScheduleModel schedule) =>
        new()
        {
            TenantId = schedule.TenantId,
            DueDate = schedule.DueDate,
            Status = schedule.Status,
            BaseRent = schedule.BaseRent,
            LateFeeAccrued = schedule.LateFeeAccrued,
            ReminderCount = schedule.ReminderCount
        };
}
