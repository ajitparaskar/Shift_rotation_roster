using Microsoft.EntityFrameworkCore;
using ShiftRotationAPI.Data;
using ShiftRotationAPI.Models;
using ShiftRotationAPI.Repositories.Interfaces;

namespace ShiftRotationAPI.Repositories.Implementations;

public class LeaveRequestRepository : GenericRepository<LeaveRequest>, ILeaveRequestRepository
{
    public LeaveRequestRepository(AppDbContext context) : base(context) { }

    public override async Task<List<LeaveRequest>> GetAllAsync() =>
        await _dbSet.Include(l => l.Employee).AsNoTracking().ToListAsync();

    public async Task<List<LeaveRequest>> GetByEmployeeAsync(int employeeId) =>
        await _dbSet.Where(l => l.EmployeeId == employeeId).AsNoTracking().ToListAsync();

    public async Task<List<LeaveRequest>> GetApprovedLeaveInRangeAsync(DateOnly start, DateOnly end)
    {
        var startDt = start.ToDateTime(TimeOnly.MinValue);
        var endDt = end.ToDateTime(TimeOnly.MaxValue);
        return await _dbSet
            .Where(l => l.Status == LeaveStatus.Approved && l.StartDate <= endDt && l.EndDate >= startDt)
            .AsNoTracking().ToListAsync();
    }
}
