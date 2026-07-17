namespace ShiftRotationAPI.DTOs;

public class RotationRequestDto
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int? DepartmentId { get; set; }
}

public class WeeklyRotationRequestDto
{
    // Any date that falls within the target week. If omitted, defaults to
    // the current week (server date). The engine snaps this to the Monday
    // of that week automatically — you never need to compute Monday/Sunday yourself.
    public DateOnly? WeekOf { get; set; }
    public int? DepartmentId { get; set; }
}

public class RotationResultDto
{
    public DateOnly? WeekStartDate { get; set; } // Monday, set only when generated via the weekly endpoint
    public DateOnly? WeekEndDate { get; set; }    // Sunday
    public int TotalDaysProcessed { get; set; }
    public int TotalAssignmentsCreated { get; set; }
    public int TotalUnfilledSlots { get; set; }
    public List<string> Warnings { get; set; } = new();
    public List<ShiftAssignmentDto> Assignments { get; set; } = new();
}
