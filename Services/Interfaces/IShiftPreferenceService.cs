using ShiftRotationAPI.DTOs;

namespace ShiftRotationAPI.Services.Interfaces;

public interface IShiftPreferenceService
{
    Task<List<ShiftPreferenceDto>> GetAllAsync();
    Task<List<ShiftPreferenceDto>> GetByEmployeeAsync(int employeeId);
    Task<ShiftPreferenceDto> CreateAsync(ShiftPreferenceCreateDto dto);
    Task<bool> DeleteAsync(int id);
}
