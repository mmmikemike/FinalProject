using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Blazor.Models;

public class PropertyFormModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Address { get; set; } = string.Empty;

    public string UnitNumber { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal MonthlyRent { get; set; }

    public static PropertyFormModel FromProperty(PropertyOption property) =>
        new()
        {
            Name = property.Name,
            Address = property.Address,
            UnitNumber = property.UnitNumber,
            MonthlyRent = property.MonthlyRent
        };
}
