using Microsoft.AspNetCore.Mvc;
using ShiftRotationAPI.DTOs;
using ShiftRotationAPI.Services.Interfaces;

namespace ShiftRotationAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SchedulingPolicyController : ControllerBase
{
    private readonly ISchedulingPolicyService _service;
    public SchedulingPolicyController(ISchedulingPolicyService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<SchedulingPolicyDto>>> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("active")]
    public async Task<ActionResult<SchedulingPolicyDto>> GetActive() => Ok(await _service.GetActiveAsync());

    [HttpPost]
    public async Task<ActionResult<SchedulingPolicyDto>> Create(SchedulingPolicyCreateDto dto) =>
        Ok(await _service.CreateAsync(dto));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, SchedulingPolicyCreateDto dto) =>
        await _service.UpdateAsync(id, dto) ? NoContent() : NotFound();

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) =>
        await _service.DeleteAsync(id) ? NoContent() : NotFound();
}
