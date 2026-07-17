namespace ShiftRotationAPI.Models;

public class ShiftPreference
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int ShiftId { get; set; }
    public Shift? Shift { get; set; }

    // 1 = most preferred, 3 = least preferred
    public int PreferenceLevel { get; set; } = 1;

    // Optional: preference tied to a specific day of week (null = applies every day)
    public DayOfWeek? DayOfWeek { get; set; }
}
