namespace ShiftRotationAPI.DTOs;

public class SchedulingPolicyDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MaxConsecutiveWorkingDays { get; set; }
    public int WeeklyOffDays { get; set; }
    public int MinimumRestHours { get; set; }
    public bool RequireExpertPerShift { get; set; }
    public bool IsActive { get; set; }
}

public class SchedulingPolicyCreateDto
{
    public string Name { get; set; } = "Default";
    public int MaxConsecutiveWorkingDays { get; set; } = 5;
    public int WeeklyOffDays { get; set; } = 2;
    public int MinimumRestHours { get; set; } = 8;
    public bool RequireExpertPerShift { get; set; } = true;
    public bool IsActive { get; set; } = true;
}
