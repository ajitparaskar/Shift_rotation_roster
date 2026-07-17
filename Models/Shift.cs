namespace ShiftRotationAPI.Models;

public class Shift
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty; // e.g. Morning, Evening, Night
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int Capacity { get; set; }

    public ICollection<ShiftAssignment> ShiftAssignments { get; set; } = new List<ShiftAssignment>();
    public ICollection<ShiftPreference> ShiftPreferences { get; set; } = new List<ShiftPreference>();
}
