using Microsoft.AspNetCore.Mvc;
using ShiftRotationAPI.DTOs;
using ShiftRotationAPI.Services.Interfaces;

namespace ShiftRotationAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HolidayController : ControllerBase
{
    private readonly IHolidayService _service;
    public HolidayController(IHolidayService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<HolidayDto>>> GetAll() => Ok(await _service.GetAllAsync());

    [HttpPost]
    public async Task<ActionResult<HolidayDto>> Create(HolidayCreateDto dto) =>
        Ok(await _service.CreateAsync(dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) =>
        await _service.DeleteAsync(id) ? NoContent() : NotFound();
}
