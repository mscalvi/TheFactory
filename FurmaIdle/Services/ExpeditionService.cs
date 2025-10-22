using FurmaIdle.Data;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Xml.Linq;

namespace FurmaIdle.Services
{
    public interface IExpeditionService
    {
        List<CharacterModel> GetActiveCharacters(ExpeditionModel expedition);

        StageModel? GetCurrentStage();
        ExpeditionModel GetOrCreateCurrentExpedition();

        // novo: consulta e seleção
        IEnumerable<CharacterModel> GetCharactersInBase();   // só “na base”
        IReadOnlyCollection<string> GetPartyIds();           // ids selecionados
        bool IsExpeditionActive();                           // está ativa?
        bool CharSelected(string charId);
        int GetPartyCap();                                  // PartySizeMax ou fallback
        bool CanToggleChar(string charId);
        bool ToggleChar(string charId, out string? reason);  // seleciona/desseleciona respeitando cap

        Task LaunchExpedition(IReadOnlyCollection<string> roster);
        void EndExpedition(GameModel g, string stageId);
        Task ReapplyAfterResetAsync(string stageId);
    }

    public sealed class ExpeditionService : IExpeditionService
    {
        private readonly ILocateService _locate;
        private readonly ICurrentGameService _game;
        private readonly IUnlockService _unlock;
        private readonly IEffectService _effect;

        public ExpeditionService(ILocateService locate, ICurrentGameService game, IUnlockService unlock, IEffectService effect)
        {
            _locate = locate;
            _game = game;
            _unlock = unlock;
            _effect = effect;
        }

        public List<CharacterModel> GetActiveCharacters(ExpeditionModel expedition)
        {
            var result = new List<CharacterModel>();
            if (expedition?.PartyIds == null) return result;

            foreach (var id in expedition.PartyIds)
            {
                if (string.IsNullOrWhiteSpace(id)) continue;
                var c = _locate.LocateCharacter(_game.CurrentGame, id);
                if (c != null) result.Add(c);
            }
            return result;
        }

        // ===== Helpers de Stage/Expedição =====
        public StageModel? GetCurrentStage()
        {
            var stId = _game.CurrentGame?.SelectedStageId;
            if (string.IsNullOrWhiteSpace(stId)) return null;

            if (_game.CurrentGame?.Stages != null &&
                _game.CurrentGame.Stages.TryGetValue(stId, out var st))
                return st;

            return null;
        }

        public ExpeditionModel GetOrCreateCurrentExpedition()
        {
            var st = GetCurrentStage() ?? throw new InvalidOperationException("Nenhum Stage selecionado.");
            // garante não-nulo
            return st.ActiveExpedition ??= new ExpeditionModel
            {
                StageId = st.Id,
                ExpeditionState = UnlockHelper.ExpeditionState.Idle,
                PartyIds = new List<string>()
            };
        }

        public bool IsExpeditionActive()
        {
            var st = GetCurrentStage();
            var ex = st?.ActiveExpedition;
            return ex is not null && ex.ExpeditionState == UnlockHelper.ExpeditionState.Active;
        }

        // ===== Consulta de personagens =====
        public IEnumerable<CharacterModel> GetCharactersInBase()
        {
            var g = _game.CurrentGame;
            if (g?.Characters is null) yield break;

            foreach (var c in g.Characters.Values)
            {
                if (c is null) continue;
                if (c.State != UnlockHelper.State.Unlocked) continue;
                if (c.CharState == UnlockHelper.CharState.InBase) yield return c;
            }
        }

