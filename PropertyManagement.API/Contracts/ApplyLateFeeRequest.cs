using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.API.Contracts;

public class ApplyLateFeeRequest
{
    [Range(0.01, double.MaxValue)]
    public decimal FeeAmount { get; set; } = 50m;

    public string CreatedBy { get; set; } = "Admin";
}
