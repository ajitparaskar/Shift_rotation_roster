using Microsoft.EntityFrameworkCore;
using ShiftRotationAPI.Data;
using ShiftRotationAPI.Models;
using ShiftRotationAPI.Repositories.Interfaces;

namespace ShiftRotationAPI.Repositories.Implementations;

public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext context) : base(context) { }

    public override async Task<List<Employee>> GetAllAsync() =>
        await _dbSet.Include(e => e.Department).AsNoTracking().ToListAsync();

    public async Task<List<Employee>> GetByDepartmentAsync(int departmentId) =>
        await _dbSet.Where(e => e.DepartmentId == departmentId).AsNoTracking().ToListAsync();

    public async Task<List<Employee>> GetActiveEmployeesAsync(int? departmentId = null)
    {
        var query = _dbSet.Where(e => e.IsActive);
        if (departmentId.HasValue) query = query.Where(e => e.DepartmentId == departmentId.Value);
        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<Employee?> GetByEmailAsync(string email) =>
        await _dbSet.FirstOrDefaultAsync(e => e.Email == email);
}
