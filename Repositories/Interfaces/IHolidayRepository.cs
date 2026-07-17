using ShiftRotationAPI.Models;

namespace ShiftRotationAPI.Repositories.Interfaces;

public interface IHolidayRepository : IGenericRepository<Holiday>
{
    Task<List<Holiday>> GetInRangeAsync(DateOnly start, DateOnly end);
    Task<bool> IsHolidayAsync(DateOnly date);
}
