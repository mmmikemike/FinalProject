using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.API.Contracts;
using PropertyManagement.API.Data;
using PropertyManagement.API.Models;
using PropertyManagement.API.Security;

namespace PropertyManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RentPaymentsController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Administrator,Staff,Tenant")]
    public async Task<ActionResult<IEnumerable<RentPaymentDto>>> GetRentPayments([FromQuery] int? tenantId = null, [FromQuery] int? scheduleId = null)
    {
        var query = context.RentPayments
            .AsNoTracking()
            .Include(payment => payment.RentSchedule)
            .ThenInclude(schedule => schedule!.Tenant)
            .AsQueryable();

        if (tenantId.HasValue)
        {
            query = query.Where(payment => payment.RentSchedule != null && payment.RentSchedule.TenantId == tenantId.Value);
        }

        if (User.IsTenantUser())
        {
            var currentTenantId = User.GetTenantId();
            if (!currentTenantId.HasValue)
            {
                return Forbid();
            }

            query = query.Where(payment => payment.RentSchedule != null && payment.RentSchedule.TenantId == currentTenantId.Value);
        }

        if (scheduleId.HasValue)
        {
            query = query.Where(payment => payment.ScheduleId == scheduleId.Value);
        }

        var payments = await query
            .OrderByDescending(payment => payment.PaymentDate)
            .ToListAsync();

        return Ok(payments.Select(MapPayment));
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Administrator,Staff,Tenant")]
    public async Task<ActionResult<RentPaymentDto>> GetRentPayment(int id)
    {
        var payment = await context.RentPayments
            .AsNoTracking()
            .Include(item => item.RentSchedule)
            .ThenInclude(schedule => schedule!.Tenant)
            .FirstOrDefaultAsync(item => item.PaymentId == id);

        if (payment is null)
        {
            return NotFound();
        }

        if (User.IsTenantUser() && payment.RentSchedule?.TenantId != User.GetTenantId())
        {
            return Forbid();
        }

        return Ok(MapPayment(payment));
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<RentPaymentDto>> PostRentPayment(RentPaymentUpsertRequest request)
    {
        var scheduleExists = await context.RentSchedules.AnyAsync(schedule => schedule.ScheduleId == request.ScheduleId);
        if (!scheduleExists)
        {
            ModelState.AddModelError(nameof(request.ScheduleId), "Selected schedule does not exist.");
            return ValidationProblem(ModelState);
        }

        var payment = new RentPayment
        {
            ScheduleId = request.ScheduleId,
            PaymentDate = request.PaymentDate,
            AmountPaid = request.AmountPaid,
            PaymentMethod = request.PaymentMethod.Trim(),
            TransactionRef = string.IsNullOrWhiteSpace(request.TransactionRef) ? null : request.TransactionRef.Trim()
        };

        context.RentPayments.Add(payment);
        await context.SaveChangesAsync();
        await RefreshScheduleStatusAsync(request.ScheduleId);

        return CreatedAtAction(nameof(GetRentPayment), new { id = payment.PaymentId }, await MapPaymentAsync(payment.PaymentId));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<RentPaymentDto>> PutRentPayment(int id, RentPaymentUpsertRequest request)
    {
        var payment = await context.RentPayments.FirstOrDefaultAsync(existingPayment => existingPayment.PaymentId == id);
        if (payment is null)
        {
            return NotFound();
        }

        var scheduleExists = await context.RentSchedules.AnyAsync(schedule => schedule.ScheduleId == request.ScheduleId);
        if (!scheduleExists)
        {
            ModelState.AddModelError(nameof(request.ScheduleId), "Selected schedule does not exist.");
            return ValidationProblem(ModelState);
        }

        payment.ScheduleId = request.ScheduleId;
        payment.PaymentDate = request.PaymentDate;
        payment.AmountPaid = request.AmountPaid;
        payment.PaymentMethod = request.PaymentMethod.Trim();
        payment.TransactionRef = string.IsNullOrWhiteSpace(request.TransactionRef) ? null : request.TransactionRef.Trim();

        await context.SaveChangesAsync();
        await RefreshScheduleStatusAsync(request.ScheduleId);

        return Ok(await MapPaymentAsync(id));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteRentPayment(int id)
    {
        var payment = await context.RentPayments.FindAsync(id);
        if (payment is null)
        {
            return NotFound();
        }

        context.RentPayments.Remove(payment);
        await context.SaveChangesAsync();
        await RefreshScheduleStatusAsync(payment.ScheduleId);

        return NoContent();
    }

    private async Task RefreshScheduleStatusAsync(int scheduleId)
    {
        var schedule = await context.RentSchedules
            .Include(item => item.RentPayments)
            .FirstOrDefaultAsync(item => item.ScheduleId == scheduleId);

        if (schedule is null)
        {
            return;
        }

        var totalDue = schedule.BaseRent + schedule.LateFeeAccrued;
        var totalPaid = schedule.RentPayments.Sum(payment => payment.AmountPaid);

        schedule.Status = totalPaid <= 0
            ? schedule.Status == "Late" ? "Late" : "Unpaid"
            : totalPaid >= totalDue ? "Paid" : "Partial";

        await context.SaveChangesAsync();
    }

    private async Task<RentPaymentDto> MapPaymentAsync(int id)
    {
        var payment = await context.RentPayments
            .AsNoTracking()
            .Include(item => item.RentSchedule)
            .ThenInclude(schedule => schedule!.Tenant)
            .FirstAsync(item => item.PaymentId == id);

        return MapPayment(payment);
    }

    private static RentPaymentDto MapPayment(RentPayment payment)
    {
        var tenantName = payment.RentSchedule?.Tenant is null
            ? "Unknown tenant"
            : $"{payment.RentSchedule.Tenant.FirstName} {payment.RentSchedule.Tenant.LastName}";

        var scheduleLabel = payment.RentSchedule is null
            ? $"Schedule #{payment.ScheduleId}"
            : $"{tenantName} - {payment.RentSchedule.DueDate:yyyy-MM-dd} ({payment.RentSchedule.Status})";

        return new(
            payment.PaymentId,
            payment.ScheduleId,
            tenantName,
            scheduleLabel,
            payment.PaymentDate,
            payment.AmountPaid,
            payment.PaymentMethod,
            payment.TransactionRef);
    }
}
