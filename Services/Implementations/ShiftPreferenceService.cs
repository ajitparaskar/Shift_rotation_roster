using ShiftRotationAPI.DTOs;
using ShiftRotationAPI.Models;
using ShiftRotationAPI.Repositories.Interfaces;
using ShiftRotationAPI.Services.Interfaces;

namespace ShiftRotationAPI.Services.Implementations;

public class ShiftPreferenceService : IShiftPreferenceService
{
    private readonly IShiftPreferenceRepository _repo;
    public ShiftPreferenceService(IShiftPreferenceRepository repo) => _repo = repo;

    public async Task<List<ShiftPreferenceDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Select(ToDto).ToList();
    }

    public async Task<List<ShiftPreferenceDto>> GetByEmployeeAsync(int employeeId)
    {
        var items = await _repo.GetByEmployeeAsync(employeeId);
        return items.Select(ToDto).ToList();
    }

    public async Task<ShiftPreferenceDto> CreateAsync(ShiftPreferenceCreateDto dto)
    {
        var entity = new ShiftPreference
        {
            EmployeeId = dto.EmployeeId,
            ShiftId = dto.ShiftId,
            PreferenceLevel = dto.PreferenceLevel,
            DayOfWeek = dto.DayOfWeek
        };
        await _repo.AddAsync(entity);
        return ToDto(entity);
    }

    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);

    private static ShiftPreferenceDto ToDto(ShiftPreference p) => new()
    {
        Id = p.Id,
        EmployeeId = p.EmployeeId,
        ShiftId = p.ShiftId,
        PreferenceLevel = p.PreferenceLevel,
        DayOfWeek = p.DayOfWeek
    };
}
