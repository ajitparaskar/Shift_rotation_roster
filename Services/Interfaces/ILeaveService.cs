using ShiftRotationAPI.DTOs;

namespace ShiftRotationAPI.Services.Interfaces;

public interface ILeaveService
{
    Task<List<LeaveRequestDto>> GetAllAsync();
    Task<LeaveRequestDto?> GetByIdAsync(int id);
    Task<List<LeaveRequestDto>> GetByEmployeeAsync(int employeeId);
    Task<LeaveRequestDto> CreateAsync(LeaveRequestCreateDto dto);
    Task<bool> UpdateStatusAsync(int id, LeaveStatusUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
