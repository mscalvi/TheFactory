using System.Linq;
using FurmaIdle.Data;
using FurmaIdle.Models;

namespace FurmaIdle.Services
{
    public interface IStageService
    {
        bool CanSelect(GameModel g, string stageId, out string? reason);
        StageModel? Get(GameModel g, string stageId);
        string GetFirstUnlocked(GameModel g);
        int GetEffectivePartyCap(GameModel g, string stageId);
    }

    public sealed class StageService : IStageService
    {
        public bool CanSelect(GameModel g, string stageId, out string? reason)
        {
            reason = null;
            if (string.IsNullOrWhiteSpace(stageId)) { reason = "Stage inválido."; return false; }
            if (!g.Stages.TryGetValue(stageId, out var st)) { reason = "Stage inexistente."; return false; }
            if (!st.Unlocked) { reason = "Stage bloqueado."; return false; }
            return true;
        }

        public StageModel? Get(GameModel g, string stageId)
            => g.Stages.TryGetValue(stageId, out var st) ? st : null;

        public string GetFirstUnlocked(GameModel g)
        {
            foreach (var id in StageData.Order)
                if (g.Stages.TryGetValue(id, out var st) && st.Unlocked) return id;

            return g.Stages.Keys.FirstOrDefault() ?? "s00";
        }

        public int GetEffectivePartyCap(GameModel g, string stageId)
        {
            var capGuild = g.Guild?.PartyCapMax ?? 0;
            var capStage = g.Stages.TryGetValue(stageId, out var st) ? st.PartyCap : 0;
            return capGuild < capStage ? capGuild : capStage;
        }
    }
}
