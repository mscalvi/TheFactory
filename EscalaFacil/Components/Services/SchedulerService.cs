using System;
using System.Collections.Generic;
using System.Linq;
using EscalaFacil.Components.Models;

namespace EscalaFacil.Components.Services;

public sealed class SchedulerService : ISchedulerService
{
    public (List<Assignment> Assignments, ScheduleSummary Summary) Generate(
        TeamModel team,
        ScheduleModel schedule,
        int maxPerPersonCap = 3,
        int? shuffleSeed = null)
    {
        var rng = shuffleSeed.HasValue ? new Random(shuffleSeed.Value) : new Random();

        var members = team.Members.Where(m => m.IsActive).ToDictionary(m => m.Id, m => m);
        var shiftsById = schedule.Shifts.ToDictionary(s => s.Id, s => s);

        // Mapa de candidatos por turno (filtra por disponibilidade e compatibilidade de cargo)
        var candidatesByShift = schedule.Shifts.ToDictionary(
            sh => sh.Id,
            sh =>
                schedule.Availability
                    .Where(a => a.ShiftId == sh.Id)
                    .Select(a => a.MemberId)
                    .Where(members.ContainsKey)
                    .Where(mid => CanDoRole(members[mid], sh))
                    .Distinct()
                    .ToList()
        );

        // Ordena por "escassez" (turnos com menos candidatos primeiro)
        var orderedShifts = schedule.Shifts
            .OrderBy(sh => candidatesByShift[sh.Id].Count)
            .ThenBy(sh => sh.Start)
            .ToList();

        var result = new List<Assignment>();
        var countByMember = new Dictionary<string, int>();
        var unfilled = new HashSet<string>(orderedShifts.Select(s => s.Id));

        for (int cap = 1; cap <= maxPerPersonCap && unfilled.Count > 0; cap++)
        {
            foreach (var sh in orderedShifts.Where(s => unfilled.Contains(s.Id)))
            {
                var cands = candidatesByShift[sh.Id]
                    .Where(mid => countByMember.GetValueOrDefault(mid) < cap)
                    .Where(mid => !Conflicts(result, mid, sh, shiftsById))
                    .OrderBy(mid => countByMember.GetValueOrDefault(mid))
                    .ThenBy(_ => rng.Next())
                    .ToList();

                if (cands.Count == 0) continue;

                var chosen = cands[0];
                result.Add(new Assignment { ShiftId = sh.Id, MemberId = chosen });
                countByMember[chosen] = countByMember.GetValueOrDefault(chosen) + 1;
                unfilled.Remove(sh.Id);
            }
        }

        var summary = new ScheduleSummary
        {
            TotalShifts = schedule.Shifts.Count,
            FilledShifts = result.Count,
            DistinctPeople = result.Select(a => a.MemberId).Distinct().Count(),
            CountByMember = result
                .GroupBy(a => a.MemberId)
                .ToDictionary(g => g.Key, g => g.Count())
        };

        return (result, summary);
    }

    private static bool CanDoRole(MemberModel m, ShiftModel sh)
        => m.RoleIds.Count == 0 || string.IsNullOrWhiteSpace(sh.RoleId) || m.RoleIds.Contains(sh.RoleId);

    private static bool Conflicts(
        List<Assignment> current,
        string memberId,
        ShiftModel target,
        Dictionary<string, ShiftModel> shiftsById)
    {
        foreach (var a in current)
        {
            if (a.MemberId != memberId) continue;

            var other = shiftsById[a.ShiftId];
            if (Overlaps(target.Start, target.End, other.Start, other.End))
                return true;
        }
        return false;
    }

    private static bool Overlaps(DateTime aStart, DateTime aEnd, DateTime bStart, DateTime bEnd)
        => aStart < bEnd && bStart < aEnd; // permite "colado" (fim == início) sem conflito
}