        // ===== Seleção (Party) =====
        public IReadOnlyCollection<string> GetPartyIds()
        {
            var ex = GetOrCreateCurrentExpedition();
            return ex.PartyIds ??= new List<string>();
        }
        public bool CharSelected(string charId)
        {
            var ids = GetPartyIds();
            return ids.Contains(charId);
        }
        public int GetPartyCap()
        {
            var st = GetCurrentStage();
            return (st?.PartySizeActual > 0) ? st!.PartySizeActual : 3;
        }
        public bool CanToggleChar(string charId)
        {
            if (IsExpeditionActive()) return false;
            var ex = GetOrCreateCurrentExpedition();

            // Remover é sempre permitido
            if (ex.PartyIds!.Contains(charId)) return true;

            // Adicionar respeitando cap
            return ex.PartyIds!.Count < GetPartyCap();
        }
        public bool ToggleChar(string charId, out string? reason)
        {
            reason = null;

            if (string.IsNullOrWhiteSpace(charId)) { reason = "Id inválido."; return false; }
            if (IsExpeditionActive()) { reason = "Expedição já está ativa."; return false; }

            var ex = GetOrCreateCurrentExpedition();
            ex.PartyIds ??= new List<string>();

            // Se já está, desseleciona
            if (ex.PartyIds.Remove(charId))
                return true;

            // Se não está, respeita o cap
            if (ex.PartyIds.Count >= GetPartyCap())
            {
                reason = $"Limite de equipe atingido ({GetPartyCap()}).";
                return false;
            }

            ex.PartyIds.Add(charId);
            return true;
        }

        // Start e End
        public async Task LaunchExpedition(IReadOnlyCollection<string> roster)
        {
            await _game.Mutate(g =>
            {
                var st = GetCurrentStage() ?? throw new InvalidOperationException("Nenhum Stage selecionado.");
                var ex = GetOrCreateCurrentExpedition();

                if (ex.ExpeditionState == UnlockHelper.ExpeditionState.Active)
                    return; // já ativa — nada a fazer

                ex.PartyIds ??= new List<string>();

                // Normaliza roster: remove vazios, distinct e respeita o cap
                var cap = GetPartyCap();
                var ids = (roster ?? Array.Empty<string>())
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .Distinct(StringComparer.Ordinal)
                    .Take(cap)
                    .ToList();

                // Mantém apenas personagens válidos e "na base"
                var finalIds = new List<string>(ids.Count);
                foreach (var id in ids)
                {
                    var c = _locate.LocateCharacter(_game.CurrentGame, id);
                    if (c is null) continue;
                    if (c.State != UnlockHelper.State.Unlocked) continue;
                    if (c.CharState != UnlockHelper.CharState.InBase) continue;

                    // Marca no personagem
                    c.CharState = UnlockHelper.CharState.OnStage;
                    c.InStageId = st.Id;

                    finalIds.Add(id);
                }

                // Atualiza expedição
                ex.PartyIds.Clear();
                ex.PartyIds.AddRange(finalIds);
                ex.StageId = st.Id;
                ex.ExpeditionState = UnlockHelper.ExpeditionState.Active;

                ex.StartedAt = DateTimeOffset.UtcNow;
                ex.FinishedAt = null;

            }, save: true); // <-- persiste ao final
        }
        public void EndExpedition(GameModel g, string stageId)
        {
            // 1) Limpa temporários da expedição
            g.ExpeditionStats = new StatsModel();
            var stage = _locate.LocateStage(g, stageId);
            //stage.ActiveExpedition?.Specialties?.Clear();      

            // 2) Reverte itens com Persistence == UntilExpedition
            foreach (var up in g.Upgrades.Values)
                if (up.Persistence == UnlockHelper.Persistence.untilExpedition)
                {
                    up.State = UnlockHelper.State.Blocked;
                    up.ActualBuy = 0;
                }

            foreach (var ch in g.Characters.Values)
                if (ch.Persistence == UnlockHelper.Persistence.untilExpedition)
                {
                    ch.State = UnlockHelper.State.Blocked;
                    ch.ContractCap = CharacterData.GetDef(ch.Id).ContractCap;
                }

            foreach (var ct in g.Contracts.Values)
                if (ct.Persistence == UnlockHelper.Persistence.untilExpedition)
                {
                    ct.State = UnlockHelper.State.Blocked;
                    ct.AddMod = 0; ct.MultMod = 1; ct.PriceFactor = 1;
                }

            foreach (var r in g.Resources.Values)
                if (r.Persistence == UnlockHelper.Persistence.untilExpedition)
                {
                    r.State = UnlockHelper.State.Blocked;
                    r.AddMod = 0; r.MultMod = 1;
                }

            foreach (var k in g.Knowledges.Values)
                if (k.Persistence == UnlockHelper.Persistence.untilExpedition)
                {
                    k.State = UnlockHelper.State.Blocked;
                    k.AddMod = 0; k.MultMod = 1;
                }

            foreach (var c in g.Coins.Values)
                if (c.Persistence == UnlockHelper.Persistence.untilExpedition)
                {
                    c.State = UnlockHelper.State.Blocked;
                    c.AddMod = 0; c.MultMod = 1;
                }

            // Stage/party derivados
            var st = _locate.LocateStage(g, stageId);
            st.PartySizeActual = StageData.GetDef(st.Id).PartySizeStart;
            var ex = st.ActiveExpedition;
            if (ex is not null)
            {
                // devolve personagens para a base
                if (ex.PartyIds != null)
                {
                    foreach (var id in ex.PartyIds)
                    {
                        if (string.IsNullOrWhiteSpace(id)) continue;
                        var c = _locate.LocateCharacter(g, id);
                        if (c is not null)
                        {
                            c.CharState = UnlockHelper.CharState.InBase;
                            c.InStageId = null;
                        }
                    }
                    ex.PartyIds.Clear();
                }

                ex.FinishedAt = DateTimeOffset.UtcNow;
                ex.ExpeditionState = UnlockHelper.ExpeditionState.Idle;
            }
        }

