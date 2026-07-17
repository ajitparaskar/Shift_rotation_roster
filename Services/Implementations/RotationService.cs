using ShiftRotationAPI.DTOs;
using ShiftRotationAPI.Models;
using ShiftRotationAPI.Repositories.Interfaces;
using ShiftRotationAPI.Services.Interfaces;

namespace ShiftRotationAPI.Services.Implementations;

/// <summary>
/// The Rotation Engine — enterprise version.
/// For every day in the requested range it loads employees, shifts, leave,
/// preferences and holidays, then fairly assigns employees to shifts while
/// enforcing:
///   - No double-booking / leave conflicts / holiday skipping
///   - Minimum rest hours between shifts
///   - Shift capacity
///   - Max consecutive working days + mandatory weekly off (policy-driven)
///   - Night-shift eligibility
///   - Skill mix (at least one Expert per shift, where possible)
///   - Fair rotation (fewest shifts so far wins) with preference as tiebreaker
/// All thresholds come from the active SchedulingPolicy — nothing is hardcoded.
/// </summary>
public class RotationService : IRotationService
{
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IShiftRepository _shiftRepo;
    private readonly ILeaveRequestRepository _leaveRepo;
    private readonly IShiftPreferenceRepository _preferenceRepo;
    private readonly IShiftAssignmentRepository _assignmentRepo;
    private readonly IHolidayRepository _holidayRepo;
    private readonly ISchedulingPolicyRepository _policyRepo;

    public RotationService(
        IEmployeeRepository employeeRepo,
        IShiftRepository shiftRepo,
        ILeaveRequestRepository leaveRepo,
        IShiftPreferenceRepository preferenceRepo,
        IShiftAssignmentRepository assignmentRepo,
        IHolidayRepository holidayRepo,
        ISchedulingPolicyRepository policyRepo)
    {
        _employeeRepo = employeeRepo;
        _shiftRepo = shiftRepo;
        _leaveRepo = leaveRepo;
        _preferenceRepo = preferenceRepo;
        _assignmentRepo = assignmentRepo;
        _holidayRepo = holidayRepo;
        _policyRepo = policyRepo;
    }

    public async Task<RotationResultDto> GenerateWeeklyRotationAsync(WeeklyRotationRequestDto request)
    {
        var referenceDate = request.WeekOf ?? DateOnly.FromDateTime(DateTime.UtcNow);

        // Snap to the Monday of that week, regardless of which day of the week was passed in
        int daysSinceMonday = ((int)referenceDate.DayOfWeek + 6) % 7; // Monday=0 ... Sunday=6
        var monday = referenceDate.AddDays(-daysSinceMonday);
        var sunday = monday.AddDays(6);

        var result = await GenerateRotationAsync(new RotationRequestDto
        {
            StartDate = monday,
            EndDate = sunday,
            DepartmentId = request.DepartmentId
        });

        result.WeekStartDate = monday;
        result.WeekEndDate = sunday;
        return result;
    }

