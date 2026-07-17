using ShiftRotationAPI.DTOs;

namespace ShiftRotationAPI.Services.Interfaces;

public interface IManualOverrideService
{
    /// <summary>
    /// Called when an employee reports a sudden/on-spot leave. Vacates their
    /// shift(s) for that date and suggests replacement candidates — the lead
    /// picks manually via ReassignAsync, nothing is auto-assigned.
    /// </summary>
    Task<EmergencyLeaveResultDto> ReportEmergencyLeaveAsync(EmergencyLeaveDto dto);

    Task<List<ReplacementCandidateDto>> GetReplacementCandidatesAsync(int assignmentId);

    /// <summary>
    /// Manager/lead's manual decision — assigns a specific employee to a
    /// specific (usually vacant) assignment slot. Always allowed; this is
    /// an intentional override point, not run through the engine's rules.
    /// </summary>
    Task<ShiftAssignmentDto> ReassignAsync(ReassignDto dto);
}
