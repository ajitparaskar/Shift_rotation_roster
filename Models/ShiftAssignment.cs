namespace ShiftRotationAPI.Models;

public enum AssignmentStatus { Scheduled, Vacant, ManuallyAssigned }

public class ShiftAssignment
{
    public int Id { get; set; }

    // Nullable: null + Status=Vacant means the slot needs a manager to pick someone
    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    public int ShiftId { get; set; }
    public Shift? Shift { get; set; }

    public DateOnly Date { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public AssignmentStatus Status { get; set; } = AssignmentStatus.Scheduled;

    // Audit trail: who was originally scheduled before an emergency-leave swap
    public int? OriginalEmployeeId { get; set; }
    public string? Notes { get; set; }
}
