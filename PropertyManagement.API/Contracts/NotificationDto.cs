namespace PropertyManagement.API.Contracts
{
    public record NotificationDto(
        int Id,
        int TenantId,
        string TenantName,
        string Subject,
        string Message,
        string Status
        );
}
