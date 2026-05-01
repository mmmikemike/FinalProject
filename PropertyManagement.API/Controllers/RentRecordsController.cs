using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagement.API.Contracts;
using PropertyManagement.API.Data;
using PropertyManagement.API.Security;

namespace PropertyManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentRecordsController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet("tenant/{tenantId:int}")]
    [Authorize(Roles = "Administrator,Staff,Tenant")]
    public async Task<ActionResult<TenantLedgerDto>> GetTenantLedger(int tenantId)
    {
        if (User.IsTenantUser() && tenantId != User.GetTenantId())
        {
            return Forbid();
        }

        var tenant = await dbContext.Tenants
            .AsNoTracking()
            .Include(item => item.Property)
            .Include(item => item.RentSchedules)
            .ThenInclude(schedule => schedule.RentPayments)
            .FirstOrDefaultAsync(item => item.TenantId == tenantId);

        if (tenant is null)
        {
            return NotFound();
        }

        var entries = tenant.RentSchedules
            .SelectMany(schedule =>
            {
                var chargeEntry = new TenantLedgerEntryDto(
                    schedule.DueDate.ToDateTime(TimeOnly.MinValue),
                    "Charge",
                    $"Rent due {schedule.DueDate:yyyy-MM-dd} ({schedule.Status})",
                    schedule.BaseRent + schedule.LateFeeAccrued,
                    0m,
                    0m);

                var paymentEntries = schedule.RentPayments
                    .OrderBy(payment => payment.PaymentDate)
                    .Select(payment => new TenantLedgerEntryDto(
                        payment.PaymentDate,
                        "Payment",
                        $"{payment.PaymentMethod} payment",
                        0m,
                        payment.AmountPaid,
                        0m));

                return new[] { chargeEntry }.Concat(paymentEntries);
            })
            .OrderBy(entry => entry.EntryDate)
            .ThenBy(entry => entry.EntryType)
            .ToList();

        var runningBalance = 0m;
        var ledgerEntries = entries
            .Select(entry =>
            {
                runningBalance += entry.ChargeAmount - entry.PaymentAmount;
                return entry with { RunningBalance = runningBalance };
            })
            .ToList();

        return Ok(new TenantLedgerDto(
            tenant.TenantId,
            $"{tenant.FirstName} {tenant.LastName}",
            $"{tenant.Property?.Name ?? "Unassigned"} {tenant.Property?.UnitNumber}".Trim(),
            runningBalance,
            ledgerEntries));
    }

    [HttpGet("property/{propertyId:int}")]
    [Authorize(Roles = "Administrator,Staff")]
    public async Task<ActionResult<PropertyLedgerDto>> GetPropertyLedger(int propertyId)
    {
        var property = await dbContext.Properties
            .AsNoTracking()
            .Include(item => item.Tenants)
            .ThenInclude(tenant => tenant.RentSchedules)
            .ThenInclude(schedule => schedule.RentPayments)
            .Include(item => item.MaintenanceProjects)
            .ThenInclude(project => project.Invoices)
            .FirstOrDefaultAsync(item => item.PropertyId == propertyId);

        if (property is null)
        {
            return NotFound();
        }

        var tenantBalances = property.Tenants
            .OrderBy(tenant => tenant.LastName)
            .ThenBy(tenant => tenant.FirstName)
            .Select(tenant =>
            {
                var charges = tenant.RentSchedules.Sum(schedule => schedule.BaseRent + schedule.LateFeeAccrued);
                var collected = tenant.RentSchedules.SelectMany(schedule => schedule.RentPayments).Sum(payment => payment.AmountPaid);
                return new PropertyTenantBalanceDto(
                    tenant.TenantId,
                    $"{tenant.FirstName} {tenant.LastName}",
                    charges,
                    collected,
                    charges - collected);
            })
            .ToList();

        var totalRentCharged = tenantBalances.Sum(item => item.Charges);
        var totalCollected = tenantBalances.Sum(item => item.Collected);
        var outstandingBalance = tenantBalances.Sum(item => item.OutstandingBalance);
        var maintenanceExpenses = property.MaintenanceProjects
            .SelectMany(project => project.Invoices)
            .Where(invoice => invoice.ProjectId.HasValue)
            .Sum(invoice => invoice.TotalAmount);

        return Ok(new PropertyLedgerDto(
            property.PropertyId,
            $"{property.Name} {property.UnitNumber}".Trim(),
            property.Address,
            totalRentCharged,
            totalCollected,
            outstandingBalance,
            maintenanceExpenses,
            totalCollected - maintenanceExpenses,
            tenantBalances));
    }
}
