namespace PropertyManagement.Blazor.Models;

public class PropertyApplicationModel
{
    public int ApplicationId { get; set; }
    public int PropertyId { get; set; }
    public string PropertyLabel { get; set; } = string.Empty;
    public string PropertyAddress { get; set; } = string.Empty;
    public string ApplicantFirstName { get; set; } = string.Empty;
    public string ApplicantLastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateOnly PreferredMoveInDate { get; set; }
    public decimal MonthlyIncome { get; set; }
    public int HouseholdSize { get; set; }
    public string CurrentEmployer { get; set; } = string.Empty;
    public string PetsDescription { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }

    public string ApplicantName => $"{ApplicantFirstName} {ApplicantLastName}".Trim();

    public string StatusCssClass => Status.ToLowerInvariant() switch
    {
        "approved" => "status-pill status-paid",
        "reviewing" => "status-pill status-partial",
        "declined" => "status-pill status-late",
        _ => "status-pill status-unpaid"
    };
}
