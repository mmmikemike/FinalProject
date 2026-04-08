using Microsoft.EntityFrameworkCore;

namespace PropertyManagement.API.Data;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(AppDbContext dbContext)
    {
        await dbContext.Database.OpenConnectionAsync();

        try
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                """
                IF OBJECT_ID(N'dbo.Properties', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.Properties (
                        PropertyID INT IDENTITY(1,1) PRIMARY KEY,
                        PropertyName NVARCHAR(100) NOT NULL,
                        Address NVARCHAR(255) NOT NULL,
                        UnitNumber NVARCHAR(20) NOT NULL CONSTRAINT DF_Properties_UnitNumber DEFAULT('Main'),
                        MonthlyRent DECIMAL(18,2) NOT NULL CONSTRAINT DF_Properties_MonthlyRent DEFAULT(0)
                    );
                END;

                IF COL_LENGTH('dbo.Properties', 'Name') IS NOT NULL AND COL_LENGTH('dbo.Properties', 'PropertyName') IS NULL
                BEGIN
                    EXEC sp_rename 'dbo.Properties.Name', 'PropertyName', 'COLUMN';
                END;

                IF COL_LENGTH('dbo.Properties', 'UnitNumber') IS NULL
                BEGIN
                    ALTER TABLE dbo.Properties ADD UnitNumber NVARCHAR(20) NULL;
                END;

                IF COL_LENGTH('dbo.Properties', 'MonthlyRent') IS NULL
                BEGIN
                    ALTER TABLE dbo.Properties ADD MonthlyRent DECIMAL(18,2) NOT NULL CONSTRAINT DF_Properties_MonthlyRent_Legacy DEFAULT(0);
                END;

                UPDATE dbo.Properties
                SET UnitNumber = ISNULL(NULLIF(UnitNumber, ''), 'Main')
                WHERE UnitNumber IS NULL OR UnitNumber = '';

                IF OBJECT_ID(N'dbo.Tenants', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.Tenants (
                        TenantID INT IDENTITY(1,1) PRIMARY KEY,
                        FirstName NVARCHAR(50) NOT NULL,
                        LastName NVARCHAR(50) NOT NULL,
                        Email NVARCHAR(100) NOT NULL,
                        PhoneNumber NVARCHAR(20) NOT NULL,
                        PropertyID INT NOT NULL,
                        CONSTRAINT FK_Tenants_Properties FOREIGN KEY (PropertyID) REFERENCES dbo.Properties(PropertyID)
                    );
                END;

                IF OBJECT_ID(N'dbo.RentSchedules', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.RentSchedules (
                        ScheduleID INT IDENTITY(1,1) PRIMARY KEY,
                        TenantID INT NOT NULL,
                        DueDate DATE NOT NULL,
                        Status NVARCHAR(20) NOT NULL CONSTRAINT DF_RentSchedules_Status DEFAULT('Unpaid'),
                        BaseRent DECIMAL(18,2) NOT NULL,
                        LateFeeAccrued DECIMAL(18,2) NOT NULL CONSTRAINT DF_RentSchedules_LateFee DEFAULT(0),
                        ReminderCount INT NOT NULL CONSTRAINT DF_RentSchedules_ReminderCount DEFAULT(0),
                        CONSTRAINT FK_RentSchedules_Tenants FOREIGN KEY (TenantID) REFERENCES dbo.Tenants(TenantID)
                    );
                END;

                IF OBJECT_ID(N'dbo.RentPayments', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.RentPayments (
                        PaymentID INT IDENTITY(1,1) PRIMARY KEY,
                        ScheduleID INT NOT NULL,
                        PaymentDate DATETIME NOT NULL CONSTRAINT DF_RentPayments_PaymentDate DEFAULT(GETDATE()),
                        AmountPaid DECIMAL(18,2) NOT NULL,
                        PaymentMethod NVARCHAR(50) NOT NULL,
                        TransactionRef NVARCHAR(100) NULL,
                        CONSTRAINT FK_RentPayments_RentSchedules FOREIGN KEY (ScheduleID) REFERENCES dbo.RentSchedules(ScheduleID)
                    );
                END;

                IF OBJECT_ID(N'dbo.MaintenanceProjects', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.MaintenanceProjects (
                        ProjectID INT IDENTITY(1,1) PRIMARY KEY,
                        PropertyID INT NOT NULL,
                        ProjectTitle NVARCHAR(200) NOT NULL,
                        BidAmount DECIMAL(18,2) NOT NULL,
                        Status NVARCHAR(50) NOT NULL CONSTRAINT DF_MaintenanceProjects_Status DEFAULT('Bid'),
                        AssignedVendor NVARCHAR(100) NOT NULL,
                        CONSTRAINT FK_MaintenanceProjects_Properties FOREIGN KEY (PropertyID) REFERENCES dbo.Properties(PropertyID)
                    );
                END;

                IF COL_LENGTH('dbo.MaintenanceProjects', 'Status') IS NULL
                BEGIN
                    ALTER TABLE dbo.MaintenanceProjects ADD Status NVARCHAR(50) NOT NULL CONSTRAINT DF_MaintenanceProjects_Status_Legacy DEFAULT('Bid');
                END;

                IF OBJECT_ID(N'dbo.WorkLogs', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.WorkLogs (
                        LogID INT IDENTITY(1,1) PRIMARY KEY,
                        ProjectID INT NOT NULL,
                        ClockInTime DATETIME NOT NULL,
                        ClockOutTime DATETIME NULL,
                        GPSLocation NVARCHAR(100) NOT NULL,
                        ProofPhotoURL NVARCHAR(MAX) NULL,
                        MaterialsUsed NVARCHAR(MAX) NULL,
                        VendorSignature NVARCHAR(MAX) NULL,
                        CONSTRAINT FK_WorkLogs_MaintenanceProjects FOREIGN KEY (ProjectID) REFERENCES dbo.MaintenanceProjects(ProjectID)
                    );
                END;

                IF COL_LENGTH('dbo.WorkLogs', 'MaterialsUsed') IS NULL
                BEGIN
                    ALTER TABLE dbo.WorkLogs ADD MaterialsUsed NVARCHAR(MAX) NULL;
                END;

                IF COL_LENGTH('dbo.WorkLogs', 'VendorSignature') IS NULL
                BEGIN
                    ALTER TABLE dbo.WorkLogs ADD VendorSignature NVARCHAR(MAX) NULL;
                END;

                IF OBJECT_ID(N'dbo.Invoices', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.Invoices (
                        InvoiceID INT IDENTITY(1,1) PRIMARY KEY,
                        ProjectID INT NULL,
                        ScheduleID INT NULL,
                        InvoiceDate DATETIME NOT NULL CONSTRAINT DF_Invoices_InvoiceDate DEFAULT(GETDATE()),
                        TotalAmount DECIMAL(18,2) NOT NULL,
                        Status NVARCHAR(20) NOT NULL CONSTRAINT DF_Invoices_Status DEFAULT('Draft'),
                        IsExported BIT NOT NULL CONSTRAINT DF_Invoices_IsExported DEFAULT(0),
                        CONSTRAINT FK_Invoices_MaintenanceProjects FOREIGN KEY (ProjectID) REFERENCES dbo.MaintenanceProjects(ProjectID),
                        CONSTRAINT FK_Invoices_RentSchedules FOREIGN KEY (ScheduleID) REFERENCES dbo.RentSchedules(ScheduleID)
                    );
                END;

                IF COL_LENGTH('dbo.Invoices', 'ProjectID') IS NULL
                BEGIN
                    ALTER TABLE dbo.Invoices ADD ProjectID INT NULL;
                END;

                IF COL_LENGTH('dbo.Invoices', 'ScheduleID') IS NULL
                BEGIN
                    ALTER TABLE dbo.Invoices ADD ScheduleID INT NULL;
                END;

                IF COL_LENGTH('dbo.Invoices', 'InvoiceDate') IS NULL
                BEGIN
                    ALTER TABLE dbo.Invoices ADD InvoiceDate DATETIME NOT NULL CONSTRAINT DF_Invoices_InvoiceDate_Legacy DEFAULT(GETDATE());
                END;

                IF COL_LENGTH('dbo.Invoices', 'TotalAmount') IS NULL
                BEGIN
                    ALTER TABLE dbo.Invoices ADD TotalAmount DECIMAL(18,2) NOT NULL CONSTRAINT DF_Invoices_TotalAmount_Legacy DEFAULT(0);
                END;

                IF COL_LENGTH('dbo.Invoices', 'Status') IS NULL
                BEGIN
                    ALTER TABLE dbo.Invoices ADD Status NVARCHAR(20) NOT NULL CONSTRAINT DF_Invoices_Status_Legacy DEFAULT('Draft');
                END;

                IF COL_LENGTH('dbo.Invoices', 'IsExported') IS NULL
                BEGIN
                    ALTER TABLE dbo.Invoices ADD IsExported BIT NOT NULL CONSTRAINT DF_Invoices_IsExported_Legacy DEFAULT(0);
                END;

                IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Invoices_MaintenanceProjects')
                BEGIN
                    ALTER TABLE dbo.Invoices
                    ADD CONSTRAINT FK_Invoices_MaintenanceProjects FOREIGN KEY (ProjectID) REFERENCES dbo.MaintenanceProjects(ProjectID);
                END;

                IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Invoices_RentSchedules')
                BEGIN
                    ALTER TABLE dbo.Invoices
                    ADD CONSTRAINT FK_Invoices_RentSchedules FOREIGN KEY (ScheduleID) REFERENCES dbo.RentSchedules(ScheduleID);
                END;

                IF OBJECT_ID(N'dbo.PropertyApplications', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.PropertyApplications (
                        ApplicationID INT IDENTITY(1,1) PRIMARY KEY,
                        PropertyID INT NOT NULL,
                        ApplicantFirstName NVARCHAR(50) NOT NULL,
                        ApplicantLastName NVARCHAR(50) NOT NULL,
                        Email NVARCHAR(100) NOT NULL,
                        PhoneNumber NVARCHAR(20) NOT NULL,
                        PreferredMoveInDate DATE NOT NULL,
                        MonthlyIncome DECIMAL(18,2) NOT NULL CONSTRAINT DF_PropertyApplications_MonthlyIncome DEFAULT(0),
                        HouseholdSize INT NOT NULL CONSTRAINT DF_PropertyApplications_HouseholdSize DEFAULT(1),
                        CurrentEmployer NVARCHAR(100) NOT NULL,
                        PetsDescription NVARCHAR(200) NOT NULL CONSTRAINT DF_PropertyApplications_Pets DEFAULT(''),
                        Notes NVARCHAR(MAX) NOT NULL CONSTRAINT DF_PropertyApplications_Notes DEFAULT(''),
                        Status NVARCHAR(20) NOT NULL CONSTRAINT DF_PropertyApplications_Status DEFAULT('New'),
                        SubmittedAt DATETIME NOT NULL CONSTRAINT DF_PropertyApplications_SubmittedAt DEFAULT(GETDATE()),
                        CONSTRAINT FK_PropertyApplications_Properties FOREIGN KEY (PropertyID) REFERENCES dbo.Properties(PropertyID)
                    );
                END;

                """);

            await dbContext.Database.ExecuteSqlRawAsync(
                """
                IF NOT EXISTS (SELECT 1 FROM dbo.Properties)
                BEGIN
                    INSERT INTO dbo.Properties (PropertyName, Address, UnitNumber, MonthlyRent)
                    VALUES
                        ('Sunrise Apts', '123 Maple St', '1A', 1200.00),
                        ('Sunrise Apts', '123 Maple St', '1B', 1250.00),
                        ('Oak Estates', '456 Oak Ave', 'Unit 10', 2000.00),
                        ('Oak Estates', '456 Oak Ave', 'Unit 11', 2100.00),
                        ('River View', '789 River Rd', '302', 1500.00),
                        ('River View', '789 River Rd', '303', 1550.00);
                END;

                IF NOT EXISTS (SELECT 1 FROM dbo.Tenants)
                BEGIN
                    INSERT INTO dbo.Tenants (FirstName, LastName, Email, PhoneNumber, PropertyID)
                    VALUES
                        ('John', 'Doe', 'john@example.com', '555-0101', 1),
                        ('Jane', 'Smith', 'jane@example.com', '555-0102', 2),
                        ('Mike', 'Jones', 'mike@example.com', '555-0103', 3),
                        ('Sarah', 'Wilson', 'sarah@example.com', '555-0104', 4),
                        ('Alex', 'Brown', 'alex@example.com', '555-0105', 5),
                        ('Chris', 'Davis', 'chris@example.com', '555-0106', 6);
                END;
                """);

            await dbContext.Database.ExecuteSqlRawAsync(
                """
                IF NOT EXISTS (SELECT 1 FROM dbo.RentSchedules)
                BEGIN
                    INSERT INTO dbo.RentSchedules (TenantID, DueDate, Status, BaseRent, LateFeeAccrued, ReminderCount)
                    VALUES
                        (1, '2026-04-01', 'Paid', 1200.00, 0.00, 0),
                        (2, '2026-04-01', 'Unpaid', 1250.00, 0.00, 1),
                        (3, '2026-04-01', 'Late', 2000.00, 150.00, 3),
                        (4, '2026-04-01', 'Unpaid', 2100.00, 0.00, 0),
                        (5, '2026-04-01', 'Paid', 1500.00, 0.00, 0),
                        (6, '2026-04-01', 'Partial', 1550.00, 50.00, 2);
                END;

                IF NOT EXISTS (SELECT 1 FROM dbo.RentPayments)
                BEGIN
                    INSERT INTO dbo.RentPayments (ScheduleID, PaymentDate, AmountPaid, PaymentMethod, TransactionRef)
                    VALUES
                        (1, '2026-03-31T10:30:00', 1200.00, 'ACH', 'ACH-1001'),
                        (5, '2026-03-31T11:00:00', 1500.00, 'Card', 'CARD-1005'),
                        (6, '2026-04-02T09:15:00', 500.00, 'Card', 'CARD-1006');
                END;
                """);

            await dbContext.Database.ExecuteSqlRawAsync(
                """
                IF NOT EXISTS (SELECT 1 FROM dbo.MaintenanceProjects)
                BEGIN
                    INSERT INTO dbo.MaintenanceProjects (PropertyID, ProjectTitle, BidAmount, Status, AssignedVendor)
                    VALUES
                        (1, 'Broken Faucet', 150.00, 'Closed', 'Fix-It Plumbing'),
                        (2, 'Paint Bedroom', 400.00, 'Invoiced', 'Pro Painters'),
                        (3, 'AC Repair', 800.00, 'Work Order', 'CoolAir Inc'),
                        (4, 'Roof Leak', 2500.00, 'Bid', 'TopRoofing'),
                        (5, 'Floor Buffing', 300.00, 'Approved', 'Janitor Pro'),
                        (6, 'Door Lock Fix', 100.00, 'Closed', 'SafeLocks');
                END;

                IF NOT EXISTS (SELECT 1 FROM dbo.WorkLogs)
                BEGIN
                    INSERT INTO dbo.WorkLogs (ProjectID, ClockInTime, ClockOutTime, GPSLocation, ProofPhotoURL, MaterialsUsed, VendorSignature)
                    VALUES
                        (1, '2026-01-10T09:00:00', '2026-01-10T10:30:00', '34.05,-118.24', 'img01.jpg', 'Replacement faucet kit', 'Fix-It Plumbing'),
                        (6, '2026-01-12T14:00:00', '2026-01-12T14:45:00', '34.06,-118.25', 'img02.jpg', 'Lock cylinder', 'SafeLocks'),
                        (3, '2026-04-03T08:30:00', '2026-04-03T11:00:00', '34.07,-118.22', 'ac-repair.jpg', 'Coolant and fan motor', 'CoolAir Inc');
                END;
                """);

            await dbContext.Database.ExecuteSqlRawAsync(
                """
                IF NOT EXISTS (SELECT 1 FROM dbo.Invoices)
                BEGIN
                    INSERT INTO dbo.Invoices (ProjectID, ScheduleID, InvoiceDate, TotalAmount, Status, IsExported)
                    VALUES
                        (1, NULL, '2026-01-11T12:00:00', 150.00, 'Paid', 1),
                        (2, NULL, '2026-02-14T12:00:00', 400.00, 'Sent', 0),
                        (NULL, 2, '2026-04-03T09:00:00', 1250.00, 'Overdue', 0),
                        (6, NULL, '2026-01-13T12:00:00', 100.00, 'Paid', 1);
                END;
                """);
        }
        finally
        {
            await dbContext.Database.CloseConnectionAsync();
        }
    }
}
