using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.API.Contracts;
using PropertyManagement.API.Data;
using PropertyManagement.API.Models;

namespace PropertyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaintenanceProjectsController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MaintenanceProjectDto>>> GetMaintenanceProjects([FromQuery] int? propertyId = null, [FromQuery] string? status = null)
    {
        var query = dbContext.MaintenanceProjects
            .AsNoTracking()
            .Include(project => project.Property)
            .Include(project => project.WorkLogs)
            .AsQueryable();

        if (propertyId.HasValue)
        {
            query = query.Where(project => project.PropertyId == propertyId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(project => project.Status == status);
        }

        var projects = await query
            .OrderBy(project => project.Status)
            .ThenBy(project => project.ProjectTitle)
            .ToListAsync();

        return Ok(projects.Select(MapProject));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MaintenanceProjectDto>> GetMaintenanceProject(int id)
    {
        var project = await dbContext.MaintenanceProjects
            .AsNoTracking()
            .Include(item => item.Property)
            .Include(item => item.WorkLogs)
            .FirstOrDefaultAsync(item => item.ProjectId == id);

        return project is null ? NotFound() : Ok(MapProject(project));
    }

    [HttpPost]
    public async Task<ActionResult<MaintenanceProjectDto>> CreateMaintenanceProject([FromBody] MaintenanceProjectUpsertRequest request)
    {
        if (!await dbContext.Properties.AnyAsync(property => property.PropertyId == request.PropertyId))
        {
            ModelState.AddModelError(nameof(request.PropertyId), "Property was not found.");
            return ValidationProblem(ModelState);
        }

        var project = new MaintenanceProject
        {
            PropertyId = request.PropertyId,
            ProjectTitle = request.ProjectTitle.Trim(),
            BidAmount = request.BidAmount,
            Status = request.Status.Trim(),
            AssignedVendor = request.AssignedVendor.Trim()
        };

        dbContext.MaintenanceProjects.Add(project);
        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMaintenanceProject), new { id = project.ProjectId }, await MapProjectAsync(project.ProjectId));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<MaintenanceProjectDto>> UpdateMaintenanceProject(int id, [FromBody] MaintenanceProjectUpsertRequest request)
    {
        var project = await dbContext.MaintenanceProjects.FindAsync(id);
        if (project is null)
        {
            return NotFound();
        }

        if (!await dbContext.Properties.AnyAsync(property => property.PropertyId == request.PropertyId))
        {
            ModelState.AddModelError(nameof(request.PropertyId), "Property was not found.");
            return ValidationProblem(ModelState);
        }

        project.PropertyId = request.PropertyId;
        project.ProjectTitle = request.ProjectTitle.Trim();
        project.BidAmount = request.BidAmount;
        project.Status = request.Status.Trim();
        project.AssignedVendor = request.AssignedVendor.Trim();

        await dbContext.SaveChangesAsync();

        return Ok(await MapProjectAsync(id));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteMaintenanceProject(int id)
    {
        var project = await dbContext.MaintenanceProjects.FindAsync(id);
        if (project is null)
        {
            return NotFound();
        }

        dbContext.MaintenanceProjects.Remove(project);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    private async Task<MaintenanceProjectDto> MapProjectAsync(int id)
    {
        var project = await dbContext.MaintenanceProjects
            .AsNoTracking()
            .Include(item => item.Property)
            .Include(item => item.WorkLogs)
            .FirstAsync(item => item.ProjectId == id);

        return MapProject(project);
    }

    private static MaintenanceProjectDto MapProject(MaintenanceProject project)
    {
        var loggedHours = project.WorkLogs.Sum(log => log.ClockOutTime.HasValue
            ? (decimal)(log.ClockOutTime.Value - log.ClockInTime).TotalHours
            : 0m);

        var lastWorkedAt = project.WorkLogs
            .OrderByDescending(log => log.ClockOutTime ?? log.ClockInTime)
            .Select(log => log.ClockOutTime ?? log.ClockInTime)
            .FirstOrDefault();

        return new MaintenanceProjectDto(
            project.ProjectId,
            project.PropertyId,
            project.Property?.Name ?? "Unknown property",
            project.Property?.Address ?? string.Empty,
            project.Property?.UnitNumber ?? string.Empty,
            project.ProjectTitle,
            project.BidAmount,
            project.Status,
            project.AssignedVendor,
            project.WorkLogs.Count,
            decimal.Round(loggedHours, 2),
            lastWorkedAt == default ? null : lastWorkedAt);
    }
}
