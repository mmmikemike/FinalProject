using Microsoft.EntityFrameworkCore;
using PropertyManagement.API.Models;

namespace PropertyManagement.API.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<RentSchedule> RentSchedules => Set<RentSchedule>();
    public DbSet<RentPayment> RentPayments => Set<RentPayment>();
    public DbSet<MaintenanceProject> MaintenanceProjects => Set<MaintenanceProject>();
    public DbSet<WorkLog> WorkLogs => Set<WorkLog>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<PropertyApplication> PropertyApplications => Set<PropertyApplication>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Property>().ToTable("Properties");
        modelBuilder.Entity<Tenant>().ToTable("Tenants");
        modelBuilder.Entity<RentSchedule>().ToTable("RentSchedules");
        modelBuilder.Entity<RentPayment>().ToTable("RentPayments");
        modelBuilder.Entity<MaintenanceProject>().ToTable("MaintenanceProjects");
        modelBuilder.Entity<WorkLog>().ToTable("WorkLogs");
        modelBuilder.Entity<Invoice>().ToTable("Invoices");
        modelBuilder.Entity<PropertyApplication>().ToTable("PropertyApplications");

        modelBuilder.Entity<PropertyApplication>()
            .HasKey(application => application.ApplicationId);

        modelBuilder.Entity<Property>()
            .Property(property => property.Name)
            .HasColumnName("PropertyName")
            .HasMaxLength(100);

        modelBuilder.Entity<Property>()
            .Property(property => property.Address)
            .HasMaxLength(255);

        modelBuilder.Entity<Property>()
            .Property(property => property.UnitNumber)
            .HasMaxLength(20);

        modelBuilder.Entity<Property>()
            .Property(property => property.MonthlyRent)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Tenant>()
            .Property(tenant => tenant.FirstName)
            .HasMaxLength(50);

        modelBuilder.Entity<Tenant>()
            .Property(tenant => tenant.LastName)
            .HasMaxLength(50);

        modelBuilder.Entity<Tenant>()
            .Property(tenant => tenant.Email)
            .HasMaxLength(100);

        modelBuilder.Entity<Tenant>()
            .Property(tenant => tenant.PhoneNumber)
            .HasMaxLength(20);

        modelBuilder.Entity<RentSchedule>()
            .Property(schedule => schedule.DueDate)
            .HasColumnType("date");

        modelBuilder.Entity<RentSchedule>()
            .Property(schedule => schedule.Status)
            .HasMaxLength(20);

        modelBuilder.Entity<RentSchedule>()
            .Property(schedule => schedule.BaseRent)
            .HasPrecision(18, 2);

        modelBuilder.Entity<RentSchedule>()
            .Property(schedule => schedule.LateFeeAccrued)
            .HasPrecision(18, 2);

        modelBuilder.Entity<RentPayment>()
            .Property(payment => payment.AmountPaid)
            .HasPrecision(18, 2);

        modelBuilder.Entity<RentPayment>()
            .Property(payment => payment.PaymentMethod)
            .HasMaxLength(50);

        modelBuilder.Entity<RentPayment>()
            .Property(payment => payment.TransactionRef)
            .HasMaxLength(100);

        modelBuilder.Entity<MaintenanceProject>()
            .Property(project => project.ProjectTitle)
            .HasMaxLength(200);

        modelBuilder.Entity<MaintenanceProject>()
            .Property(project => project.BidAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<MaintenanceProject>()
            .Property(project => project.Status)
            .HasMaxLength(50);

        modelBuilder.Entity<MaintenanceProject>()
            .Property(project => project.AssignedVendor)
            .HasMaxLength(100);

        modelBuilder.Entity<WorkLog>()
            .Property(log => log.GpsLocation)
            .HasMaxLength(100);

        modelBuilder.Entity<Invoice>()
            .Property(invoice => invoice.TotalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Invoice>()
            .Property(invoice => invoice.Status)
            .HasMaxLength(20);

        modelBuilder.Entity<PropertyApplication>()
            .Property(application => application.ApplicantFirstName)
            .HasMaxLength(50);

        modelBuilder.Entity<PropertyApplication>()
            .Property(application => application.ApplicantLastName)
            .HasMaxLength(50);

        modelBuilder.Entity<PropertyApplication>()
            .Property(application => application.Email)
            .HasMaxLength(100);

        modelBuilder.Entity<PropertyApplication>()
            .Property(application => application.PhoneNumber)
            .HasMaxLength(20);

        modelBuilder.Entity<PropertyApplication>()
            .Property(application => application.PreferredMoveInDate)
            .HasColumnType("date");

        modelBuilder.Entity<PropertyApplication>()
            .Property(application => application.MonthlyIncome)
            .HasPrecision(18, 2);

        modelBuilder.Entity<PropertyApplication>()
            .Property(application => application.CurrentEmployer)
            .HasMaxLength(100);

        modelBuilder.Entity<PropertyApplication>()
            .Property(application => application.PetsDescription)
            .HasMaxLength(200);

        modelBuilder.Entity<PropertyApplication>()
            .Property(application => application.Status)
            .HasMaxLength(20);

        modelBuilder.Entity<Tenant>()
            .HasOne(tenant => tenant.Property)
            .WithMany(property => property.Tenants)
            .HasForeignKey(tenant => tenant.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RentSchedule>()
            .HasOne(schedule => schedule.Tenant)
            .WithMany(tenant => tenant.RentSchedules)
            .HasForeignKey(schedule => schedule.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RentPayment>()
            .HasOne(payment => payment.RentSchedule)
            .WithMany(schedule => schedule.RentPayments)
            .HasForeignKey(payment => payment.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MaintenanceProject>()
            .HasOne(project => project.Property)
            .WithMany(property => property.MaintenanceProjects)
            .HasForeignKey(project => project.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WorkLog>()
            .HasOne(log => log.Project)
            .WithMany(project => project.WorkLogs)
            .HasForeignKey(log => log.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Invoice>()
            .HasOne(invoice => invoice.Project)
            .WithMany(project => project.Invoices)
            .HasForeignKey(invoice => invoice.ProjectId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Invoice>()
            .HasOne(invoice => invoice.Schedule)
            .WithMany(schedule => schedule.Invoices)
            .HasForeignKey(invoice => invoice.ScheduleId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<PropertyApplication>()
            .HasOne(application => application.Property)
            .WithMany(property => property.Applications)
            .HasForeignKey(application => application.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Property>().HasData(
            new Property { PropertyId = 1, Name = "Sunrise Apts", Address = "123 Maple St", UnitNumber = "1A", MonthlyRent = 1200m },
            new Property { PropertyId = 2, Name = "Sunrise Apts", Address = "123 Maple St", UnitNumber = "1B", MonthlyRent = 1250m },
            new Property { PropertyId = 3, Name = "Oak Estates", Address = "456 Oak Ave", UnitNumber = "Unit 10", MonthlyRent = 2000m },
            new Property { PropertyId = 4, Name = "Oak Estates", Address = "456 Oak Ave", UnitNumber = "Unit 11", MonthlyRent = 2100m },
            new Property { PropertyId = 5, Name = "River View", Address = "789 River Rd", UnitNumber = "302", MonthlyRent = 1500m },
            new Property { PropertyId = 6, Name = "River View", Address = "789 River Rd", UnitNumber = "303", MonthlyRent = 1550m });

        modelBuilder.Entity<Tenant>().HasData(
            new Tenant { TenantId = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", PhoneNumber = "555-0101", PropertyId = 1 },
            new Tenant { TenantId = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", PhoneNumber = "555-0102", PropertyId = 2 },
            new Tenant { TenantId = 3, FirstName = "Mike", LastName = "Jones", Email = "mike@example.com", PhoneNumber = "555-0103", PropertyId = 3 },
            new Tenant { TenantId = 4, FirstName = "Sarah", LastName = "Wilson", Email = "sarah@example.com", PhoneNumber = "555-0104", PropertyId = 4 },
            new Tenant { TenantId = 5, FirstName = "Alex", LastName = "Brown", Email = "alex@example.com", PhoneNumber = "555-0105", PropertyId = 5 },
            new Tenant { TenantId = 6, FirstName = "Chris", LastName = "Davis", Email = "chris@example.com", PhoneNumber = "555-0106", PropertyId = 6 });

        modelBuilder.Entity<RentSchedule>().HasData(
            new RentSchedule { ScheduleId = 1, TenantId = 1, DueDate = new DateOnly(2026, 4, 1), Status = "Paid", BaseRent = 1200m, LateFeeAccrued = 0m, ReminderCount = 0 },
            new RentSchedule { ScheduleId = 2, TenantId = 2, DueDate = new DateOnly(2026, 4, 1), Status = "Unpaid", BaseRent = 1250m, LateFeeAccrued = 0m, ReminderCount = 1 },
            new RentSchedule { ScheduleId = 3, TenantId = 3, DueDate = new DateOnly(2026, 4, 1), Status = "Late", BaseRent = 2000m, LateFeeAccrued = 150m, ReminderCount = 3 },
            new RentSchedule { ScheduleId = 4, TenantId = 4, DueDate = new DateOnly(2026, 4, 1), Status = "Unpaid", BaseRent = 2100m, LateFeeAccrued = 0m, ReminderCount = 0 },
            new RentSchedule { ScheduleId = 5, TenantId = 5, DueDate = new DateOnly(2026, 4, 1), Status = "Paid", BaseRent = 1500m, LateFeeAccrued = 0m, ReminderCount = 0 },
            new RentSchedule { ScheduleId = 6, TenantId = 6, DueDate = new DateOnly(2026, 4, 1), Status = "Partial", BaseRent = 1550m, LateFeeAccrued = 50m, ReminderCount = 2 });

        modelBuilder.Entity<RentPayment>().HasData(
            new RentPayment { PaymentId = 1, ScheduleId = 1, PaymentDate = new DateTime(2026, 3, 31, 10, 30, 0, DateTimeKind.Utc), AmountPaid = 1200m, PaymentMethod = "ACH", TransactionRef = "ACH-1001" },
            new RentPayment { PaymentId = 2, ScheduleId = 5, PaymentDate = new DateTime(2026, 3, 31, 11, 0, 0, DateTimeKind.Utc), AmountPaid = 1500m, PaymentMethod = "Card", TransactionRef = "CARD-1005" },
            new RentPayment { PaymentId = 3, ScheduleId = 6, PaymentDate = new DateTime(2026, 4, 2, 9, 15, 0, DateTimeKind.Utc), AmountPaid = 500m, PaymentMethod = "Card", TransactionRef = "CARD-1006" });

        modelBuilder.Entity<MaintenanceProject>().HasData(
            new MaintenanceProject { ProjectId = 1, PropertyId = 1, ProjectTitle = "Broken Faucet", BidAmount = 150m, Status = "Closed", AssignedVendor = "Fix-It Plumbing" },
            new MaintenanceProject { ProjectId = 2, PropertyId = 2, ProjectTitle = "Paint Bedroom", BidAmount = 400m, Status = "Invoiced", AssignedVendor = "Pro Painters" },
            new MaintenanceProject { ProjectId = 3, PropertyId = 3, ProjectTitle = "AC Repair", BidAmount = 800m, Status = "Work Order", AssignedVendor = "CoolAir Inc" },
            new MaintenanceProject { ProjectId = 4, PropertyId = 4, ProjectTitle = "Roof Leak", BidAmount = 2500m, Status = "Bid", AssignedVendor = "TopRoofing" },
            new MaintenanceProject { ProjectId = 5, PropertyId = 5, ProjectTitle = "Floor Buffing", BidAmount = 300m, Status = "Approved", AssignedVendor = "Janitor Pro" },
            new MaintenanceProject { ProjectId = 6, PropertyId = 6, ProjectTitle = "Door Lock Fix", BidAmount = 100m, Status = "Closed", AssignedVendor = "SafeLocks" });

        modelBuilder.Entity<WorkLog>().HasData(
            new WorkLog { LogId = 1, ProjectId = 1, ClockInTime = new DateTime(2026, 1, 10, 9, 0, 0, DateTimeKind.Utc), ClockOutTime = new DateTime(2026, 1, 10, 10, 30, 0, DateTimeKind.Utc), GpsLocation = "34.05,-118.24", ProofPhotoUrl = "img01.jpg", MaterialsUsed = "Replacement faucet kit", VendorSignature = "Fix-It Plumbing" },
            new WorkLog { LogId = 2, ProjectId = 6, ClockInTime = new DateTime(2026, 1, 12, 14, 0, 0, DateTimeKind.Utc), ClockOutTime = new DateTime(2026, 1, 12, 14, 45, 0, DateTimeKind.Utc), GpsLocation = "34.06,-118.25", ProofPhotoUrl = "img02.jpg", MaterialsUsed = "Lock cylinder", VendorSignature = "SafeLocks" },
            new WorkLog { LogId = 3, ProjectId = 3, ClockInTime = new DateTime(2026, 4, 3, 8, 30, 0, DateTimeKind.Utc), ClockOutTime = new DateTime(2026, 4, 3, 11, 0, 0, DateTimeKind.Utc), GpsLocation = "34.07,-118.22", ProofPhotoUrl = "ac-repair.jpg", MaterialsUsed = "Coolant and fan motor", VendorSignature = "CoolAir Inc" });

        modelBuilder.Entity<Invoice>().HasData(
            new Invoice { InvoiceId = 1, ProjectId = 1, ScheduleId = null, InvoiceDate = new DateTime(2026, 1, 11, 12, 0, 0, DateTimeKind.Utc), TotalAmount = 150m, Status = "Paid", IsExported = true },
            new Invoice { InvoiceId = 2, ProjectId = 2, ScheduleId = null, InvoiceDate = new DateTime(2026, 2, 14, 12, 0, 0, DateTimeKind.Utc), TotalAmount = 400m, Status = "Sent", IsExported = false },
            new Invoice { InvoiceId = 3, ProjectId = null, ScheduleId = 2, InvoiceDate = new DateTime(2026, 4, 3, 9, 0, 0, DateTimeKind.Utc), TotalAmount = 1250m, Status = "Overdue", IsExported = false },
            new Invoice { InvoiceId = 4, ProjectId = 6, ScheduleId = null, InvoiceDate = new DateTime(2026, 1, 13, 12, 0, 0, DateTimeKind.Utc), TotalAmount = 100m, Status = "Paid", IsExported = true });
    }
}
