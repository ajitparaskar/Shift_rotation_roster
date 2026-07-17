using Microsoft.EntityFrameworkCore;
using ShiftRotationAPI.Data;
using ShiftRotationAPI.Models;
using ShiftRotationAPI.Repositories.Interfaces;

namespace ShiftRotationAPI.Repositories.Implementations;

public class SchedulingPolicyRepository : GenericRepository<SchedulingPolicy>, ISchedulingPolicyRepository
{
    public SchedulingPolicyRepository(AppDbContext context) : base(context) { }

    public async Task<SchedulingPolicy> GetActivePolicyAsync()
    {
        var policy = await _dbSet.FirstOrDefaultAsync(p => p.IsActive);
        return policy ?? new SchedulingPolicy(); // safe defaults if none configured
    }
}
