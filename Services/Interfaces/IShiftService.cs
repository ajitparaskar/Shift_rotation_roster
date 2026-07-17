using ShiftRotationAPI.DTOs;

namespace ShiftRotationAPI.Services.Interfaces;

public interface IShiftService
{
    Task<List<ShiftDto>> GetAllAsync();
    Task<ShiftDto?> GetByIdAsync(int id);
    Task<ShiftDto> CreateAsync(ShiftCreateDto dto);
    Task<bool> UpdateAsync(int id, ShiftCreateDto dto);
    Task<bool> DeleteAsync(int id);
}
