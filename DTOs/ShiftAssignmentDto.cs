using ShiftRotationAPI.Models;

namespace ShiftRotationAPI.DTOs;

public class ShiftAssignmentDto
{
    public int Id { get; set; }
    public int? EmployeeId { get; set; }
    public string? EmployeeName { get; set; }
    public int ShiftId { get; set; }
    public string? ShiftName { get; set; }
    public DateOnly Date { get; set; }
    public AssignmentStatus Status { get; set; }
    public int? OriginalEmployeeId { get; set; }
    public string? OriginalEmployeeName { get; set; }
    public string? Notes { get; set; }
}
