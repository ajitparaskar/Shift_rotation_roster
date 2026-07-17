using ShiftRotationAPI.DTOs;
using ShiftRotationAPI.Models;
using ShiftRotationAPI.Repositories.Interfaces;
using ShiftRotationAPI.Services.Interfaces;

namespace ShiftRotationAPI.Services.Implementations;

public class HolidayService : IHolidayService
{
    private readonly IHolidayRepository _repo;
    public HolidayService(IHolidayRepository repo) => _repo = repo;

    public async Task<List<HolidayDto>> GetAllAsync()
    {
        var items = await _repo.GetAllAsync();
        return items.Select(ToDto).ToList();
    }

    public async Task<HolidayDto> CreateAsync(HolidayCreateDto dto)
    {
        var entity = new Holiday
        {
            Name = dto.Name,
            Date = dto.Date,
            IsRecurringYearly = dto.IsRecurringYearly
        };
        await _repo.AddAsync(entity);
        return ToDto(entity);
    }

    public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);

    private static HolidayDto ToDto(Holiday h) => new()
    {
        Id = h.Id,
        Name = h.Name,
        Date = h.Date,
        IsRecurringYearly = h.IsRecurringYearly
    };
}
