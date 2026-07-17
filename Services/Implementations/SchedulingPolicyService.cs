using ShiftRotationAPI.DTOs;
using ShiftRotationAPI.Models;
using ShiftRotationAPI.Repositories.Interfaces;
using ShiftRotationAPI.Services.Interfaces;

namespace ShiftRotationAPI.Services.Implementations;

public class SchedulingPolicyService : ISchedulingPolicyService
{
    private readonly ISchedulingPolicyRepository _repo;
    public SchedulingPolicyService(ISchedulingPolicyRepository repo) => _repo = repo;

    public async Task<List<SchedulingPolicyDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Select(ToDto).ToList();
    }

    public async Task<SchedulingPolicyDto> GetActiveAsync()
    {
        var policy = await _repo.GetActivePolicyAsync();
        return ToDto(policy);
    }

    public async Task<SchedulingPolicyDto> CreateAsync(SchedulingPolicyCreateDto dto)
    {
        var entity = new SchedulingPolicy
        {
            Name = dto.Name,
            MaxConsecutiveWorkingDays = dto.MaxConsecutiveWorkingDays,
            WeeklyOffDays = dto.WeeklyOffDays,
            MinimumRestHours = dto.MinimumRestHours,
            RequireExpertPerShift = dto.RequireExpertPerShift,
            IsActive = dto.IsActive
        };
        await _repo.AddAsync(entity);
        return ToDto(entity);
    }

    public async Task<bool> UpdateAsync(int id, SchedulingPolicyCreateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return false;
        entity.Name = dto.Name;
        entity.MaxConsecutiveWorkingDays = dto.MaxConsecutiveWorkingDays;
        entity.WeeklyOffDays = dto.WeeklyOffDays;
        entity.MinimumRestHours = dto.MinimumRestHours;
        entity.RequireExpertPerShift = dto.RequireExpertPerShift;
        entity.IsActive = dto.IsActive;
        await _repo.UpdateAsync(entity);
        return true;
    }

    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);

    private static SchedulingPolicyDto ToDto(SchedulingPolicy p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        MaxConsecutiveWorkingDays = p.MaxConsecutiveWorkingDays,
        WeeklyOffDays = p.WeeklyOffDays,
        MinimumRestHours = p.MinimumRestHours,
        RequireExpertPerShift = p.RequireExpertPerShift,
        IsActive = p.IsActive
    };
}
