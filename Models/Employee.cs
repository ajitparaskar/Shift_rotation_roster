namespace ShiftRotationAPI.Models;

public enum ProficiencyLevel { Trainee, Intermediate, Expert }

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; } = true;

    // Enterprise scheduling fields
    public ProficiencyLevel ProficiencyLevel { get; set; } = ProficiencyLevel.Intermediate;
    public bool IsNightEligible { get; set; } = true;
    public string? Team { get; set; } // e.g. L1 Support, L2 Support, DBA, Network

    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    public ICollection<ShiftPreference> ShiftPreferences { get; set; } = new List<ShiftPreference>();
    public ICollection<ShiftAssignment> ShiftAssignments { get; set; } = new List<ShiftAssignment>();
}
