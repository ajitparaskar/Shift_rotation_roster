using ShiftRotationAPI.DTOs;
using ShiftRotationAPI.Repositories.Interfaces;
using ShiftRotationAPI.Services.Interfaces;

namespace ShiftRotationAPI.Services.Implementations;

public class ReportService : IReportService
{
    private readonly IShiftAssignmentRepository _assignmentRepo;
    public ReportService(IShiftAssignmentRepository assignmentRepo) => _assignmentRepo = assignmentRepo;

    public async Task<List<ShiftAssignmentDto>> GetDailyScheduleAsync(DateOnly date)
    {
        var items = await _assignmentRepo.GetByDateAsync(date);
        return items.Select(ToDto).ToList();
    }

    public async Task<List<ShiftAssignmentDto>> GetWeeklyScheduleAsync(DateOnly weekStart)
    {
        var items = await _assignmentRepo.GetByDateRangeAsync(weekStart, weekStart.AddDays(6));
        return items.Select(ToDto).ToList();
    }

    public async Task<List<ShiftAssignmentDto>> GetMonthlyScheduleAsync(int year, int month)
    {
        var start = new DateOnly(year, month, 1);
        var end = start.AddMonths(1).AddDays(-1);
        var items = await _assignmentRepo.GetByDateRangeAsync(start, end);
        return items.Select(ToDto).ToList();
    }

    public async Task<List<ShiftAssignmentDto>> GetEmployeeHistoryAsync(int employeeId, DateOnly? start, DateOnly? end)
    {
        var items = await _assignmentRepo.GetByEmployeeAsync(employeeId, start, end);
        return items.Select(ToDto).ToList();
    }

    public async Task<List<ShiftAssignmentDto>> GetDepartmentScheduleAsync(int departmentId, DateOnly start, DateOnly end)
    {
        var items = await _assignmentRepo.GetByDateRangeAsync(start, end, departmentId);
        return items.Select(ToDto).ToList();
    }

    public async Task<Dictionary<string, int>> GetShiftUtilizationAsync(DateOnly start, DateOnly end)
    {
        var items = await _assignmentRepo.GetByDateRangeAsync(start, end);
        return items
            .GroupBy(a => a.Shift?.Name ?? "Unknown")
            .ToDictionary(g => g.Key, g => g.Count());
    }

    private static ShiftAssignmentDto ToDto(Models.ShiftAssignment a) => new()
    {
        Id = a.Id,
        EmployeeId = a.EmployeeId,
        EmployeeName = a.Employee?.FullName,
        ShiftId = a.ShiftId,
        ShiftName = a.Shift?.Name,
        Date = a.Date,
        Status = a.Status,
        OriginalEmployeeId = a.OriginalEmployeeId,
        Notes = a.Notes
    };
}
