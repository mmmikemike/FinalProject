namespace PropertyManagement.API.Contracts
{
    public record NotificationRequest(
         int? TenantId,
         string Subject,
         string Message
         );
}
