using ShiftRotationAPI.Data;
using ShiftRotationAPI.Models;
using ShiftRotationAPI.Repositories.Interfaces;

namespace ShiftRotationAPI.Repositories.Implementations;

public class ShiftRepository : GenericRepository<Shift>, IShiftRepository
{
    public ShiftRepository(AppDbContext context) : base(context) { }
}
