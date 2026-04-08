using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.API.Contracts;
using PropertyManagement.API.Data;
using PropertyManagement.API.Models;

namespace PropertyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PropertyLookupDto>>> GetProperties()
    {
        var properties = await dbContext.Properties
            .AsNoTracking()
            .OrderBy(property => property.Name)
            .ThenBy(property => property.UnitNumber)
            .Select(property => new PropertyLookupDto(
                property.PropertyId,
                property.Name,
                property.Address,
                property.UnitNumber,
                property.MonthlyRent,
                property.Tenants.Count,
                property.MaintenanceProjects.Count(project => project.Status != "Closed")))
            .ToListAsync();

        return Ok(properties);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PropertyLookupDto>> GetProperty(int id)
    {
        var property = await dbContext.Properties
            .AsNoTracking()
            .Where(item => item.PropertyId == id)
            .Select(item => new PropertyLookupDto(
                item.PropertyId,
                item.Name,
                item.Address,
                item.UnitNumber,
                item.MonthlyRent,
                item.Tenants.Count,
                item.MaintenanceProjects.Count(project => project.Status != "Closed")))
            .FirstOrDefaultAsync();

        return property is null ? NotFound() : Ok(property);
    }

    [HttpPost]
    public async Task<ActionResult<PropertyLookupDto>> CreateProperty([FromBody] PropertyUpsertRequest request)
    {
        var property = new Property
        {
            Name = request.Name.Trim(),
            Address = request.Address.Trim(),
            UnitNumber = request.UnitNumber.Trim(),
            MonthlyRent = request.MonthlyRent
        };

        dbContext.Properties.Add(property);
        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProperty), new { id = property.PropertyId }, await MapPropertyAsync(property.PropertyId));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PropertyLookupDto>> UpdateProperty(int id, [FromBody] PropertyUpsertRequest request)
    {
        var property = await dbContext.Properties.FindAsync(id);
        if (property is null)
        {
            return NotFound();
        }

        property.Name = request.Name.Trim();
        property.Address = request.Address.Trim();
        property.UnitNumber = request.UnitNumber.Trim();
        property.MonthlyRent = request.MonthlyRent;

        await dbContext.SaveChangesAsync();

        return Ok(await MapPropertyAsync(id));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteProperty(int id)
    {
        var property = await dbContext.Properties
            .Include(item => item.Tenants)
            .Include(item => item.MaintenanceProjects)
            .Include(item => item.Applications)
            .FirstOrDefaultAsync(item => item.PropertyId == id);

        if (property is null)
        {
            return NotFound();
        }

        if (property.Tenants.Count != 0 || property.MaintenanceProjects.Count != 0 || property.Applications.Count != 0)
        {
            ModelState.AddModelError(nameof(id), "Property cannot be deleted while tenants, maintenance projects, or applications are linked.");
            return ValidationProblem(ModelState);
        }

        dbContext.Properties.Remove(property);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    private async Task<PropertyLookupDto> MapPropertyAsync(int id)
    {
        return await dbContext.Properties
            .AsNoTracking()
            .Where(item => item.PropertyId == id)
            .Select(item => new PropertyLookupDto(
                item.PropertyId,
                item.Name,
                item.Address,
                item.UnitNumber,
                item.MonthlyRent,
                item.Tenants.Count,
                item.MaintenanceProjects.Count(project => project.Status != "Closed")))
            .SingleAsync();
    }
}
