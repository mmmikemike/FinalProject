using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PropertyManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    PropertyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UnitNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MonthlyRent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.PropertyId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceProjects",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    ProjectTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BidAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AssignedVendor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaintenanceProjects", x => x.ProjectId);
                    table.ForeignKey(
                        name: "FK_MaintenanceProjects_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyApplications",
                columns: table => new
                {
                    ApplicationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    ApplicantFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApplicantLastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PreferredMoveInDate = table.Column<DateOnly>(type: "date", nullable: false),
                    MonthlyIncome = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    HouseholdSize = table.Column<int>(type: "int", nullable: false),
                    CurrentEmployer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PetsDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyApplications", x => x.ApplicationId);
                    table.ForeignKey(
                        name: "FK_PropertyApplications_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    TenantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PropertyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.TenantId);
                    table.ForeignKey(
                        name: "FK_Tenants_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "PropertyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkLogs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    ClockInTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClockOutTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GpsLocation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProofPhotoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialsUsed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VendorSignature = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLogs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_WorkLogs_MaintenanceProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "MaintenanceProjects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RentSchedules",
                columns: table => new
                {
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenantId = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BaseRent = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    LateFeeAccrued = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReminderCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentSchedules", x => x.ScheduleId);
                    table.ForeignKey(
                        name: "FK_RentSchedules_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    ScheduleId = table.Column<int>(type: "int", nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsExported = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.InvoiceId);
                    table.ForeignKey(
                        name: "FK_Invoices_MaintenanceProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "MaintenanceProjects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Invoices_RentSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "RentSchedules",
                        principalColumn: "ScheduleId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RentPayments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScheduleId = table.Column<int>(type: "int", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TransactionRef = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RentPayments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_RentPayments_RentSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "RentSchedules",
                        principalColumn: "ScheduleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Properties",
                columns: new[] { "PropertyId", "Address", "MonthlyRent", "PropertyName", "UnitNumber" },
                values: new object[,]
                {
                    { 1, "123 Maple St", 1200m, "Sunrise Apts", "1A" },
                    { 2, "123 Maple St", 1250m, "Sunrise Apts", "1B" },
                    { 3, "456 Oak Ave", 2000m, "Oak Estates", "Unit 10" },
                    { 4, "456 Oak Ave", 2100m, "Oak Estates", "Unit 11" },
                    { 5, "789 River Rd", 1500m, "River View", "302" },
                    { 6, "789 River Rd", 1550m, "River View", "303" }
                });

            migrationBuilder.InsertData(
                table: "MaintenanceProjects",
                columns: new[] { "ProjectId", "AssignedVendor", "BidAmount", "ProjectTitle", "PropertyId", "Status" },
                values: new object[,]
                {
                    { 1, "Fix-It Plumbing", 150m, "Broken Faucet", 1, "Closed" },
                    { 2, "Pro Painters", 400m, "Paint Bedroom", 2, "Invoiced" },
                    { 3, "CoolAir Inc", 800m, "AC Repair", 3, "Work Order" },
                    { 4, "TopRoofing", 2500m, "Roof Leak", 4, "Bid" },
                    { 5, "Janitor Pro", 300m, "Floor Buffing", 5, "Approved" },
                    { 6, "SafeLocks", 100m, "Door Lock Fix", 6, "Closed" }
                });

            migrationBuilder.InsertData(
                table: "Tenants",
                columns: new[] { "TenantId", "Email", "FirstName", "LastName", "PhoneNumber", "PropertyId" },
                values: new object[,]
                {
                    { 1, "john@example.com", "John", "Doe", "555-0101", 1 },
                    { 2, "jane@example.com", "Jane", "Smith", "555-0102", 2 },
                    { 3, "mike@example.com", "Mike", "Jones", "555-0103", 3 },
                    { 4, "sarah@example.com", "Sarah", "Wilson", "555-0104", 4 },
                    { 5, "alex@example.com", "Alex", "Brown", "555-0105", 5 },
                    { 6, "chris@example.com", "Chris", "Davis", "555-0106", 6 }
                });

            migrationBuilder.InsertData(
                table: "Invoices",
                columns: new[] { "InvoiceId", "InvoiceDate", "IsExported", "ProjectId", "ScheduleId", "Status", "TotalAmount" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 11, 12, 0, 0, 0, DateTimeKind.Utc), true, 1, null, "Paid", 150m },
                    { 2, new DateTime(2026, 2, 14, 12, 0, 0, 0, DateTimeKind.Utc), false, 2, null, "Sent", 400m },
                    { 4, new DateTime(2026, 1, 13, 12, 0, 0, 0, DateTimeKind.Utc), true, 6, null, "Paid", 100m }
                });

            migrationBuilder.InsertData(
                table: "RentSchedules",
                columns: new[] { "ScheduleId", "BaseRent", "DueDate", "LateFeeAccrued", "ReminderCount", "Status", "TenantId" },
                values: new object[,]
                {
                    { 1, 1200m, new DateOnly(2026, 4, 1), 0m, 0, "Paid", 1 },
                    { 2, 1250m, new DateOnly(2026, 4, 1), 0m, 1, "Unpaid", 2 },
                    { 3, 2000m, new DateOnly(2026, 4, 1), 150m, 3, "Late", 3 },
                    { 4, 2100m, new DateOnly(2026, 4, 1), 0m, 0, "Unpaid", 4 },
                    { 5, 1500m, new DateOnly(2026, 4, 1), 0m, 0, "Paid", 5 },
                    { 6, 1550m, new DateOnly(2026, 4, 1), 50m, 2, "Partial", 6 }
                });

            migrationBuilder.InsertData(
                table: "WorkLogs",
                columns: new[] { "LogId", "ClockInTime", "ClockOutTime", "GpsLocation", "MaterialsUsed", "ProjectId", "ProofPhotoUrl", "VendorSignature" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 10, 9, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 10, 10, 30, 0, 0, DateTimeKind.Utc), "34.05,-118.24", "Replacement faucet kit", 1, "img01.jpg", "Fix-It Plumbing" },
                    { 2, new DateTime(2026, 1, 12, 14, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 12, 14, 45, 0, 0, DateTimeKind.Utc), "34.06,-118.25", "Lock cylinder", 6, "img02.jpg", "SafeLocks" },
                    { 3, new DateTime(2026, 4, 3, 8, 30, 0, 0, DateTimeKind.Utc), new DateTime(2026, 4, 3, 11, 0, 0, 0, DateTimeKind.Utc), "34.07,-118.22", "Coolant and fan motor", 3, "ac-repair.jpg", "CoolAir Inc" }
                });

            migrationBuilder.InsertData(
                table: "Invoices",
                columns: new[] { "InvoiceId", "InvoiceDate", "IsExported", "ProjectId", "ScheduleId", "Status", "TotalAmount" },
                values: new object[] { 3, new DateTime(2026, 4, 3, 9, 0, 0, 0, DateTimeKind.Utc), false, null, 2, "Overdue", 1250m });

            migrationBuilder.InsertData(
                table: "RentPayments",
                columns: new[] { "PaymentId", "AmountPaid", "PaymentDate", "PaymentMethod", "ScheduleId", "TransactionRef" },
                values: new object[,]
                {
                    { 1, 1200m, new DateTime(2026, 3, 31, 10, 30, 0, 0, DateTimeKind.Utc), "ACH", 1, "ACH-1001" },
                    { 2, 1500m, new DateTime(2026, 3, 31, 11, 0, 0, 0, DateTimeKind.Utc), "Card", 5, "CARD-1005" },
                    { 3, 500m, new DateTime(2026, 4, 2, 9, 15, 0, 0, DateTimeKind.Utc), "Card", 6, "CARD-1006" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ProjectId",
                table: "Invoices",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ScheduleId",
                table: "Invoices",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_MaintenanceProjects_PropertyId",
                table: "MaintenanceProjects",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyApplications_PropertyId",
                table: "PropertyApplications",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_RentPayments_ScheduleId",
                table: "RentPayments",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_RentSchedules_TenantId",
                table: "RentSchedules",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_PropertyId",
                table: "Tenants",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_ProjectId",
                table: "WorkLogs",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "PropertyApplications");

            migrationBuilder.DropTable(
                name: "RentPayments");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WorkLogs");

            migrationBuilder.DropTable(
                name: "RentSchedules");

            migrationBuilder.DropTable(
                name: "MaintenanceProjects");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropTable(
                name: "Properties");
        }
    }
}
