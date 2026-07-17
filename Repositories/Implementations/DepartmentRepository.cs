using ShiftRotationAPI.Data;
using ShiftRotationAPI.Models;
using ShiftRotationAPI.Repositories.Interfaces;

namespace ShiftRotationAPI.Repositories.Implementations;

public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(AppDbContext context) : base(context) { }
}
