// 1) Contexto p/ consultar o jogo dentro das condições
using FurmaIdle.Models;
using FurmaIdle.Services;

public sealed class UnlockContext
{
    public GameModel M { get; }
    public IStageService Stages { get; }
    public IUpgradeService Effects { get; }

    public UnlockContext(GameModel m, IStageService stages, IUpgradeService effects)
    {
        M = m; Stages = stages; Effects = effects;
    }

    // Helpers comuns de condições:
    public double ResourceTotal(string resId)
        => M.Resources != null && M.Resources.TryGetValue(resId, out var r) ? r.Total : 0;

    public bool StageUnlocked(string stageId)
        => M.Stages.TryGetValue(stageId, out var s) && s.Unlocked;

    public int UpgradeBuys(string upgId)
        => M.Upgrades.TryGetValue(upgId, out var u) ? u.Buys : 0;

    public bool TechUnlocked(string techId)
        => M.Technologies.TryGetValue(techId, out var t) && t.Unlocked;

    public int ActivePartyOnStage(string stageId)
        => M.Stages.TryGetValue(stageId, out var s) && s.Expedition?.PartyId != null ? s.Expedition.PartyId.Count : 0;

    // Adicione helpers que você precisar (contratos comprados, conhecimento acumulado, etc.)
}

// 2) Regra genérica
public sealed class UnlockRule
{
    public string TargetType { get; init; } = ""; // "dest", "tech", "upgrade", "stage"
    public string TargetId { get; init; } = "";
    public Func<UnlockContext, bool> When { get; init; } = _ => false;
    public Action<GameModel>? OnUnlock { get; init; } // efeitos extras opcionais (ex.: dar 1 de recurso)
}