    public async Task<RotationResultDto> GenerateRotationAsync(RotationRequestDto request)
    {
        if (request.EndDate < request.StartDate)
            throw new InvalidOperationException("End date cannot be before start date.");

        var result = new RotationResultDto();
        var policy = await _policyRepo.GetActivePolicyAsync();

        // ---- Load everything up front ----
        var employees = await _employeeRepo.GetActiveEmployeesAsync(request.DepartmentId);
        var shifts = (await _shiftRepo.GetAllAsync()).OrderBy(s => s.StartTime).ToList();
        var approvedLeave = await _leaveRepo.GetApprovedLeaveInRangeAsync(request.StartDate, request.EndDate);
        var preferences = await _preferenceRepo.GetAllAsync();

        if (!employees.Any())
        {
            result.Warnings.Add("No active employees found for the given department.");
            return result;
        }
        if (!shifts.Any())
        {
            result.Warnings.Add("No shifts are configured in the system.");
            return result;
        }

        // ---- Seed consecutive-working-day streaks from recent history ----
        // Look back up to (MaxConsecutiveWorkingDays + WeeklyOffDays) days before
        // the requested start date so the 5-day / weekly-off rule carries over
        // correctly instead of resetting to zero every time you generate a new range.
        var lookbackDays = policy.MaxConsecutiveWorkingDays + policy.WeeklyOffDays;
        var historyStart = request.StartDate.AddDays(-lookbackDays);
        var historyEnd = request.StartDate.AddDays(-1);
        var priorAssignments = historyEnd >= historyStart
            ? await _assignmentRepo.GetByDateRangeAsync(historyStart, historyEnd, request.DepartmentId)
            : new List<ShiftAssignment>();

        var consecutiveDays = new Dictionary<int, int>();
        var forcedOffRemaining = new Dictionary<int, int>();

        foreach (var emp in employees)
        {
            int streak = 0;
            for (var d = historyEnd; d >= historyStart; d = d.AddDays(-1))
            {
                var workedThatDay = priorAssignments.Any(a => a.EmployeeId == emp.Id && a.Date == d);
                if (workedThatDay) streak++;
                else break;
            }
            consecutiveDays[emp.Id] = streak;
            forcedOffRemaining[emp.Id] = 0;
        }

        // fairness tracker: how many shifts each employee has been assigned in this run
        var assignmentCount = employees.ToDictionary(e => e.Id, _ => 0);
        // tracks the end datetime of an employee's most recent shift, for rest-period checks
        var lastShiftEnd = new Dictionary<int, DateTime>();

        var newAssignments = new List<ShiftAssignment>();
        int unfilledSlots = 0;

        // ---- Walk day by day ----
        for (var date = request.StartDate; date <= request.EndDate; date = date.AddDays(1))
        {
            result.TotalDaysProcessed++;

            if (await _holidayRepo.IsHolidayAsync(date))
            {
                result.Warnings.Add($"{date:yyyy-MM-dd} skipped (holiday).");
                continue;
            }

            var onLeaveToday = approvedLeave
                .Where(l => l.StartDate.Date <= date.ToDateTime(TimeOnly.MinValue) &&
                            l.EndDate.Date >= date.ToDateTime(TimeOnly.MinValue))
                .Select(l => l.EmployeeId)
                .ToHashSet();

            var assignedToday = new HashSet<int>();

            foreach (var shift in shifts)
            {
                var shiftStartToday = date.ToDateTime(TimeOnly.FromTimeSpan(shift.StartTime));
                bool isNightShift = shift.Name.Equals("Night", StringComparison.OrdinalIgnoreCase);

                var candidates = employees
                    .Where(e => !onLeaveToday.Contains(e.Id))
                    .Where(e => !assignedToday.Contains(e.Id))
                    .Where(e => forcedOffRemaining[e.Id] == 0)                                   // weekly-off rule
                    .Where(e => consecutiveDays[e.Id] < policy.MaxConsecutiveWorkingDays)          // 5-day rule
                    .Where(e => !isNightShift || e.IsNightEligible)                                // night eligibility
                    .Where(e => !lastShiftEnd.TryGetValue(e.Id, out var end) ||
                                (shiftStartToday - end).TotalHours >= policy.MinimumRestHours)      // min rest
                    .ToList();

                // Rank: fairness first (fewest total assignments), then preference as tiebreaker
                var ranked = candidates
                    .OrderBy(e => assignmentCount[e.Id])
                    .ThenBy(e => preferences
                        .Where(p => p.EmployeeId == e.Id && p.ShiftId == shift.Id &&
                                    (p.DayOfWeek == null || p.DayOfWeek == date.DayOfWeek))
                        .Select(p => (int?)p.PreferenceLevel)
                        .DefaultIfEmpty(int.MaxValue)
                        .Min())
                    .ToList();

                var chosen = ranked.Take(shift.Capacity).ToList();

                // Skill-mix rule: ensure at least one Expert on the shift if the policy requires it
                if (policy.RequireExpertPerShift && chosen.Count > 0 &&
                    !chosen.Any(e => e.ProficiencyLevel == ProficiencyLevel.Expert))
                {
                    var expertAvailable = ranked
                        .Skip(chosen.Count)
                        .FirstOrDefault(e => e.ProficiencyLevel == ProficiencyLevel.Expert);

                    if (expertAvailable != null)
                    {
                        // swap out the lowest-priority (last) chosen employee for the expert
                        chosen[chosen.Count - 1] = expertAvailable;
                    }
                    else
                    {
                        result.Warnings.Add(
                            $"{date:yyyy-MM-dd} — {shift.Name}: no Expert-level employee available for skill-mix rule.");
                    }
                }

                // Skill-mix rule: avoid pairing two Trainees on the same shift when possible.
                // Worst case (no non-trainee available) it's allowed — but flagged more
                // seriously if it happens on Night vs. Morning/Evening.
                var traineesChosen = chosen.Where(e => e.ProficiencyLevel == ProficiencyLevel.Trainee).ToList();
                if (traineesChosen.Count > 1)
                {
                    var excessTrainees = traineesChosen.Skip(1).ToList(); // keep the first (highest priority) trainee
                    foreach (var trainee in excessTrainees)
                    {
                        var nonTraineeReplacement = ranked
                            .Skip(chosen.Count)
                            .FirstOrDefault(e => e.ProficiencyLevel != ProficiencyLevel.Trainee && !chosen.Contains(e));

                        if (nonTraineeReplacement != null)
                        {
                            var idx = chosen.IndexOf(trainee);
                            chosen[idx] = nonTraineeReplacement;
                        }
                    }

                    var stillPaired = chosen.Count(e => e.ProficiencyLevel == ProficiencyLevel.Trainee) > 1;
                    if (stillPaired)
                    {
                        if (isNightShift)
                            result.Warnings.Add(
                                $"{date:yyyy-MM-dd} — {shift.Name}: multiple Trainees forced onto NIGHT shift (no non-trainee available) — recommend manager review.");
                        else
                            result.Warnings.Add(
                                $"{date:yyyy-MM-dd} — {shift.Name}: multiple Trainees paired (no non-trainee available) — acceptable for {shift.Name} shift, monitor.");
                    }
                }

                foreach (var emp in chosen)
                {
                    var assignment = new ShiftAssignment
                    {
                        EmployeeId = emp.Id,
                        ShiftId = shift.Id,
                        Date = date
                    };
                    newAssignments.Add(assignment);

                    assignedToday.Add(emp.Id);
                    assignmentCount[emp.Id]++;

                    var shiftEndToday = shiftStartToday.Add(
                        shift.EndTime >= shift.StartTime
                            ? shift.EndTime - shift.StartTime
                            : (TimeSpan.FromHours(24) - shift.StartTime + shift.EndTime)); // overnight shift
                    lastShiftEnd[emp.Id] = shiftEndToday;
                }

                if (chosen.Count < shift.Capacity)
                {
                    var shortfall = shift.Capacity - chosen.Count;
                    unfilledSlots += shortfall;
                    result.Warnings.Add(
                        $"{date:yyyy-MM-dd} — {shift.Name}: only filled {chosen.Count}/{shift.Capacity} slots.");
                }
            }

            // ---- End-of-day bookkeeping: update streaks + forced-off counters ----
            foreach (var emp in employees)
            {
                if (forcedOffRemaining[emp.Id] > 0)
                {
                    forcedOffRemaining[emp.Id]--;
                    consecutiveDays[emp.Id] = 0; // still on mandatory rest
                }
                else if (assignedToday.Contains(emp.Id))
                {
                    consecutiveDays[emp.Id]++;
                    if (consecutiveDays[emp.Id] >= policy.MaxConsecutiveWorkingDays)
                    {
                        forcedOffRemaining[emp.Id] = policy.WeeklyOffDays; // trigger mandatory weekly off
                        consecutiveDays[emp.Id] = 0;
                    }
                }
                else
                {
                    consecutiveDays[emp.Id] = 0; // voluntary/fairness-driven day off breaks the streak
                }
            }
        }

        // ---- Persist ----
        foreach (var assignment in newAssignments)
            await _assignmentRepo.AddAsync(assignment);

        result.TotalAssignmentsCreated = newAssignments.Count;
        result.TotalUnfilledSlots = unfilledSlots;
        result.Assignments = newAssignments.Select(a => new ShiftAssignmentDto
        {
            Id = a.Id,
            EmployeeId = a.EmployeeId,
            EmployeeName = employees.FirstOrDefault(e => e.Id == a.EmployeeId)?.FullName,
            ShiftId = a.ShiftId,
            ShiftName = shifts.FirstOrDefault(s => s.Id == a.ShiftId)?.Name,
            Date = a.Date,
            Status = a.Status
        }).ToList();

        return result;
    }
}
