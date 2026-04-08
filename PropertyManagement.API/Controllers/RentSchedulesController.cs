using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.API.Contracts;
using PropertyManagement.API.Data;
using PropertyManagement.API.Models;

namespace PropertyManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RentSchedulesController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RentScheduleDto>>> GetRentSchedules(
        [FromQuery] string? status = null,
        [FromQuery] string? month = null,
        [FromQuery] int? tenantId = null,
        [FromQuery] int? propertyId = null)
    {
        var query = context.RentSchedules
            .AsNoTracking()
            .Include(schedule => schedule.Tenant)
            .Include(schedule => schedule.RentPayments)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(schedule => schedule.Status == status);
        }

        if (tenantId.HasValue)
        {
            query = query.Where(schedule => schedule.TenantId == tenantId.Value);
        }

        if (propertyId.HasValue)
        {
            query = query.Where(schedule => schedule.Tenant != null && schedule.Tenant.PropertyId == propertyId.Value);
        }

        if (!string.IsNullOrWhiteSpace(month) &&
            DateOnly.TryParseExact($"{month}-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedMonth))
        {
            query = query.Where(schedule => schedule.DueDate.Year == parsedMonth.Year && schedule.DueDate.Month == parsedMonth.Month);
        }

        var schedules = await query
            .OrderBy(schedule => schedule.DueDate)
            .ThenBy(schedule => schedule.Tenant!.LastName)
            .ToListAsync();

        return Ok(schedules.Select(MapSchedule));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RentScheduleDto>> GetRentSchedule(int id)
    {
        var schedule = await context.RentSchedules
            .AsNoTracking()
            .Include(schedule => schedule.Tenant)
            .Include(schedule => schedule.RentPayments)
            .FirstOrDefaultAsync(schedule => schedule.ScheduleId == id);

        return schedule is null ? NotFound() : Ok(MapSchedule(schedule));
    }

    [HttpPost]
    public async Task<ActionResult<RentScheduleDto>> PostRentSchedule(RentScheduleUpsertRequest request)
    {
        var tenantExists = await context.Tenants.AnyAsync(tenant => tenant.TenantId == request.TenantId);
        if (!tenantExists)
        {
            ModelState.AddModelError(nameof(request.TenantId), "Selected tenant does not exist.");
            return ValidationProblem(ModelState);
        }

        var schedule = new RentSchedule
        {
            TenantId = request.TenantId,
            DueDate = request.DueDate,
            Status = request.Status.Trim(),
            BaseRent = request.BaseRent,
            LateFeeAccrued = request.LateFeeAccrued,
            ReminderCount = request.ReminderCount
        };

        context.RentSchedules.Add(schedule);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetRentSchedule), new { id = schedule.ScheduleId }, await MapScheduleAsync(schedule.ScheduleId));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RentScheduleDto>> PutRentSchedule(int id, RentScheduleUpsertRequest request)
    {
        var schedule = await context.RentSchedules.FirstOrDefaultAsync(existingSchedule => existingSchedule.ScheduleId == id);
        if (schedule is null)
        {
            return NotFound();
        }

        var tenantExists = await context.Tenants.AnyAsync(tenant => tenant.TenantId == request.TenantId);
        if (!tenantExists)
        {
            ModelState.AddModelError(nameof(request.TenantId), "Selected tenant does not exist.");
            return ValidationProblem(ModelState);
        }

        schedule.TenantId = request.TenantId;
        schedule.DueDate = request.DueDate;
        schedule.Status = request.Status.Trim();
        schedule.BaseRent = request.BaseRent;
        schedule.LateFeeAccrued = request.LateFeeAccrued;
        schedule.ReminderCount = request.ReminderCount;

        await context.SaveChangesAsync();

        return Ok(await MapScheduleAsync(id));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteRentSchedule(int id)
    {
        var schedule = await context.RentSchedules.FindAsync(id);
        if (schedule is null)
        {
            return NotFound();
        }

        context.RentSchedules.Remove(schedule);
        await context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<RentScheduleDto> MapScheduleAsync(int id)
    {
        var schedule = await context.RentSchedules
            .AsNoTracking()
            .Include(item => item.Tenant)
            .Include(item => item.RentPayments)
            .FirstAsync(item => item.ScheduleId == id);

        return MapSchedule(schedule);
    }

    private static RentScheduleDto MapSchedule(RentSchedule schedule)
    {
        var totalPaid = schedule.RentPayments.Sum(payment => payment.AmountPaid);
        var totalDue = schedule.BaseRent + schedule.LateFeeAccrued;

        return new(
            schedule.ScheduleId,
            schedule.TenantId,
            schedule.Tenant is null ? "Unknown tenant" : $"{schedule.Tenant.FirstName} {schedule.Tenant.LastName}",
            schedule.DueDate,
            schedule.Status,
            schedule.BaseRent,
            schedule.LateFeeAccrued,
            schedule.ReminderCount,
            totalDue,
            totalPaid);
    }
}
