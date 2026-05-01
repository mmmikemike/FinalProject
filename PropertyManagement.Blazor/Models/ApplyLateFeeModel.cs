using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Blazor.Models;

public class ApplyLateFeeModel
{
    [Range(0.01, double.MaxValue)]
    public decimal FeeAmount { get; set; } = 50m;

    public string CreatedBy { get; set; } = "Admin";
}
