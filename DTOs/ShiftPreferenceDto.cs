namespace ShiftRotationAPI.DTOs;

public class ShiftPreferenceDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int ShiftId { get; set; }
    public int PreferenceLevel { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
}

public class ShiftPreferenceCreateDto
{
    public int EmployeeId { get; set; }
    public int ShiftId { get; set; }
    public int PreferenceLevel { get; set; } = 1;
    public DayOfWeek? DayOfWeek { get; set; }
}
