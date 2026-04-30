using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.API.Contracts;
using PropertyManagement.API.Data;
using PropertyManagement.API.Models;

namespace PropertyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff")]
    public class NotificationsController(AppDbContext db) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? tenantId)
        {
            var query = db.Notifications.Include(n => n.Tenant).AsQueryable();

            if (tenantId.HasValue)
                query = query.Where(n => n.TenantId == tenantId.Value);

            var results = await query
                .Select(n => new NotificationDto(
                    n.Id,
                    n.TenantId,
                    n.Tenant!.FirstName + " " + n.Tenant.LastName,
                    n.Subject,
                    n.Message,
                    n.Status))
                .ToListAsync();

            return Ok(results);
        }
        [HttpPost]
        public async Task<IActionResult> Send([FromBody] NotificationRequest request)
        {
            var tenant = await db.Tenants.FindAsync(request.TenantId);
            if (tenant is null) return NotFound("Tenant not found.");

            var notification = new Notification
            {
                TenantId = (int)request.TenantId,
                Subject = request.Subject,
                Message = request.Message,
                Status = "Sent"
            };

            db.Notifications.Add(notification);
            await db.SaveChangesAsync();

            return Ok(new { message = $"Notification sent to {tenant.FirstName} {tenant.LastName}." });
        }
    }
}
