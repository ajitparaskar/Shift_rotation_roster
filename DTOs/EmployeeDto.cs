using ShiftRotationAPI.Models;

namespace ShiftRotationAPI.DTOs;

public class EmployeeDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; }
    public ProficiencyLevel ProficiencyLevel { get; set; }
    public bool IsNightEligible { get; set; }
    public string? Team { get; set; }
    public int DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
}

public class EmployeeCreateDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public int DepartmentId { get; set; }
    public ProficiencyLevel ProficiencyLevel { get; set; } = ProficiencyLevel.Intermediate;
    public bool IsNightEligible { get; set; } = true;
    public string? Team { get; set; }
}
