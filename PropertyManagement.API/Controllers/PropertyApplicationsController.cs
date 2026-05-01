using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.API.Contracts;
using PropertyManagement.API.Data;
using PropertyManagement.API.Models;

namespace PropertyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertyApplicationsController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrator,Staff")]
    public async Task<ActionResult<IEnumerable<PropertyApplicationDto>>> GetApplications([FromQuery] string? status = null, [FromQuery] int? propertyId = null)
    {
        var query = dbContext.PropertyApplications
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(item => item.Status == status);
        }

        if (propertyId is int propertyFilter)
        {
            query = query.Where(item => item.PropertyId == propertyFilter);
        }

        var applications = await query
            .OrderByDescending(item => item.SubmittedAt)
            .ThenBy(item => item.ApplicantLastName)
            .ThenBy(item => item.ApplicantFirstName)
            .Select(item => new PropertyApplicationDto(
                item.ApplicationId,
                item.PropertyId,
                $"{item.Property!.Name} {item.Property.UnitNumber}".Trim(),
                item.Property.Address,
                item.ApplicantFirstName,
                item.ApplicantLastName,
                item.Email,
                item.PhoneNumber,
                item.PreferredMoveInDate,
                item.MonthlyIncome,
                item.HouseholdSize,
                item.CurrentEmployer,
                item.PetsDescription,
                item.Notes,
                item.Status,
                item.SubmittedAt))
            .ToListAsync();

        return Ok(applications);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Administrator,Staff")]
    public async Task<ActionResult<PropertyApplicationDto>> GetApplication(int id)
    {
        var application = await MapApplicationAsync(id);
        return application is null ? NotFound() : Ok(application);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<PropertyApplicationDto>> CreateApplication([FromBody] PropertyApplicationUpsertRequest request)
    {
        if (!await dbContext.Properties.AnyAsync(item => item.PropertyId == request.PropertyId))
        {
            ModelState.AddModelError(nameof(request.PropertyId), "The selected property was not found.");
            return ValidationProblem(ModelState);
        }

        var application = new PropertyApplication
        {
            PropertyId = request.PropertyId,
            ApplicantFirstName = request.ApplicantFirstName.Trim(),
            ApplicantLastName = request.ApplicantLastName.Trim(),
            Email = request.Email.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            PreferredMoveInDate = request.PreferredMoveInDate,
            MonthlyIncome = request.MonthlyIncome,
            HouseholdSize = request.HouseholdSize,
            CurrentEmployer = request.CurrentEmployer.Trim(),
            PetsDescription = request.PetsDescription.Trim(),
            Notes = request.Notes.Trim(),
            Status = NormalizeStatus(request.Status),
            SubmittedAt = DateTime.UtcNow
        };

        dbContext.PropertyApplications.Add(application);
        await dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetApplication), new { id = application.ApplicationId }, await MapApplicationAsync(application.ApplicationId));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<PropertyApplicationDto>> UpdateApplication(int id, [FromBody] PropertyApplicationUpsertRequest request)
    {
        var application = await dbContext.PropertyApplications.FindAsync(id);
        if (application is null)
        {
            return NotFound();
        }

        if (!await dbContext.Properties.AnyAsync(item => item.PropertyId == request.PropertyId))
        {
            ModelState.AddModelError(nameof(request.PropertyId), "The selected property was not found.");
            return ValidationProblem(ModelState);
        }

        application.PropertyId = request.PropertyId;
        application.ApplicantFirstName = request.ApplicantFirstName.Trim();
        application.ApplicantLastName = request.ApplicantLastName.Trim();
        application.Email = request.Email.Trim();
        application.PhoneNumber = request.PhoneNumber.Trim();
        application.PreferredMoveInDate = request.PreferredMoveInDate;
        application.MonthlyIncome = request.MonthlyIncome;
        application.HouseholdSize = request.HouseholdSize;
        application.CurrentEmployer = request.CurrentEmployer.Trim();
        application.PetsDescription = request.PetsDescription.Trim();
        application.Notes = request.Notes.Trim();
        application.Status = NormalizeStatus(request.Status);

        await dbContext.SaveChangesAsync();

        return Ok(await MapApplicationAsync(id));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteApplication(int id)
    {
        var application = await dbContext.PropertyApplications.FindAsync(id);
        if (application is null)
        {
            return NotFound();
        }

        dbContext.PropertyApplications.Remove(application);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    private async Task<PropertyApplicationDto?> MapApplicationAsync(int id)
    {
        return await dbContext.PropertyApplications
            .AsNoTracking()
            .Where(item => item.ApplicationId == id)
            .Select(item => new PropertyApplicationDto(
                item.ApplicationId,
                item.PropertyId,
                $"{item.Property!.Name} {item.Property.UnitNumber}".Trim(),
                item.Property.Address,
                item.ApplicantFirstName,
                item.ApplicantLastName,
                item.Email,
                item.PhoneNumber,
                item.PreferredMoveInDate,
                item.MonthlyIncome,
                item.HouseholdSize,
                item.CurrentEmployer,
                item.PetsDescription,
                item.Notes,
                item.Status,
                item.SubmittedAt))
            .FirstOrDefaultAsync();
    }

    private static string NormalizeStatus(string? status)
    {
        return string.IsNullOrWhiteSpace(status) ? "New" : status.Trim();
    }
}
