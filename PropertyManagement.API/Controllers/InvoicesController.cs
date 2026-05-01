using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.API.Contracts;
using PropertyManagement.API.Data;
using PropertyManagement.API.Models;
using PropertyManagement.API.Security;

namespace PropertyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrator,Staff,Contractor,Tenant")]
    public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetInvoices([FromQuery] string? status = null, [FromQuery] int? projectId = null, [FromQuery] int? scheduleId = null, [FromQuery] int? tenantId = null)
    {
        var query = dbContext.Invoices
            .AsNoTracking()
            .Include(invoice => invoice.Project)
            .ThenInclude(project => project!.Property)
            .Include(invoice => invoice.Schedule)
            .ThenInclude(schedule => schedule!.Tenant)
            .ThenInclude(tenant => tenant!.Property)
            .Include(invoice => invoice.LineItems)
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

        if (User.IsTenantUser())
        {
            var currentTenantId = User.GetTenantId();
            if (!currentTenantId.HasValue)
            {
                return Forbid();
            }

            query = query.Where(invoice => invoice.Schedule != null && invoice.Schedule.TenantId == currentTenantId.Value);
        }

        if (User.IsContractorUser())
        {
            query = query.Where(invoice => invoice.ProjectId.HasValue);
        }

        var invoices = await query
            .OrderByDescending(invoice => invoice.InvoiceDate)
            .ToListAsync();

        return Ok(invoices.Select(MapInvoice));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Administrator,Staff,Contractor,Tenant")]
    public async Task<ActionResult<InvoiceDto>> GetInvoice(int id)
    {
        var invoice = await dbContext.Invoices
            .AsNoTracking()
            .Include(item => item.Project)
            .ThenInclude(project => project!.Property)
            .Include(item => item.Schedule)
            .ThenInclude(schedule => schedule!.Tenant)
            .ThenInclude(tenant => tenant!.Property)
            .Include(item => item.LineItems)
            .FirstOrDefaultAsync(item => item.InvoiceId == id);

        if (invoice is null)
        {
            return NotFound();
        }

        if (User.IsTenantUser() && invoice.Schedule?.TenantId != User.GetTenantId())
        {
            return Forbid();
        }

        if (User.IsContractorUser() && !invoice.ProjectId.HasValue)
        {
            return Forbid();
        }

        return Ok(MapInvoice(invoice));
    }

    [HttpPost]
    [Authorize(Roles = "Administrator,Contractor")]
    public async Task<ActionResult<InvoiceDto>> CreateInvoice([FromBody] InvoiceUpsertRequest request)
    {
        if (User.IsContractorUser() && (!request.ProjectId.HasValue || request.ScheduleId.HasValue))
        {
            return Forbid();
        }

        var validationResult = await ValidateReferencesAsync(request);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var lineItems = NormalizeLineItems(request);
        var invoice = new Invoice
        {
            ProjectId = request.ProjectId,
            ScheduleId = request.ScheduleId,
            InvoiceDate = request.InvoiceDate,
            TotalAmount = CalculateTotal(request, lineItems),
            Status = request.Status.Trim(),
            IsExported = request.IsExported,
            LineItems = lineItems
        };

        dbContext.Invoices.Add(invoice);
        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetInvoice), new { id = invoice.InvoiceId }, await MapInvoiceAsync(invoice.InvoiceId));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrator,Contractor")]
    public async Task<ActionResult<InvoiceDto>> UpdateInvoice(int id, [FromBody] InvoiceUpsertRequest request)
    {
        var invoice = await dbContext.Invoices.FindAsync(id);
        if (invoice is null)
        {
            return NotFound();
        }

        if (User.IsContractorUser() && (!invoice.ProjectId.HasValue || !request.ProjectId.HasValue || request.ScheduleId.HasValue))
        {
            return Forbid();
        }

        var validationResult = await ValidateReferencesAsync(request);
        if (validationResult is not null)
        {
            return validationResult;
        }

        invoice.ProjectId = request.ProjectId;
        invoice.ScheduleId = request.ScheduleId;
        invoice.InvoiceDate = request.InvoiceDate;
        var lineItems = NormalizeLineItems(request);
        invoice.TotalAmount = CalculateTotal(request, lineItems);
        invoice.Status = request.Status.Trim();
        invoice.IsExported = request.IsExported;

        var existingLineItems = dbContext.InvoiceLineItems.Where(item => item.InvoiceId == id);
        dbContext.InvoiceLineItems.RemoveRange(existingLineItems);
        invoice.LineItems = lineItems;

        await dbContext.SaveChangesAsync();

        return Ok(await MapInvoiceAsync(id));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrator,Contractor")]
    public async Task<IActionResult> DeleteInvoice(int id)
    {
        var invoice = await dbContext.Invoices.FindAsync(id);
        if (invoice is null)
        {
            return NotFound();
        }

        if (User.IsContractorUser() && !invoice.ProjectId.HasValue)
        {
            return Forbid();
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
            .Include(item => item.LineItems)
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

        var lineItems = invoice.LineItems
            .OrderBy(item => item.LineItemId)
            .Select(item => new InvoiceLineItemDto(
                item.LineItemId,
                item.InvoiceId,
                item.Description,
                item.ItemType,
                item.Quantity,
                item.UnitPrice,
                item.LineTotal))
            .ToList();

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
            customerName,
            lineItems);
    }

    private static List<InvoiceLineItem> NormalizeLineItems(InvoiceUpsertRequest request)
    {
        if (request.LineItems.Count == 0)
        {
            return [];
        }

        return request.LineItems
            .Where(item => !string.IsNullOrWhiteSpace(item.Description) && item.Quantity > 0)
            .Select(item => new InvoiceLineItem
            {
                Description = item.Description.Trim(),
                ItemType = string.IsNullOrWhiteSpace(item.ItemType) ? "Labor" : item.ItemType.Trim(),
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            })
            .ToList();
    }

    private static decimal CalculateTotal(InvoiceUpsertRequest request, IReadOnlyCollection<InvoiceLineItem> lineItems)
    {
        return lineItems.Count == 0
            ? request.TotalAmount
            : lineItems.Sum(item => item.Quantity * item.UnitPrice);
    }
}
