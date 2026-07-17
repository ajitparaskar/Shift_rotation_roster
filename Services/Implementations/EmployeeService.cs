using ShiftRotationAPI.DTOs;
using ShiftRotationAPI.Models;
using ShiftRotationAPI.Repositories.Interfaces;
using ShiftRotationAPI.Services.Interfaces;

namespace ShiftRotationAPI.Services.Implementations;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repo;
    public EmployeeService(IEmployeeRepository repo) => _repo = repo;

    public async Task<List<EmployeeDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Select(ToDto).ToList();
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item == null ? null : ToDto(item);
    }

    public async Task<List<EmployeeDto>> GetByDepartmentAsync(int departmentId)
    {
        var items = await _repo.GetByDepartmentAsync(departmentId);
        return items.Select(ToDto).ToList();
    }

    public async Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto)
    {
        var existing = await _repo.GetByEmailAsync(dto.Email);
        if (existing != null) throw new InvalidOperationException("An employee with this email already exists.");

        var entity = new Employee
        {
            FullName = dto.FullName,
            Email = dto.Email,
            HireDate = dto.HireDate,
            DepartmentId = dto.DepartmentId,
            ProficiencyLevel = dto.ProficiencyLevel,
            IsNightEligible = dto.IsNightEligible,
            Team = dto.Team,
            IsActive = true
        };
        await _repo.AddAsync(entity);
        return ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, EmployeeCreateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return false;
        entity.FullName = dto.FullName;
        entity.Email = dto.Email;
        entity.HireDate = dto.HireDate;
        entity.DepartmentId = dto.DepartmentId;
        entity.ProficiencyLevel = dto.ProficiencyLevel;
        entity.IsNightEligible = dto.IsNightEligible;
        entity.Team = dto.Team;
        await _repo.UpdateAsync(entity);
        return true;
    }

    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);

    public async Task<bool> DeactivateAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return false;
        entity.IsActive = false;
        await _repo.UpdateAsync(entity);
        return true;
    }

    private static EmployeeDto ToDto(Employee e) => new()
    {
        Id = e.Id,
        FullName = e.FullName,
        Email = e.Email,
        HireDate = e.HireDate,
        IsActive = e.IsActive,
        ProficiencyLevel = e.ProficiencyLevel,
        IsNightEligible = e.IsNightEligible,
        Team = e.Team,
        DepartmentId = e.DepartmentId,
        DepartmentName = e.Department?.Name
    };
}