        public async Task ReapplyAfterResetAsync(string stageId)
        {
            var g = _game.CurrentGame;

            // 2.3.a) Reaplicar UNLOCKS via UnlockService (cada método já persiste)
            foreach (var up in g.Upgrades.Values)
            {
                if (!IsPermanent(up.Persistence)) continue;
                if (!(up.State == UnlockHelper.State.Unlocked || up.ActualBuy > 0)) continue;

                switch (up.EffectType)
                {
                    case EffectHelper.EffectType.CharacterUnlock: await _unlock.UnlockCharacter(up.TargetId); break;
                    case EffectHelper.EffectType.ContractUnlock: await _unlock.UnlockContract(up.TargetId); break;
                    case EffectHelper.EffectType.ExpansionUnlock: await _unlock.UnlockExpansion(up.TargetId); break;
                    case EffectHelper.EffectType.KnowledgeUnlock: await _unlock.UnlockKnowledge(up.TargetId); break;
                    case EffectHelper.EffectType.LocalUnlock: await _unlock.UnlockLocal(up.TargetId); break;
                    case EffectHelper.EffectType.StageUnlock: await _unlock.UnlockStage(up.TargetId); break;
                    case EffectHelper.EffectType.TechUnlock: await _unlock.UnlockTech(up.TargetId); break;
                }
            }

            // 2.3.b) Reaplicar efeitos permanentes NÃO-unlock em um único Mutate
            await _game.Mutate(gg =>
            {
                foreach (var up in gg.Upgrades.Values)
                {
                    if (!IsPermanent(up.Persistence)) continue;
                    if (!(up.State == UnlockHelper.State.Unlocked || up.ActualBuy > 0)) continue;

                    switch (up.EffectType)
                    {
                        case EffectHelper.EffectType.ContractLevelUnlock:
                            {
                                var st = _locate.LocateStage(gg, stageId);
                                st.PartySizeActual = Math.Max(st.PartySizeActual, StageData.GetDef(st.Id).PartySizeStart);
                                st.ActualContractLevel += (int)up.EffectValue;
                                break;
                            }

                        case EffectHelper.EffectType.ContractCapUnlock:
                            {
                                if (up.TargetId == "aCharacters")
                                {
                                    foreach (var kv in gg.Characters)
                                    {
                                        if (up.EffectOp == EffectHelper.EffectOperation.Additive)
                                            kv.Value.ContractCap += (int)up.EffectValue;
                                        else if (up.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                                            kv.Value.ContractCap *= (int)up.EffectValue;
                                        else if (up.EffectOp == EffectHelper.EffectOperation.Override)
                                            kv.Value.ContractCap = (int)up.EffectValue;
                                    }
                                }
                                else
                                {
                                    var ch = _locate.LocateCharacter(gg, up.TargetId);
                                    if (up.EffectOp == EffectHelper.EffectOperation.Additive)
                                        ch.ContractCap += (int)up.EffectValue;
                                    else if (up.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                                        ch.ContractCap *= (int)up.EffectValue;
                                    else if (up.EffectOp == EffectHelper.EffectOperation.Override)
                                        ch.ContractCap = (int)up.EffectValue;
                                }
                                break;
                            }

                        case EffectHelper.EffectType.PartySize:
                            {
                                var st = _locate.LocateStage(gg, stageId);
                                st.PartySizeActual += (int)up.EffectValue;
                                break;
                            }

                        case EffectHelper.EffectType.ContractCost:
                            {
                                if (up.TargetId == "aContracts")
                                    foreach (var kv in gg.Contracts)
                                        kv.Value.PriceFactor = ApplyFactor(kv.Value.PriceFactor, up.EffectOp, up.EffectValue);
                                else
                                {
                                    var c = _locate.LocateContract(gg, up.TargetId);
                                    c.PriceFactor = ApplyFactor(c.PriceFactor, up.EffectOp, up.EffectValue);
                                }
                                break;
                            }

                        case EffectHelper.EffectType.CoinGain:
                            {
                                if (up.TargetId == "aCoins")
                                    foreach (var kv in gg.Coins)
                                        ApplyAddMultDirect(kv.Value, up.EffectOp, up.EffectValue);
                                else
                                    ApplyAddMultDirect(_locate.LocateCoin(gg, up.TargetId), up.EffectOp, up.EffectValue);
                                break;
                            }

                        case EffectHelper.EffectType.KnowledgeGain:
                            {
                                if (up.TargetId == "aKnowledges")
                                    foreach (var kv in gg.Knowledges)
                                        ApplyAddMultDirect(kv.Value, up.EffectOp, up.EffectValue);
                                else
                                    ApplyAddMultDirect(_locate.LocateKnowledge(gg, up.TargetId), up.EffectOp, up.EffectValue);
                                break;
                            }

                        case EffectHelper.EffectType.ResourceGain:
                            {
                                if (up.TargetId == "aResources")
                                    foreach (var kv in gg.Resources)
                                        ApplyAddMultDirect(kv.Value, up.EffectOp, up.EffectValue);
                                else
                                    ApplyAddMultDirect(_locate.LocateResource(gg, up.TargetId), up.EffectOp, up.EffectValue);
                                break;
                            }

                        case EffectHelper.EffectType.ClickGain:
                            {
                                if (up.TargetId == "aClicks")
                                    foreach (var kv in gg.Clicks)
                                        ApplyAddMultDirect(kv.Value, up.EffectOp, up.EffectValue);
                                else
                                    ApplyAddMultDirect(_locate.LocateStageClick(gg, up.TargetId), up.EffectOp, up.EffectValue);
                                break;
                            }

                        case EffectHelper.EffectType.ContractGain:
                            {
                                if (up.TargetId == "aContracts")
                                    foreach (var kv in gg.Contracts)
                                        ApplyAddMultDirect(kv.Value, up.EffectOp, up.EffectValue);
                                else
                                    ApplyAddMultDirect(_locate.LocateContract(gg, up.TargetId), up.EffectOp, up.EffectValue);
                                break;
                            }
                    }
                }
            }, save: true);

            await ApplyPartyTraitsAsync();
        }

        // Helpers
        private static bool IsPermanent(UnlockHelper.Persistence p)
            => p == UnlockHelper.Persistence.Permanent
            || p == UnlockHelper.Persistence.untilExpansion;
        private static double ApplyFactor(double f, EffectHelper.EffectOperation op, double v)
            => op switch
            {
                EffectHelper.EffectOperation.Additive => f + v,
                EffectHelper.EffectOperation.Multiplicative => f * v,
                EffectHelper.EffectOperation.Override => v,
                _ => f
            };
        private static void ApplyAddMultDirect(dynamic target, EffectHelper.EffectOperation op, double v)
        {
            // target tem AddMod e MultMod (Coin/Resource/Knowledge/Click/Contract)
            if (op == EffectHelper.EffectOperation.Additive) target.AddMod += v;
            else if (op == EffectHelper.EffectOperation.Multiplicative) target.MultMod *= v;
            else if (op == EffectHelper.EffectOperation.Override) { target.AddMod = 0; target.MultMod = v; }
        }
        private async Task ApplyPartyTraitsAsync()
        {
            var g = _game.CurrentGame;
            var st = GetCurrentStage();
            var ex = GetOrCreateCurrentExpedition();

            foreach (var charId in ex.PartyIds)
            {
                var ch = _locate.LocateCharacter(g, charId);

                await _effect.ApplyEffect(ItemHelper.ItemType.Trait, ch.TraitId, st.Id);
            }
        }


    }
}
