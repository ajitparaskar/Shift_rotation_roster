using Microsoft.EntityFrameworkCore;
using ShiftRotationAPI.Data;
using ShiftRotationAPI.Models;
using ShiftRotationAPI.Repositories.Interfaces;

namespace ShiftRotationAPI.Repositories.Implementations;

public class HolidayRepository : GenericRepository<Holiday>, IHolidayRepository
{
    public HolidayRepository(AppDbContext context) : base(context) { }

    public async Task<List<Holiday>> GetInRangeAsync(DateOnly start, DateOnly end) =>
        await _dbSet.Where(h => h.Date >= start && h.Date <= end).AsNoTracking().ToListAsync();

    public async Task<bool> IsHolidayAsync(DateOnly date) =>
        await _dbSet.AnyAsync(h => h.Date == date ||
            (h.IsRecurringYearly && h.Date.Month == date.Month && h.Date.Day == date.Day));
}
