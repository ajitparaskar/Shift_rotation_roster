using ShiftRotationAPI.Models;

namespace ShiftRotationAPI.Repositories.Interfaces;

public interface ISchedulingPolicyRepository : IGenericRepository<SchedulingPolicy>
{
    Task<SchedulingPolicy> GetActivePolicyAsync();
}
