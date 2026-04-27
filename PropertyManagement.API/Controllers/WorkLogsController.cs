using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.API.Contracts;
using PropertyManagement.API.Data;
using PropertyManagement.API.Models;
using Microsoft.AspNetCore.Authorization;


namespace PropertyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkLogsController(AppDbContext dbContext) : ControllerBase
{
    [Authorize(Roles = "Admin,Contractor,Staff")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkLogDto>>> GetWorkLogs([FromQuery] int? projectId = null)
    {
        var query = dbContext.WorkLogs
            .AsNoTracking()
            .Include(log => log.Project)
            .AsQueryable();

        if (projectId.HasValue)
        {
            query = query.Where(log => log.ProjectId == projectId.Value);
        }

        var logs = await query
            .OrderByDescending(log => log.ClockInTime)
            .ToListAsync();

        return Ok(logs.Select(MapLog));
    }

    [Authorize(Roles = "Admin,Contractor,Staff")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<WorkLogDto>> GetWorkLog(int id)
    {
        var log = await dbContext.WorkLogs
            .AsNoTracking()
            .Include(item => item.Project)
            .FirstOrDefaultAsync(item => item.LogId == id);

        return log is null ? NotFound() : Ok(MapLog(log));
    }

    [Authorize(Roles = "Admin,Contractor")]
    [HttpPost]
    public async Task<ActionResult<WorkLogDto>> CreateWorkLog([FromBody] WorkLogUpsertRequest request)
    {
        if (!await dbContext.MaintenanceProjects.AnyAsync(project => project.ProjectId == request.ProjectId))
        {
            ModelState.AddModelError(nameof(request.ProjectId), "Maintenance project was not found.");
            return ValidationProblem(ModelState);
        }

        if (request.ClockOutTime.HasValue && request.ClockOutTime.Value < request.ClockInTime)
        {
            ModelState.AddModelError(nameof(request.ClockOutTime), "Clock out must be after clock in.");
            return ValidationProblem(ModelState);
        }

        var log = new WorkLog
        {
            ProjectId = request.ProjectId,
            ClockInTime = request.ClockInTime,
            ClockOutTime = request.ClockOutTime,
            GpsLocation = request.GpsLocation.Trim(),
            ProofPhotoUrl = string.IsNullOrWhiteSpace(request.ProofPhotoUrl) ? null : request.ProofPhotoUrl.Trim(),
            MaterialsUsed = string.IsNullOrWhiteSpace(request.MaterialsUsed) ? null : request.MaterialsUsed.Trim(),
            VendorSignature = string.IsNullOrWhiteSpace(request.VendorSignature) ? null : request.VendorSignature.Trim()
        };

        dbContext.WorkLogs.Add(log);
        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetWorkLog), new { id = log.LogId }, await MapLogAsync(log.LogId));
    }

    [Authorize(Roles = "Admin,Contractor")]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<WorkLogDto>> UpdateWorkLog(int id, [FromBody] WorkLogUpsertRequest request)
    {
        var log = await dbContext.WorkLogs.FindAsync(id);
        if (log is null)
        {
            return NotFound();
        }

        if (!await dbContext.MaintenanceProjects.AnyAsync(project => project.ProjectId == request.ProjectId))
        {
            ModelState.AddModelError(nameof(request.ProjectId), "Maintenance project was not found.");
            return ValidationProblem(ModelState);
        }

        if (request.ClockOutTime.HasValue && request.ClockOutTime.Value < request.ClockInTime)
        {
            ModelState.AddModelError(nameof(request.ClockOutTime), "Clock out must be after clock in.");
            return ValidationProblem(ModelState);
        }

        log.ProjectId = request.ProjectId;
        log.ClockInTime = request.ClockInTime;
        log.ClockOutTime = request.ClockOutTime;
        log.GpsLocation = request.GpsLocation.Trim();
        log.ProofPhotoUrl = string.IsNullOrWhiteSpace(request.ProofPhotoUrl) ? null : request.ProofPhotoUrl.Trim();
        log.MaterialsUsed = string.IsNullOrWhiteSpace(request.MaterialsUsed) ? null : request.MaterialsUsed.Trim();
        log.VendorSignature = string.IsNullOrWhiteSpace(request.VendorSignature) ? null : request.VendorSignature.Trim();

        await dbContext.SaveChangesAsync();

        return Ok(await MapLogAsync(id));
    }
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteWorkLog(int id)
    {
        var log = await dbContext.WorkLogs.FindAsync(id);
        if (log is null)
        {
            return NotFound();
        }

        dbContext.WorkLogs.Remove(log);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    private async Task<WorkLogDto> MapLogAsync(int id)
    {
        var log = await dbContext.WorkLogs
            .AsNoTracking()
            .Include(item => item.Project)
            .FirstAsync(item => item.LogId == id);

        return MapLog(log);
    }

    private static WorkLogDto MapLog(WorkLog log)
    {
        var hours = log.ClockOutTime.HasValue
            ? decimal.Round((decimal)(log.ClockOutTime.Value - log.ClockInTime).TotalHours, 2)
            : 0m;

        return new WorkLogDto(
            log.LogId,
            log.ProjectId,
            log.Project?.ProjectTitle ?? "Unknown project",
            log.ClockInTime,
            log.ClockOutTime,
            log.GpsLocation,
            log.ProofPhotoUrl,
            log.MaterialsUsed,
            log.VendorSignature,
            hours);
    }
}
