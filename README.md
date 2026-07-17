# Shift Rotation API — v1.0 (MVP)

Automated employee shift scheduling REST API built with ASP.NET Core 8, EF Core, and SQL Server.

## What this is
A backend-only system that takes employees, shifts, leave requests and preferences,
and automatically generates a fair, rule-compliant shift schedule (the "Rotation Engine"),
plus reporting endpoints to view the generated schedules.

## Prerequisites
- .NET 8 SDK
- SQL Server (local instance, Docker container, or Azure SQL)
- VS Code (with the "REST Client" extension) or Visual Studio / Postman

## Setup & Run

1. **Restore packages**
   ```bash
   cd ShiftRotationAPI
   dotnet restore
   ```

2. **Set your connection string**
   Edit `appsettings.json` → `ConnectionStrings:DefaultConnection` to point at your SQL Server instance.

3. **Create the database (EF Core migrations)**
   ```bash
   dotnet tool install --global dotnet-ef   # first time only
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
   (The app also auto-applies migrations and seeds sample data on startup, so this step
   is optional once you've generated the first migration.)

4. **Run the API**
   ```bash
   dotnet run
   ```

5. **Open Swagger**
   Navigate to `https://localhost:<port>/swagger` (port is printed in the console on startup).

6. **Test endpoints**
   Open any `.http` file under `Tests/` in VS Code (with the REST Client extension) and
   click "Send Request" above each request. Update `@baseUrl` to match your actual port.

## Project Structure
```
ShiftRotationAPI/
├── Controllers/       REST endpoints (Department, Employee, Shift, Leave,
│                       Preference, Holiday, Rotation, Report)
├── Services/           Business logic layer (Interfaces + Implementations)
│   └── RotationService.cs   <- the core scheduling algorithm
├── Repositories/       Data access layer (Interfaces + Implementations)
├── Models/              EF Core entities
├── DTOs/                Request/response contracts (keeps entities out of the API surface)
├── Data/                AppDbContext + SeedData
├── Tests/               .http files for manual API testing
└── Program.cs            App startup, DI wiring, Swagger, auto-migrate
```

## How the Rotation Engine works
For every day in the requested date range:
1. Skip the day if it's a holiday.
2. Find employees on approved leave that day — they're excluded.
3. For each shift (ordered by start time), build a candidate list of employees who are:
   active, not on leave, not already assigned another shift that day, and who have had
   at least the configured minimum rest (default 8h) since their last shift ended.
4. Rank candidates by fairness first (fewest total assignments so far this run wins),
   then by stated shift preference as a tiebreaker.
5. Assign the top N candidates, where N = shift capacity.
6. Save all assignments; report any shifts that couldn't be fully staffed.

This guarantees: no double-booking, no leave conflicts, minimum rest between shifts,
capacity limits respected, and an even rotation across the team over time.

## API Modules
| Module | Endpoints |
|---|---|
| Department | CRUD |
| Employee | CRUD + get by department + deactivate |
| Shift | CRUD |
| Leave | CRUD + approve/reject |
| Preference | Create/list/delete shift preferences per employee |
| Holiday | CRUD |
| Rotation | `POST /api/rotation/generate` — runs the engine |
| Report | Daily / Weekly / Monthly / Employee history / Department schedule / Shift utilization |

## Not included in v1 (by design)
React frontend, JWT authentication, email notifications, AI, background jobs, deployment config.
