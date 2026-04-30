namespace PropertyManagement.Blazor.Models
{
    public record NotificationDto
    (
        int Id,
        int TenantId,
        string TenantName,
        string Subject,
        string Message,
        string Status
        );
}
