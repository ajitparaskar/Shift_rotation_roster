using Microsoft.EntityFrameworkCore;
using ShiftRotationAPI.Data;
using ShiftRotationAPI.Models;
using ShiftRotationAPI.Repositories.Interfaces;

namespace ShiftRotationAPI.Repositories.Implementations;

public class ShiftPreferenceRepository : GenericRepository<ShiftPreference>, IShiftPreferenceRepository
{
    public ShiftPreferenceRepository(AppDbContext context) : base(context) { }

    public async Task<List<ShiftPreference>> GetByEmployeeAsync(int employeeId) =>
        await _dbSet.Where(p => p.EmployeeId == employeeId).AsNoTracking().ToListAsync();
}
