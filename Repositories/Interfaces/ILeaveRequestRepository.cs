using ShiftRotationAPI.Models;

namespace ShiftRotationAPI.Repositories.Interfaces;

public interface ILeaveRequestRepository : IGenericRepository<LeaveRequest>
{
    Task<List<LeaveRequest>> GetByEmployeeAsync(int employeeId);
    Task<List<LeaveRequest>> GetApprovedLeaveInRangeAsync(DateOnly start, DateOnly end);
}
