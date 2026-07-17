using Microsoft.AspNetCore.Mvc;
using ShiftRotationAPI.DTOs;
using ShiftRotationAPI.Services.Interfaces;

namespace ShiftRotationAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PreferenceController : ControllerBase
{
    private readonly IShiftPreferenceService _service;
    public PreferenceController(IShiftPreferenceService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<ShiftPreferenceDto>>> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("employee/{employeeId}")]
    public async Task<ActionResult<List<ShiftPreferenceDto>>> GetByEmployee(int employeeId) =>
        Ok(await _service.GetByEmployeeAsync(employeeId));

    [HttpPost]
    public async Task<ActionResult<ShiftPreferenceDto>> Create(ShiftPreferenceCreateDto dto) =>
        Ok(await _service.CreateAsync(dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) =>
        await _service.DeleteAsync(id) ? NoContent() : NotFound();
}
