namespace ShiftRotationAPI.Models;

/// <summary>
/// Configurable labor rules for the Rotation Engine. Instead of hardcoding
/// "5 working days" / "2 weekly offs" in code, these live in the database so
/// different organizations (or departments) can tune them without a redeploy.
/// </summary>
public class SchedulingPolicy
{
    public int Id { get; set; }
    public string Name { get; set; } = "Default";

    public int MaxConsecutiveWorkingDays { get; set; } = 5;
    public int WeeklyOffDays { get; set; } = 2;
    public int MinimumRestHours { get; set; } = 8;

    // Skill-mix rule: require at least one Expert on every shift when possible
    public bool RequireExpertPerShift { get; set; } = true;

    public bool IsActive { get; set; } = true;
}
