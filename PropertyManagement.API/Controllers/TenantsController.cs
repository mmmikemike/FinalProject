using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.API.Contracts;
using PropertyManagement.API.Data;
using PropertyManagement.API.Models;

namespace PropertyManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TenantsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TenantDto>>> GetTenants()
        {
            var tenants = await _context.Tenants
                .AsNoTracking()
                .Include(t => t.Property)
                .OrderBy(t => t.LastName)
                .ThenBy(t => t.FirstName)
                .ToListAsync();

            return Ok(tenants.Select(MapTenant));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TenantDto>> GetTenant(int id)
        {
            var tenant = await _context.Tenants
                .AsNoTracking()
                .Include(t => t.Property)
                .FirstOrDefaultAsync(t => t.TenantId == id);

            if (tenant == null)
                return NotFound();

            return Ok(MapTenant(tenant));
        }

        [HttpPost]
        public async Task<ActionResult<TenantDto>> PostTenant(TenantUpsertRequest request)
        {
            var propertyExists = await _context.Properties.AnyAsync(property => property.PropertyId == request.PropertyId);
            if (!propertyExists)
            {
                ModelState.AddModelError(nameof(request.PropertyId), "Selected property does not exist.");
                return ValidationProblem(ModelState);
            }

            var tenant = new Tenant
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = request.Email.Trim(),
                PhoneNumber = request.PhoneNumber.Trim(),
                PropertyId = request.PropertyId
            };

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            var createdTenant = await _context.Tenants
                .AsNoTracking()
                .Include(t => t.Property)
                .FirstAsync(t => t.TenantId == tenant.TenantId);

            return CreatedAtAction(nameof(GetTenant), new { id = tenant.TenantId }, MapTenant(createdTenant));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TenantDto>> PutTenant(int id, TenantUpsertRequest request)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(existingTenant => existingTenant.TenantId == id);
            if (tenant == null)
            {
                return NotFound();
            }

            var propertyExists = await _context.Properties.AnyAsync(property => property.PropertyId == request.PropertyId);
            if (!propertyExists)
            {
                ModelState.AddModelError(nameof(request.PropertyId), "Selected property does not exist.");
                return ValidationProblem(ModelState);
            }

            tenant.FirstName = request.FirstName.Trim();
            tenant.LastName = request.LastName.Trim();
            tenant.Email = request.Email.Trim();
            tenant.PhoneNumber = request.PhoneNumber.Trim();
            tenant.PropertyId = request.PropertyId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Tenants.Any(e => e.TenantId == id))
                    return NotFound();
                throw;
            }

            var updatedTenant = await _context.Tenants
                .AsNoTracking()
                .Include(t => t.Property)
                .FirstAsync(t => t.TenantId == id);

            return Ok(MapTenant(updatedTenant));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTenant(int id)
        {
            var tenant = await _context.Tenants.FindAsync(id);
            if (tenant == null)
                return NotFound();

            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static TenantDto MapTenant(Tenant tenant) =>
            new(
                tenant.TenantId,
                tenant.FirstName,
                tenant.LastName,
                tenant.Email,
                tenant.PhoneNumber,
                tenant.PropertyId,
                tenant.Property?.Name ?? "Unassigned");
    }
}
