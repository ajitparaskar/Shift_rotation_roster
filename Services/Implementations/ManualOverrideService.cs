using ShiftRotationAPI.DTOs;
using ShiftRotationAPI.Models;
using ShiftRotationAPI.Repositories.Interfaces;
using ShiftRotationAPI.Services.Interfaces;

namespace ShiftRotationAPI.Services.Implementations;

public class ManualOverrideService : IManualOverrideService
{
    private readonly IShiftAssignmentRepository _assignmentRepo;
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IShiftRepository _shiftRepo;
    private readonly ILeaveRequestRepository _leaveRepo;
    private readonly IShiftPreferenceRepository _preferenceRepo;

    public ManualOverrideService(
        IShiftAssignmentRepository assignmentRepo,
        IEmployeeRepository employeeRepo,
        IShiftRepository shiftRepo,
        ILeaveRequestRepository leaveRepo,
        IShiftPreferenceRepository preferenceRepo)
    {
        _assignmentRepo = assignmentRepo;
        _employeeRepo = employeeRepo;
        _shiftRepo = shiftRepo;
        _leaveRepo = leaveRepo;
        _preferenceRepo = preferenceRepo;
    }

    public async Task<EmergencyLeaveResultDto> ReportEmergencyLeaveAsync(EmergencyLeaveDto dto)
    {
        var result = new EmergencyLeaveResultDto();

        var todaysAssignments = await _assignmentRepo.GetByEmployeeAsync(dto.EmployeeId, dto.Date, dto.Date);
        var scheduledToday = todaysAssignments.Where(a => a.Status != AssignmentStatus.Vacant).ToList();

        if (!scheduledToday.Any())
        {
            result.Message = "No shift assignment found for this employee on this date — nothing to vacate.";
            return result;
        }

        // Log an ad-hoc approved leave record for the audit trail
        await _leaveRepo.AddAsync(new LeaveRequest
        {
            EmployeeId = dto.EmployeeId,
            StartDate = dto.Date.ToDateTime(TimeOnly.MinValue),
            EndDate = dto.Date.ToDateTime(TimeOnly.MinValue),
            Reason = dto.Reason ?? "Emergency / on-spot leave",
            Status = LeaveStatus.Approved
        });

        foreach (var assignment in scheduledToday)
        {
            assignment.OriginalEmployeeId = assignment.EmployeeId;
            assignment.EmployeeId = null;
            assignment.Status = AssignmentStatus.Vacant;
            assignment.Notes = dto.Reason ?? "Emergency leave — awaiting manual replacement";
            await _assignmentRepo.UpdateAsync(assignment);
        }

        var employee = await _employeeRepo.GetByIdAsync(dto.EmployeeId);
        var shift = await _shiftRepo.GetByIdAsync(scheduledToday.First().ShiftId);

        result.VacatedAssignments = scheduledToday.Select(a => new ShiftAssignmentDto
        {
            Id = a.Id,
            EmployeeId = null,
            ShiftId = a.ShiftId,
            ShiftName = shift?.Name,
            Date = a.Date,
            Status = AssignmentStatus.Vacant,
            OriginalEmployeeId = a.OriginalEmployeeId,
            OriginalEmployeeName = employee?.FullName,
            Notes = a.Notes
        }).ToList();

        result.SuggestedReplacements = await GetReplacementCandidatesAsync(scheduledToday.First().Id);
        result.Message = $"{scheduledToday.Count} shift(s) vacated for {employee?.FullName} on {dto.Date:yyyy-MM-dd}. " +
                          "Review suggested replacements and call /api/manualoverride/reassign with your choice.";

        return result;
    }

