using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.API.Contracts;
using PropertyManagement.API.Data;
using PropertyManagement.API.Models;

namespace PropertyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrator,Staff")]
public class CommunicationLogsController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommunicationLogDto>>> GetLogs([FromQuery] int? tenantId = null, [FromQuery] int? scheduleId = null)
    {
        var query = dbContext.CommunicationLogs
            .AsNoTracking()
            .Include(item => item.Tenant)
            .AsQueryable();

        if (tenantId.HasValue)
        {
            query = query.Where(item => item.TenantId == tenantId.Value);
        }

        if (scheduleId.HasValue)
        {
            query = query.Where(item => item.ScheduleId == scheduleId.Value);
        }

        var logs = await query
            .OrderByDescending(item => item.LoggedAt)
            .ToListAsync();

        return Ok(logs.Select(MapLog));
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<CommunicationLogDto>> CreateLog([FromBody] CommunicationLogUpsertRequest request)
    {
        var tenantExists = await dbContext.Tenants.AnyAsync(tenant => tenant.TenantId == request.TenantId);
        if (!tenantExists)
        {
            ModelState.AddModelError(nameof(request.TenantId), "Selected tenant does not exist.");
            return ValidationProblem(ModelState);
        }

        if (request.ScheduleId.HasValue)
        {
            var scheduleMatchesTenant = await dbContext.RentSchedules.AnyAsync(schedule =>
                schedule.ScheduleId == request.ScheduleId.Value && schedule.TenantId == request.TenantId);

            if (!scheduleMatchesTenant)
            {
                ModelState.AddModelError(nameof(request.ScheduleId), "Selected schedule does not belong to this tenant.");
                return ValidationProblem(ModelState);
            }
        }

        var log = new CommunicationLog
        {
            TenantId = request.TenantId,
            ScheduleId = request.ScheduleId,
            LoggedAt = request.LoggedAt,
            Channel = request.Channel.Trim(),
            Subject = request.Subject.Trim(),
            Message = request.Message.Trim(),
            CreatedBy = request.CreatedBy.Trim()
        };

        dbContext.CommunicationLogs.Add(log);
        await dbContext.SaveChangesAsync();

        var created = await dbContext.CommunicationLogs
            .AsNoTracking()
            .Include(item => item.Tenant)
            .FirstAsync(item => item.CommunicationId == log.CommunicationId);

        return CreatedAtAction(nameof(GetLogs), new { tenantId = created.TenantId }, MapLog(created));
    }

    private static CommunicationLogDto MapLog(CommunicationLog log)
    {
        var tenantName = log.Tenant is null
            ? "Unknown tenant"
            : $"{log.Tenant.FirstName} {log.Tenant.LastName}";

        return new CommunicationLogDto(
            log.CommunicationId,
            log.TenantId,
            log.ScheduleId,
            tenantName,
            log.LoggedAt,
            log.Channel,
            log.Subject,
            log.Message,
            log.CreatedBy);
    }
}
