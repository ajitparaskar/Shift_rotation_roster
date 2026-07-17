using ShiftRotationAPI.DTOs;

namespace ShiftRotationAPI.Services.Interfaces;

public interface IRotationService
{
    Task<RotationResultDto> GenerateRotationAsync(RotationRequestDto request);

    /// <summary>
    /// Generates a schedule for exactly one Monday–Sunday week. Pass any date
    /// that falls inside the target week — it's automatically snapped to that
    /// week's Monday and Sunday.
    /// </summary>
    Task<RotationResultDto> GenerateWeeklyRotationAsync(WeeklyRotationRequestDto request);
}
