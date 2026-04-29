using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.API.Contracts;
using PropertyManagement.API.Data;
using PropertyManagement.API.Models;

namespace PropertyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetInvoices([FromQuery] string? status = null, [FromQuery] int? projectId = null, [FromQuery] int? scheduleId = null, [FromQuery] int? tenantId = null)
    {
        var query = dbContext.Invoices
            .AsNoTracking()
            .Include(invoice => invoice.Project)
            .ThenInclude(project => project!.Property)
            .Include(invoice => invoice.Schedule)
            .ThenInclude(schedule => schedule!.Tenant)
            .ThenInclude(tenant => tenant!.Property)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(invoice => invoice.Status == status);
        }

        if (projectId.HasValue)
        {
            query = query.Where(invoice => invoice.ProjectId == projectId.Value);
        }

        if (scheduleId.HasValue)
        {
            query = query.Where(invoice => invoice.ScheduleId == scheduleId.Value);
        }

        if (tenantId.HasValue)
        {
            query = query.Where(invoice => invoice.Schedule != null && invoice.Schedule.TenantId == tenantId.Value);
        }

        var invoices = await query
            .OrderByDescending(invoice => invoice.InvoiceDate)
            .ToListAsync();

        return Ok(invoices.Select(MapInvoice));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InvoiceDto>> GetInvoice(int id)
    {
        var invoice = await dbContext.Invoices
            .AsNoTracking()
            .Include(item => item.Project)
            .ThenInclude(project => project!.Property)
            .Include(item => item.Schedule)
            .ThenInclude(schedule => schedule!.Tenant)
            .ThenInclude(tenant => tenant!.Property)
            .FirstOrDefaultAsync(item => item.InvoiceId == id);

        return invoice is null ? NotFound() : Ok(MapInvoice(invoice));
    }

    [HttpPost]
    public async Task<ActionResult<InvoiceDto>> CreateInvoice([FromBody] InvoiceUpsertRequest request)
    {
        var validationResult = await ValidateReferencesAsync(request);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var invoice = new Invoice
        {
            ProjectId = request.ProjectId,
            ScheduleId = request.ScheduleId,
            InvoiceDate = request.InvoiceDate,
            TotalAmount = request.TotalAmount,
            Status = request.Status.Trim(),
            IsExported = request.IsExported
        };

        dbContext.Invoices.Add(invoice);
        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetInvoice), new { id = invoice.InvoiceId }, await MapInvoiceAsync(invoice.InvoiceId));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<InvoiceDto>> UpdateInvoice(int id, [FromBody] InvoiceUpsertRequest request)
    {
        var invoice = await dbContext.Invoices.FindAsync(id);
        if (invoice is null)
        {
            return NotFound();
        }

        var validationResult = await ValidateReferencesAsync(request);
        if (validationResult is not null)
        {
            return validationResult;
        }

        invoice.ProjectId = request.ProjectId;
        invoice.ScheduleId = request.ScheduleId;
        invoice.InvoiceDate = request.InvoiceDate;
        invoice.TotalAmount = request.TotalAmount;
        invoice.Status = request.Status.Trim();
        invoice.IsExported = request.IsExported;

        await dbContext.SaveChangesAsync();

        return Ok(await MapInvoiceAsync(id));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteInvoice(int id)
    {
        var invoice = await dbContext.Invoices.FindAsync(id);
        if (invoice is null)
        {
            return NotFound();
        }

        dbContext.Invoices.Remove(invoice);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    private async Task<ActionResult?> ValidateReferencesAsync(InvoiceUpsertRequest request)
    {
        if (request.ProjectId.HasValue && !await dbContext.MaintenanceProjects.AnyAsync(project => project.ProjectId == request.ProjectId.Value))
        {
            ModelState.AddModelError(nameof(request.ProjectId), "Maintenance project was not found.");
            return ValidationProblem(ModelState);
        }

        if (request.ScheduleId.HasValue && !await dbContext.RentSchedules.AnyAsync(schedule => schedule.ScheduleId == request.ScheduleId.Value))
        {
            ModelState.AddModelError(nameof(request.ScheduleId), "Rent schedule was not found.");
            return ValidationProblem(ModelState);
        }

        return null;
    }

    private async Task<InvoiceDto> MapInvoiceAsync(int id)
    {
        var invoice = await dbContext.Invoices
            .AsNoTracking()
            .Include(item => item.Project)
            .ThenInclude(project => project!.Property)
            .Include(item => item.Schedule)
            .ThenInclude(schedule => schedule!.Tenant)
            .ThenInclude(tenant => tenant!.Property)
            .FirstAsync(item => item.InvoiceId == id);

        return MapInvoice(invoice);
    }

    private static InvoiceDto MapInvoice(Invoice invoice)
    {
        var referenceName = invoice.Project is not null
            ? invoice.Project.ProjectTitle
            : invoice.Schedule is not null
                ? $"Rent due {invoice.Schedule.DueDate:yyyy-MM-dd}"
                : "Standalone invoice";

        var propertyName = invoice.Project?.Property is not null
            ? $"{invoice.Project.Property.Name} {invoice.Project.Property.UnitNumber}".Trim()
            : invoice.Schedule?.Tenant?.Property is not null
                ? $"{invoice.Schedule.Tenant.Property.Name} {invoice.Schedule.Tenant.Property.UnitNumber}".Trim()
                : "Unassigned";

        var customerName = invoice.Schedule?.Tenant is not null
            ? $"{invoice.Schedule.Tenant.FirstName} {invoice.Schedule.Tenant.LastName}"
            : invoice.Project?.AssignedVendor ?? "Internal";

        return new InvoiceDto(
            invoice.InvoiceId,
            invoice.ProjectId,
            invoice.ScheduleId,
            invoice.Schedule?.TenantId,
            invoice.InvoiceDate,
            invoice.TotalAmount,
            invoice.Status,
            invoice.IsExported,
            referenceName,
            propertyName,
            customerName);
    }
}
