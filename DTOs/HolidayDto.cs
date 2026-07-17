namespace ShiftRotationAPI.DTOs;

public class HolidayDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public bool IsRecurringYearly { get; set; }
}

public class HolidayCreateDto
{
    public string Name { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public bool IsRecurringYearly { get; set; }
}
