using Microsoft.AspNetCore.Mvc;
using ShiftRotationAPI.DTOs;
using ShiftRotationAPI.Services.Interfaces;

namespace ShiftRotationAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RotationController : ControllerBase
{
    private readonly IRotationService _service;
    public RotationController(IRotationService service) => _service = service;

    /// <summary>
    /// Runs the rotation engine for a date range (and optional department),
    /// automatically generating and saving fair shift assignments.
    /// </summary>
    [HttpPost("generate")]
    public async Task<ActionResult<RotationResultDto>> Generate(RotationRequestDto request)
    {
        try
        {
            var result = await _service.GenerateRotationAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Generates a full Monday–Sunday week schedule. Pass any date inside the
    /// target week (or omit it to use the current week) — no need to calculate
    /// Monday/Sunday yourself.
    /// </summary>
    [HttpPost("generate-week")]
    public async Task<ActionResult<RotationResultDto>> GenerateWeek(WeeklyRotationRequestDto request)
    {
        try
        {
            var result = await _service.GenerateWeeklyRotationAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
