using Microsoft.AspNetCore.Mvc;
using ShiftRotationAPI.DTOs;
using ShiftRotationAPI.Services.Interfaces;

namespace ShiftRotationAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IReportService _service;
    public ReportController(IReportService service) => _service = service;

    [HttpGet("daily/{date}")]
    public async Task<ActionResult<List<ShiftAssignmentDto>>> Daily(DateOnly date) =>
        Ok(await _service.GetDailyScheduleAsync(date));

    [HttpGet("weekly/{weekStart}")]
    public async Task<ActionResult<List<ShiftAssignmentDto>>> Weekly(DateOnly weekStart) =>
        Ok(await _service.GetWeeklyScheduleAsync(weekStart));

    [HttpGet("monthly/{year}/{month}")]
    public async Task<ActionResult<List<ShiftAssignmentDto>>> Monthly(int year, int month) =>
        Ok(await _service.GetMonthlyScheduleAsync(year, month));

    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<List<ShiftAssignmentDto>>> EmployeeHistory(
        int employeeId, [FromQuery] DateOnly? start, [FromQuery] DateOnly? end) =>
        Ok(await _service.GetEmployeeHistoryAsync(employeeId, start, end));

    [HttpGet("department/{departmentId}")]
    public async Task<ActionResult<List<ShiftAssignmentDto>>> DepartmentSchedule(
        int departmentId, [FromQuery] DateOnly start, [FromQuery] DateOnly end) =>
        Ok(await _service.GetDepartmentScheduleAsync(departmentId, start, end));

    [HttpGet("utilization")]
    public async Task<ActionResult<Dictionary<string, int>>> Utilization(
        [FromQuery] DateOnly start, [FromQuery] DateOnly end) =>
        Ok(await _service.GetShiftUtilizationAsync(start, end));
}
