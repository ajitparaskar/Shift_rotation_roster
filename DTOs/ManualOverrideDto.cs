namespace ShiftRotationAPI.DTOs;

public class EmergencyLeaveDto
{
    public int EmployeeId { get; set; }
    public DateOnly Date { get; set; }
    public string? Reason { get; set; }
}

public class ReassignDto
{
    public int AssignmentId { get; set; }
    public int NewEmployeeId { get; set; }
    public string? Notes { get; set; }
}

public class ReplacementCandidateDto
{
    public int EmployeeId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string ProficiencyLevel { get; set; } = string.Empty;
    public int ShiftsWorkedThisWeek { get; set; }
    public bool PrefersThisShift { get; set; }
}

public class EmergencyLeaveResultDto
{
    public List<ShiftAssignmentDto> VacatedAssignments { get; set; } = new();
    public List<ReplacementCandidateDto> SuggestedReplacements { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}
