using System.Linq;
using System.Threading.Tasks;
using EscalaFacil.Components.Models;

namespace EscalaFacil.Components.Services;

public sealed class ScheduleLocator : IScheduleLocator
{
    private readonly IDataStore _data;

    public ScheduleLocator(IDataStore data) => _data = data;

    public async Task<(TeamModel Team, ScheduleModel Schedule)?> FindByScheduleIdAsync(string scheduleId)
    {
        // Carrega equipes top-level
        var teamKeys = _data.EnumerateKeys("teams/")
            .Where(k => k.Count(c => c == '/') == 1)
            .ToList();

        foreach (var tkey in teamKeys)
        {
            var team = await _data.LoadAsync<TeamModel>(tkey);
            if (team is null) continue;

            var skey = DataKeys.Schedule(team.Id, scheduleId);
            if (await _data.ExistsAsync(skey))
            {
                var sched = await _data.LoadAsync<ScheduleModel>(skey);
                if (sched != null) return (team, sched);
            }
        }
        return null;
    }
}
