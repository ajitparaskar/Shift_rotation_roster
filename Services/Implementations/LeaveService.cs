using ShiftRotationAPI.DTOs;
using ShiftRotationAPI.Models;
using ShiftRotationAPI.Repositories.Interfaces;
using ShiftRotationAPI.Services.Interfaces;

namespace ShiftRotationAPI.Services.Implementations;

public class LeaveService : ILeaveService
{
    private readonly ILeaveRequestRepository _repo;
    public LeaveService(ILeaveRequestRepository repo) => _repo = repo;

    public async Task<List<LeaveRequestDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Select(ToDto).ToList();
    }

    public async Task<LeaveRequestDto?> GetByIdAsync(int id)
    {
        var item = await _repo.GetByIdAsync(id);
        return item == null ? null : ToDto(item);
    }

    public async Task<List<LeaveRequestDto>> GetByEmployeeAsync(int employeeId)
    {
        var items = await _repo.GetByEmployeeAsync(employeeId);
        return items.Select(ToDto).ToList();
    }

    public async Task<LeaveRequestDto> CreateAsync(LeaveRequestCreateDto dto)
    {
        if (dto.EndDate < dto.StartDate)
            throw new InvalidOperationException("End date cannot be before start date.");

        var entity = new LeaveRequest
        {
            EmployeeId = dto.EmployeeId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Reason = dto.Reason,
            Status = LeaveStatus.Pending
        };
        await _repo.AddAsync(entity);
        return ToDto(entity);
    }

    public async Task<bool> UpdateStatusAsync(int id, LeaveStatusUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null) return false;
        entity.Status = dto.Status;
        await _repo.UpdateAsync(entity);
        return true;
    }

    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);

    private static LeaveRequestDto ToDto(LeaveRequest l) => new()
    {
        Id = l.Id,
        EmployeeId = l.EmployeeId,
        EmployeeName = l.Employee?.FullName,
        StartDate = l.StartDate,
        EndDate = l.EndDate,
        Reason = l.Reason,
        Status = l.Status
    };
}
