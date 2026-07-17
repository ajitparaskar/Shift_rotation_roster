using Microsoft.EntityFrameworkCore;
using ShiftRotationAPI.Data;
using ShiftRotationAPI.Models;
using ShiftRotationAPI.Repositories.Interfaces;

namespace ShiftRotationAPI.Repositories.Implementations;

public class ShiftAssignmentRepository : GenericRepository<ShiftAssignment>, IShiftAssignmentRepository
{
    public ShiftAssignmentRepository(AppDbContext context) : base(context) { }

    public override async Task<List<ShiftAssignment>> GetAllAsync() =>
        await _dbSet.Include(a => a.Employee).Include(a => a.Shift).AsNoTracking().ToListAsync();

    public async Task<List<ShiftAssignment>> GetByDateRangeAsync(DateOnly start, DateOnly end, int? departmentId = null)
    {
        var query = _dbSet.Include(a => a.Employee).Include(a => a.Shift)
            .Where(a => a.Date >= start && a.Date <= end);
        if (departmentId.HasValue)
            query = query.Where(a => a.Employee != null && a.Employee.DepartmentId == departmentId.Value);
        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<List<ShiftAssignment>> GetByEmployeeAsync(int employeeId, DateOnly? start = null, DateOnly? end = null)
    {
        var query = _dbSet.Include(a => a.Shift).Where(a => a.EmployeeId == employeeId);
        if (start.HasValue) query = query.Where(a => a.Date >= start.Value);
        if (end.HasValue) query = query.Where(a => a.Date <= end.Value);
        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<List<ShiftAssignment>> GetByDateAsync(DateOnly date) =>
        await _dbSet.Include(a => a.Employee).Include(a => a.Shift)
            .Where(a => a.Date == date).AsNoTracking().ToListAsync();

    public async Task<bool> IsEmployeeAssignedOnDateAsync(int employeeId, DateOnly date) =>
        await _dbSet.AnyAsync(a => a.EmployeeId == employeeId && a.Date == date);
}
