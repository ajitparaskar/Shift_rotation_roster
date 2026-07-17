using ShiftRotationAPI.Models;

namespace ShiftRotationAPI.Repositories.Interfaces;

public interface IEmployeeRepository : IGenericRepository<Employee>
{
    Task<List<Employee>> GetByDepartmentAsync(int departmentId);
    Task<List<Employee>> GetActiveEmployeesAsync(int? departmentId = null);
    Task<Employee?> GetByEmailAsync(string email);
}
