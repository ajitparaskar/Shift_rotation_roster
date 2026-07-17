using ShiftRotationAPI.Models;

namespace ShiftRotationAPI.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        if (db.Departments.Any()) return; // already seeded

        var departments = new List<Department>
        {
            new() { Name = "Customer Support", Description = "Handles inbound support tickets" },
            new() { Name = "IT Operations", Description = "Infrastructure and systems" }
        };
        db.Departments.AddRange(departments);
        db.SaveChanges();

        var shifts = new List<Shift>
        {
            new() { Name = "Morning", StartTime = new TimeSpan(6,0,0), EndTime = new TimeSpan(14,0,0), Capacity = 3 },
            new() { Name = "Evening", StartTime = new TimeSpan(14,0,0), EndTime = new TimeSpan(22,0,0), Capacity = 3 },
            new() { Name = "Night",   StartTime = new TimeSpan(22,0,0), EndTime = new TimeSpan(6,0,0),  Capacity = 2 }
        };
        db.Shifts.AddRange(shifts);
        db.SaveChanges();

        var employees = new List<Employee>
        {
            new() { FullName = "Aarav Sharma", Email = "aarav.sharma@company.com", DepartmentId = departments[0].Id, HireDate = DateTime.UtcNow.AddYears(-2), ProficiencyLevel = ProficiencyLevel.Expert, IsNightEligible = true, Team = "L2 Support" },
            new() { FullName = "Priya Nair", Email = "priya.nair@company.com", DepartmentId = departments[0].Id, HireDate = DateTime.UtcNow.AddYears(-1), ProficiencyLevel = ProficiencyLevel.Intermediate, IsNightEligible = true, Team = "L1 Support" },
            new() { FullName = "Rohan Patel", Email = "rohan.patel@company.com", DepartmentId = departments[0].Id, HireDate = DateTime.UtcNow.AddMonths(-8), ProficiencyLevel = ProficiencyLevel.Trainee, IsNightEligible = false, Team = "L1 Support" },
            new() { FullName = "Sneha Iyer", Email = "sneha.iyer@company.com", DepartmentId = departments[1].Id, HireDate = DateTime.UtcNow.AddYears(-3), ProficiencyLevel = ProficiencyLevel.Expert, IsNightEligible = true, Team = "DBA" },
            new() { FullName = "Karan Mehta", Email = "karan.mehta@company.com", DepartmentId = departments[1].Id, HireDate = DateTime.UtcNow.AddMonths(-5), ProficiencyLevel = ProficiencyLevel.Trainee, IsNightEligible = false, Team = "Network" }
        };
        db.Employees.AddRange(employees);
        db.SaveChanges();

        if (!db.SchedulingPolicies.Any())
        {
            db.SchedulingPolicies.Add(new SchedulingPolicy
            {
                Name = "Default",
                MaxConsecutiveWorkingDays = 5,
                WeeklyOffDays = 2,
                MinimumRestHours = 8,
                RequireExpertPerShift = true,
                IsActive = true
            });
            db.SaveChanges();
        }

        var holidays = new List<Holiday>
        {
            new() { Name = "New Year's Day", Date = new DateOnly(DateTime.UtcNow.Year, 1, 1), IsRecurringYearly = true },
            new() { Name = "Independence Day", Date = new DateOnly(DateTime.UtcNow.Year, 8, 15), IsRecurringYearly = true }
        };
        db.Holidays.AddRange(holidays);
        db.SaveChanges();
    }
}
