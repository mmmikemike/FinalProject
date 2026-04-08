using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.API.Contracts;

public class PropertyApplicationUpsertRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Select a property.")]
    public int PropertyId { get; set; }

    [Required]
    public string ApplicantFirstName { get; set; } = string.Empty;

    [Required]
    public string ApplicantLastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PhoneNumber { get; set; } = string.Empty;

    public DateOnly PreferredMoveInDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddDays(30));

    [Range(0, double.MaxValue)]
    public decimal MonthlyIncome { get; set; }

    [Range(1, 20)]
    public int HouseholdSize { get; set; } = 1;

    [Required]
    public string CurrentEmployer { get; set; } = string.Empty;

    public string PetsDescription { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string Status { get; set; } = "New";
}
