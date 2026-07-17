using ShiftRotationAPI.DTOs;

namespace ShiftRotationAPI.Services.Interfaces;

public interface IHolidayService
{
    Task<List<HolidayDto>> GetAllAsync();
    Task<HolidayDto> CreateAsync(HolidayCreateDto dto);
    Task<bool> DeleteAsync(int id);
}
