using ShiftRotationAPI.DTOs;

namespace ShiftRotationAPI.Services.Interfaces;

public interface ISchedulingPolicyService
{
    Task<List<SchedulingPolicyDto>> GetAllAsync();
    Task<SchedulingPolicyDto> GetActiveAsync();
    Task<SchedulingPolicyDto> CreateAsync(SchedulingPolicyCreateDto dto);
    Task<bool> UpdateAsync(int id, SchedulingPolicyCreateDto dto);
    Task<bool> DeleteAsync(int id);
}
