using ShiftRotationAPI.DTOs;
using ShiftRotationAPI.Models;
using ShiftRotationAPI.Repositories.Interfaces;
using ShiftRotationAPI.Services.Interfaces;

namespace ShiftRotationAPI.Services.Implementations;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repo;
    public DepartmentService(IDepartmentRepository repo) => _repo = repo;

    public async Task<List<DepartmentDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Select(ToDto).ToList();
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item == null ? null : ToDto(item);
    }

    public async Task<DepartmentDto> CreateAsync(DepartmentCreateDto dto)
    {
        var entity = new Department { Name = dto.Name, Description = dto.Description };
        await _repo.AddAsync(entity);
        return ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, DepartmentCreateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return false;
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        await _repo.UpdateAsync(entity);
        return true;
    }

    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);

    private static DepartmentDto ToDto(Department d) => new()
    {
        Id = d.Id,
        Name = d.Name,
        Description = d.Description
    };
}
