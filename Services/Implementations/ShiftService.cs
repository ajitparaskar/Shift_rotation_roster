using ShiftRotationAPI.DTOs;
using ShiftRotationAPI.Models;
using ShiftRotationAPI.Repositories.Interfaces;
using ShiftRotationAPI.Services.Interfaces;

namespace ShiftRotationAPI.Services.Implementations;

public class ShiftService : IShiftService
{
    private readonly IShiftRepository _repo;
    public ShiftService(IShiftRepository repo) => _repo = repo;

    public async Task<List<ShiftDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Select(ToDto).ToList();
    }

    public async Task<ShiftDto?> GetByIdAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item == null ? null : ToDto(item);
    }

    public async Task<ShiftDto> CreateAsync(ShiftCreateDto dto)
    {
        var entity = new Shift
        {
            Name = dto.Name,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            Capacity = dto.Capacity
        };
        await _repo.AddAsync(entity);
        return ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, ShiftCreateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return false;
        entity.Name = dto.Name;
        entity.StartTime = dto.StartTime;
        entity.EndTime = dto.EndTime;
        entity.Capacity = dto.Capacity;
        await _repo.UpdateAsync(entity);
        return true;
    }

    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);

    private static ShiftDto ToDto(Shift s) => new()
    {
        Id = s.Id,
        Name = s.Name,
        StartTime = s.StartTime,
        EndTime = s.EndTime,
        Capacity = s.Capacity
    };
}
