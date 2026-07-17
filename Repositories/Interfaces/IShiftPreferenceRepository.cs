using ShiftRotationAPI.Models;

namespace ShiftRotationAPI.Repositories.Interfaces;

public interface IShiftPreferenceRepository : IGenericRepository<ShiftPreference>
{
    Task<List<ShiftPreference>> GetByEmployeeAsync(int employeeId);
}
