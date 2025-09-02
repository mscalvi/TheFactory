using System.Collections.Generic;
using EscalaFacil.Components.Models;

namespace EscalaFacil.Components.Services;

public interface ISchedulerService
{
    /// <summary>
    /// Gera as atribuições respeitando disponibilidade/cargo e maximizando pessoas distintas.
    /// Ignora Assignments existentes e recomeça do zero (Bronze).
    /// </summary>
    (List<Assignment> Assignments, ScheduleSummary Summary) Generate(
        TeamModel team,
        ScheduleModel schedule,
        int maxPerPersonCap = 3,
        int? shuffleSeed = null);
}
