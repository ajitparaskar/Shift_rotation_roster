using ShiftRotationAPI.DTOs;

namespace ShiftRotationAPI.Services.Interfaces;

public interface IEmployeeService
{
    Task<List<EmployeeDto>> GetAllAsync();
    Task<EmployeeDto?> GetByIdAsync(int id);
    Task<List<EmployeeDto>> GetByDepartmentAsync(int departmentId);
    Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto);
    Task<bool> UpdateAsync(int id, EmployeeCreateDto dto);
    Task<bool> DeleteAsync(int id);
    Task<bool> DeactivateAsync(int id);
}
