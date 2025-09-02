using System.Threading.Tasks;
using EscalaFacil.Components.Models;

namespace EscalaFacil.Components.Services;

public interface IScheduleLocator
{
    /// Retorna (Team, Schedule) pelo scheduleId, ou null se não achar.
    Task<(TeamModel Team, ScheduleModel Schedule)?> FindByScheduleIdAsync(string scheduleId);
}
