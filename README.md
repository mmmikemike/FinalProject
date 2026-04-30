# Property Management System

A full-stack property management web application built with ASP.NET Core 9 Web API and Blazor WebAssembly. The system supports multiple user roles and provides tools for managing properties, tenants, rent schedules, maintenance projects, invoices, and notifications.

Group Members: Mike Mayatskiy, Joe Jahshan, Aiden LeClaire

---

## Tech Stack

- **Frontend:** Blazor WebAssembly (.NET 9)
- **Backend:** ASP.NET Core 9 Web API
- **Database:** SQL Server with Entity Framework Core
---

## Prerequisites

- Visual Studio 2022 or later
- .NET 9 SDK
- SQL Server (local or remote instance)
- dotnet-ef tools (`dotnet tool install --global dotnet-ef`)

---

## Setup Instructions

1. **Clone the repository**
2. **Configure the database connection**
   
   Open `PropertyManagement.API/appsettings.json` and update the connection string to point to your SQL Server instance:
```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=PropertyManagement;Trusted_Connection=True;"
   }
```


3. **Run database migrations**
   
   Navigate to the API project folder and run:
   ```
   dotnet ef database update
   ```
This will create all tables and seed the default users automatically.

5. **Run the API**
   
   Start the API project first. It will run on `https://localhost:7027` by default.

6. **Run the Blazor app**
   
   Start the Blazor project. It will run on `https://localhost:7274` by default.

---

## Default Logins

| Username | Password | Role |
|---|---|---|
| ADMIN | ADMIN | Admin |
| STAFF | STAFF | Staff |
| CONTRACTOR | CONTRACTOR | Contractor |
| TENANT1 | TENANT1 | Tenant |
| TENANT2 | TENANT2 | Tenant |
| TENANT3 | TENANT3 | Tenant |
| TENANT4 | TENANT4 | Tenant |
| TENANT5 | TENANT5 | Tenant |
| TENANT6 | TENANT6 | Tenant |

---

## Role Permissions

| Feature | Admin | Staff | Contractor | Tenant |
|---|---|---|---|---|
| Properties | ✅ Full access | ❌ | ❌ | ❌ |
| Tenants | ✅ Full access | ✅ View & edit | ❌ | ❌ |
| Rent Schedules | ✅ Full access | ✅ View & edit | ❌ | ❌ |
| Rent Payments | ✅ Full access | ✅ View & edit | ❌ | ✅ Own payments |
| Maintenance Projects | ✅ Full access | ✅ View & edit | ✅ Assigned only | ❌ |
| Work Logs | ✅ Full access | ✅ View & edit | ✅ Assigned only | ❌ |
| Invoices | ✅ Full access | ❌ | ✅ Upload only | ❌ |
| Notifications | ✅ Send & view | ✅ Send & view | ❌ | ❌ |
| Applications | ✅ Full access | ✅ View & edit | ❌ | ✅ Submit |

---

## Features

- **Property Management** — create and manage rental properties
- **Tenant Management** — track tenants and their assigned properties
- **Rent Schedules & Payments** — manage due dates, amounts, and payment history
- **Maintenance Projects** — assign contractors to projects and track progress
- **Work Logs** — log hours and work completed on maintenance projects
- **Invoices** — create and track billing for maintenance and rent-related charges
- **Notifications** — send rent reminders, eviction notices, and invoice alerts to tenants
- **Property Applications** — manage incoming rental applications

---

## Project Structure
```
FinalProject/
├── PropertyManagement.API/         # ASP.NET Core Web API
│   ├── Controllers/                # API endpoints
│   ├── Models/                     # Database entity models
│   ├── Contracts/                  # Request and response DTOs
│   ├── Data/                       # DbContext and database initializer
│   └── Migrations/                 # EF Core migrations
│
└── PropertyManagement.Blazor/      # Blazor WebAssembly frontend
├── Pages/                      # Razor pages
├── Models/                     # Client-side models
└── Services/                   # API client and auth services
```
