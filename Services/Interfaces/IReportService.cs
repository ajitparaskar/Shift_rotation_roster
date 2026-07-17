using ShiftRotationAPI.DTOs;

namespace ShiftRotationAPI.Services.Interfaces;

public interface IReportService
{
    Task<List<ShiftAssignmentDto>> GetDailyScheduleAsync(DateOnly date);
    Task<List<ShiftAssignmentDto>> GetWeeklyScheduleAsync(DateOnly weekStart);
    Task<List<ShiftAssignmentDto>> GetMonthlyScheduleAsync(int year, int month);
    Task<List<ShiftAssignmentDto>> GetEmployeeHistoryAsync(int employeeId, DateOnly? start, DateOnly? end);
    Task<List<ShiftAssignmentDto>> GetDepartmentScheduleAsync(int departmentId, DateOnly start, DateOnly end);
    Task<Dictionary<string, int>> GetShiftUtilizationAsync(DateOnly start, DateOnly end);
}
