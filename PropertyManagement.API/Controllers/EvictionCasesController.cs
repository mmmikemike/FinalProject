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
public class EvictionCasesController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EvictionCaseDto>>> GetCases([FromQuery] string? status = null, [FromQuery] int? tenantId = null)
    {
        var query = dbContext.EvictionCases
            .AsNoTracking()
            .Include(item => item.Tenant)
            .ThenInclude(tenant => tenant!.Property)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(item => item.Status == status);
        }

        if (tenantId.HasValue)
        {
            query = query.Where(item => item.TenantId == tenantId.Value);
        }

        var cases = await query
            .OrderByDescending(item => item.OpenedDate)
            .ToListAsync();

        return Ok(cases.Select(MapCase));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EvictionCaseDto>> GetCase(int id)
    {
        var evictionCase = await dbContext.EvictionCases
            .AsNoTracking()
            .Include(item => item.Tenant)
            .ThenInclude(tenant => tenant!.Property)
            .FirstOrDefaultAsync(item => item.CaseId == id);

        return evictionCase is null ? NotFound() : Ok(MapCase(evictionCase));
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<EvictionCaseDto>> CreateCase([FromBody] EvictionCaseUpsertRequest request)
    {
        var tenantExists = await dbContext.Tenants.AnyAsync(tenant => tenant.TenantId == request.TenantId);
        if (!tenantExists)
        {
            ModelState.AddModelError(nameof(request.TenantId), "Selected tenant does not exist.");
            return ValidationProblem(ModelState);
        }

        var evictionCase = new EvictionCase();
        ApplyRequest(evictionCase, request);

        dbContext.EvictionCases.Add(evictionCase);
        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetCase), new { id = evictionCase.CaseId }, await MapCaseAsync(evictionCase.CaseId));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<EvictionCaseDto>> UpdateCase(int id, [FromBody] EvictionCaseUpsertRequest request)
    {
        var evictionCase = await dbContext.EvictionCases.FirstOrDefaultAsync(item => item.CaseId == id);
        if (evictionCase is null)
        {
            return NotFound();
        }

        var tenantExists = await dbContext.Tenants.AnyAsync(tenant => tenant.TenantId == request.TenantId);
        if (!tenantExists)
        {
            ModelState.AddModelError(nameof(request.TenantId), "Selected tenant does not exist.");
            return ValidationProblem(ModelState);
        }

        ApplyRequest(evictionCase, request);
        await dbContext.SaveChangesAsync();

        return Ok(await MapCaseAsync(id));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteCase(int id)
    {
        var evictionCase = await dbContext.EvictionCases.FindAsync(id);
        if (evictionCase is null)
        {
            return NotFound();
        }

        dbContext.EvictionCases.Remove(evictionCase);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    private async Task<EvictionCaseDto> MapCaseAsync(int id)
    {
        var evictionCase = await dbContext.EvictionCases
            .AsNoTracking()
            .Include(item => item.Tenant)
            .ThenInclude(tenant => tenant!.Property)
            .FirstAsync(item => item.CaseId == id);

        return MapCase(evictionCase);
    }

    private static void ApplyRequest(EvictionCase evictionCase, EvictionCaseUpsertRequest request)
    {
        evictionCase.TenantId = request.TenantId;
        evictionCase.OpenedDate = request.OpenedDate;
        evictionCase.Reason = request.Reason.Trim();
        evictionCase.Status = request.Resolved ? "Resolved" : request.Status.Trim();
        evictionCase.CurrentStep = request.CurrentStep.Trim();
        evictionCase.Notes = request.Notes.Trim();
        evictionCase.LateRentNoticeComplete = request.LateRentNoticeComplete;
        evictionCase.PayOrQuitNoticeComplete = request.PayOrQuitNoticeComplete;
        evictionCase.EvidenceAttached = request.EvidenceAttached;
        evictionCase.AttorneyConsulted = request.AttorneyConsulted;
        evictionCase.FilingPrepared = request.FilingPrepared;
        evictionCase.Resolved = request.Resolved;
    }

    private static EvictionCaseDto MapCase(EvictionCase evictionCase)
    {
        var tenantName = evictionCase.Tenant is null
            ? "Unknown tenant"
            : $"{evictionCase.Tenant.FirstName} {evictionCase.Tenant.LastName}";
        var propertyName = evictionCase.Tenant?.Property is null
            ? "Unassigned"
            : $"{evictionCase.Tenant.Property.Name} {evictionCase.Tenant.Property.UnitNumber}".Trim();

        return new EvictionCaseDto(
            evictionCase.CaseId,
            evictionCase.TenantId,
            tenantName,
            propertyName,
            evictionCase.OpenedDate,
            evictionCase.Reason,
            evictionCase.Status,
            evictionCase.CurrentStep,
            evictionCase.Notes,
            evictionCase.LateRentNoticeComplete,
            evictionCase.PayOrQuitNoticeComplete,
            evictionCase.EvidenceAttached,
            evictionCase.AttorneyConsulted,
            evictionCase.FilingPrepared,
            evictionCase.Resolved);
    }
}
