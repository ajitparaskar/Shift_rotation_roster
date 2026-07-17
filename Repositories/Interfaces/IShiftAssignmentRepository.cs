using ShiftRotationAPI.Models;

namespace ShiftRotationAPI.Repositories.Interfaces;

public interface IShiftAssignmentRepository : IGenericRepository<ShiftAssignment>
{
    Task<List<ShiftAssignment>> GetByDateRangeAsync(DateOnly start, DateOnly end, int? departmentId = null);
    Task<List<ShiftAssignment>> GetByEmployeeAsync(int employeeId, DateOnly? start = null, DateOnly? end = null);
    Task<List<ShiftAssignment>> GetByDateAsync(DateOnly date);
    Task<bool> IsEmployeeAssignedOnDateAsync(int employeeId, DateOnly date);
}
