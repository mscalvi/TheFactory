using FurmaIdle.Models;

namespace FurmaIdle.Services
{
    public interface IUnlockService
    {
        // Retorna uma lista de (tipo,id) desbloqueados nesta passada — útil para log
        List<(string type, string id)> Recompute(GameModel m);
    }

    public sealed class UnlockService : IUnlockService
    {
        private readonly IStageService _stages;
        private readonly IUpgradeService _effects;

        public UnlockService(IStageService stages, IUpgradeService effects)
        {
            _stages = stages; _effects = effects;
        }

        public List<(string type, string id)> Recompute(GameModel m)
        {
            var ctx = new UnlockContext(m, _stages, _effects);
            var newly = new List<(string, string)>();

            foreach (var rule in UnlockData.Rules)
            {
                if (!rule.When(ctx)) continue;

                switch (rule.TargetType)
                {
                    case "dest":
                        if (m.Destinations.TryGetValue(rule.TargetId, out var d) && !d.Unlocked)
                        {
                            d.Unlocked = true;
                            d.Avaliable = true;
                            newly.Add(("dest", rule.TargetId));
                        }
                        break;

                    case "tech":
                        if (m.Technologies.TryGetValue(rule.TargetId, out var t) && !t.Unlocked)
                        {
                            t.Unlocked = true;
                            t.Avaliable = true;
                            newly.Add(("tech", rule.TargetId));
                        }
                        break;

                    case "upgrade":
                        if (m.Upgrades.TryGetValue(rule.TargetId, out var u) && !u.Unlocked)
                        {
                            u.Unlocked = true;
                            // disponibilidade deriva de Unlocked && !IsMaxed
                            u.Avaliable = !u.IsMaxed;
                            newly.Add(("upgrade", rule.TargetId));
                        }
                        break;

                    case "stage":
                        if (m.Stages.TryGetValue(rule.TargetId, out var s) && !s.Unlocked)
                        {
                            s.Unlocked = true;
                            newly.Add(("stage", rule.TargetId));
                        }
                        break;
                }

                rule.OnUnlock?.Invoke(m);
            }

            return newly;
        }
    }

}
