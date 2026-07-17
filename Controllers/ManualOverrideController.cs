using Microsoft.AspNetCore.Mvc;
using ShiftRotationAPI.DTOs;
using ShiftRotationAPI.Services.Interfaces;

namespace ShiftRotationAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ManualOverrideController : ControllerBase
{
    private readonly IManualOverrideService _service;
    public ManualOverrideController(IManualOverrideService service) => _service = service;

    /// <summary>
    /// Step 1: employee reports a sudden/on-spot leave. Vacates their shift(s)
    /// for that date and returns suggested replacements — nothing is auto-assigned.
    /// </summary>
    [HttpPost("emergency-leave")]
    public async Task<ActionResult<EmergencyLeaveResultDto>> ReportEmergencyLeave(EmergencyLeaveDto dto) =>
        Ok(await _service.ReportEmergencyLeaveAsync(dto));

    /// <summary>
    /// Re-pull the ranked candidate list for a specific vacant assignment.
    /// </summary>
    [HttpGet("candidates/{assignmentId}")]
    public async Task<ActionResult<List<ReplacementCandidateDto>>> GetCandidates(int assignmentId) =>
        Ok(await _service.GetReplacementCandidatesAsync(assignmentId));

    /// <summary>
    /// Step 2: lead/manager's manual decision — always allowed, not run
    /// through the engine's automatic rules. Works for vacant slots (emergency
    /// replacement) or any existing assignment (general manual override).
    /// </summary>
    [HttpPost("reassign")]
    public async Task<ActionResult<ShiftAssignmentDto>> Reassign(ReassignDto dto)
    {
        try
        {
            return Ok(await _service.ReassignAsync(dto));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