    public async Task<List<ReplacementCandidateDto>> GetReplacementCandidatesAsync(int assignmentId)
    {
        var assignment = await _assignmentRepo.GetByIdAsync(assignmentId);
        if (assignment == null) return new List<ReplacementCandidateDto>();

        var shift = await _shiftRepo.GetByIdAsync(assignment.ShiftId);
        if (shift == null) return new List<ReplacementCandidateDto>();

        // Scope candidates to the same department as the originally-assigned employee
        int? departmentId = null;
        if (assignment.OriginalEmployeeId.HasValue)
        {
            var originalEmployee = await _employeeRepo.GetByIdAsync(assignment.OriginalEmployeeId.Value);
            departmentId = originalEmployee?.DepartmentId;
        }

        var pool = await _employeeRepo.GetActiveEmployeesAsync(departmentId);

        var onLeaveToday = (await _leaveRepo.GetApprovedLeaveInRangeAsync(assignment.Date, assignment.Date))
            .Select(l => l.EmployeeId).ToHashSet();

        var assignedElsewhereToday = (await _assignmentRepo.GetByDateAsync(assignment.Date))
            .Where(a => a.Id != assignment.Id && a.EmployeeId.HasValue)
            .Select(a => a.EmployeeId!.Value).ToHashSet();

        bool isNightShift = shift.Name.Equals("Night", StringComparison.OrdinalIgnoreCase);

        var allPreferences = await _preferenceRepo.GetAllAsync();

        var weekStart = assignment.Date.AddDays(-6);
        var candidates = new List<ReplacementCandidateDto>();

        foreach (var emp in pool)
        {
            if (onLeaveToday.Contains(emp.Id)) continue;
            if (assignedElsewhereToday.Contains(emp.Id)) continue;
            if (isNightShift && !emp.IsNightEligible) continue;

            var recentAssignments = await _assignmentRepo.GetByEmployeeAsync(emp.Id, weekStart, assignment.Date);
            var shiftsThisWeek = recentAssignments.Count(a => a.Status != AssignmentStatus.Vacant);

            var prefers = allPreferences.Any(p => p.EmployeeId == emp.Id && p.ShiftId == shift.Id &&
                                                   (p.DayOfWeek == null || p.DayOfWeek == assignment.Date.DayOfWeek));

            candidates.Add(new ReplacementCandidateDto
            {
                EmployeeId = emp.Id,
                FullName = emp.FullName,
                ProficiencyLevel = emp.ProficiencyLevel.ToString(),
                ShiftsWorkedThisWeek = shiftsThisWeek,
                PrefersThisShift = prefers
            });
        }

        return candidates
            .OrderBy(c => c.ShiftsWorkedThisWeek)      // fairness first
            .ThenByDescending(c => c.PrefersThisShift)  // preference as tiebreaker
            .Take(8)
            .ToList();
    }

    public async Task<ShiftAssignmentDto> ReassignAsync(ReassignDto dto)
    {
        var assignment = await _assignmentRepo.GetByIdAsync(dto.AssignmentId);
        if (assignment == null)
            throw new InvalidOperationException("Assignment not found.");

        var newEmployee = await _employeeRepo.GetByIdAsync(dto.NewEmployeeId);
        if (newEmployee == null)
            throw new InvalidOperationException("Replacement employee not found.");

        assignment.EmployeeId = dto.NewEmployeeId;
        assignment.Status = AssignmentStatus.ManuallyAssigned;
        assignment.Notes = dto.Notes ?? assignment.Notes ?? "Manually reassigned by lead/manager";
        await _assignmentRepo.UpdateAsync(assignment);

        var shift = await _shiftRepo.GetByIdAsync(assignment.ShiftId);

        return new ShiftAssignmentDto
        {
            Id = assignment.Id,
            EmployeeId = assignment.EmployeeId,
            EmployeeName = newEmployee.FullName,
            ShiftId = assignment.ShiftId,
            ShiftName = shift?.Name,
            Date = assignment.Date,
            Status = assignment.Status,
            OriginalEmployeeId = assignment.OriginalEmployeeId,
            Notes = assignment.Notes
        };
    }
}
