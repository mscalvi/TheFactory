// Services/TooltipService.cs
using FurmaIdle.Data;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using FurmaIdle.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FurmaIdle.Services
{
    public enum HoverType { Personagem, Especialidade, Tecnologia, Destino, Melhoria }
    public sealed record HoverTip(string Title, string Body);

    public interface ITooltipService
    {
        HoverTip GetHover(HoverType type, string id, string? stageId = null);
    }

    public sealed class TooltipService : ITooltipService
    {
        private readonly IGameService _game;
        private readonly IUpgradeService _effects;

        public TooltipService(IGameService game, IUpgradeService effects)
        {
            _game = game;
            _effects = effects;
        }

        public HoverTip GetHover(HoverType type, string id, string? stageId = null)
        {
            return type switch
            {
                HoverType.Personagem => BuildPersonHover(id, stageId ?? _game.SelectedStageId),
                HoverType.Especialidade => BuildEspecialidadeHover(id),
                HoverType.Tecnologia => BuildTecnologiaHover(id),
                HoverType.Destino => BuildDestinoHover(id),
                HoverType.Melhoria => BuildMelhoriaHover(id),
                _ => new HoverTip("—", "—")
            };
        }

        // ===================== Personagem =====================
        private HoverTip BuildPersonHover(string id, string stageId)
        {
            _game.Current.Characters.TryGetValue(id, out var live);
            // usa definição só para nomes/listas estáticas
            CharacterModel? def = null;
            try { def = CharacterData.GetDef(id); } catch { /* seguro */ }

            var name = NameOrId(live?.Name ?? def?.Name, id);

            // status Base/Expedição
            string status = "Na Base";
            if (live?.CharState == CharStateEnum.CharState.OnStage)
            {
                var stName = LookupData.Stage(_game.Current, null, live.CharDestId ?? stageId).Name;
                status = $"Em expedição: {stName}";
            }

            // conhecimentos
            string knows = "—";
            var ch = (live ?? def);
            if (ch is not null)
            {
                var listK = new[] { ch.MainKnowId, ch.SecondKnowId }
                    .Where(s => !string.IsNullOrWhiteSpace(s));
                knows = listK.Any() ? string.Join(", ", listK) : "—";
            }

            // contratos conhecidos + mult efetivos (ganho/tempo)
            var contractIds = ch?.KnowContractsIds ?? new List<string>();
            string contractsStr = "—";
            if (contractIds.Count > 0)
            {
                var items = new List<string>(contractIds.Count);
                foreach (var cid in contractIds)
                {
                    // nome
                    var cname = TryName(() => ContractData.GetDef(cid)?.Name, cid);
                    // multiplicadores atuais
                    var g = _effects.ContractGainMult(cid);
                    var t = _effects.ContractTimeMult(cid);
                    // x1.10 e x0.90 curtos
                    items.Add($"{cname} ({cid}) · ganho x{g:0.##} · tempo x{t:0.##}");
                }
                contractsStr = string.Join("\n", items);
            }

            // cap por personagem
            int baseCap = Math.Max(0, live?.MaxContracts ?? def?.MaxContracts ?? 0);
            int extra = _effects.ExtraContractsPerChar();
            int capEff = Math.Max(0, baseCap + extra);
            string traitLine = SummarizeTrait(live?.TraitId ?? def?.TraitId);

            var body =
                $"Status: {status}\n" +
                $"Conhecimentos: {knows}\n" +
                $"Especialidade: {ch?.SpecialtyId ?? "—"}\n" +
                $"Cap. de contratos (por personagem): {capEff}  (base {baseCap} + extra {extra})\n" +
                $"Contratos: {contractsStr}\n" + 
                $"{traitLine}";

            return new HoverTip($"{name} ({id})", body);
        }

        private string SummarizeTrait(string? traitId)
        {
            if (string.IsNullOrWhiteSpace(traitId)) return "Traço: —";
            TraitModel t;
            try { t = TraitData.GetDef(traitId); } catch { return $"Traço: {traitId}"; }

            var sb = new StringBuilder();
            sb.Append($"Traço: {t.Name}");
            var hasDetail = false;

            if (t.CharacterCostMult != 1.0)
            {
                var perc = (1.0 - t.CharacterCostMult) * 100.0;
                sb.Append($"\n• Contratação de personagem: -{perc:0.#}%");
                hasDetail = true;
            }
            if (t.AddPerSecond != 0 && !string.IsNullOrWhiteSpace(t.ResourceId))
            {
                sb.Append($"\n• +{t.AddPerSecond:0.##}/s em {t.ResourceId}");
                hasDetail = true;
            }
            if (t.GainMult != 1.0 && !string.IsNullOrWhiteSpace(t.KnowledgeId))
            {
                sb.Append($"\n• Knowledge {t.KnowledgeId}: x{t.GainMult:0.##}");
                hasDetail = true;
            }

            return hasDetail ? sb.ToString() : $"Traço: {t.Name}";
        }

        // ===================== Especialidade =====================
        private HoverTip BuildEspecialidadeHover(string charId)
        {
            if (!_game.Current.Characters.TryGetValue(charId, out var ch) || string.IsNullOrWhiteSpace(ch.SpecialtyId))
                return new HoverTip("Especialidade", "—");

            var spec = SpecialtyData.GetDef(ch.SpecialtyId);
            var cost = $"{spec.Cost:N0} {spec.CostResourceId}";
            var dur = TimeSpan.FromSeconds(spec.DurationSec);

            string desc = spec.Id switch
            {
                "e00" => $"Produção Instantânea\n• Completa 1 ciclo de todos os contratos ativos.\n• Recarga: {dur:mm\\:ss}\n• Custo: {cost}",
                "e01" => $"Geração de {spec.ResourceIdScope}: x{spec.Value:0.##}\nDuração: {dur:mm\\:ss}\nCusto: {cost}",
                "e02" => $"Ganho de contratos (coins): x{spec.Value:0.##}\nDuração: {dur:mm\\:ss}\nCusto: {cost}",
                "e03" => $"Consumo de {spec.ResourceIdScope}: x{spec.Value:0.##}\nDuração: {dur:mm\\:ss}\nCusto: {cost}",
                _ => $"Duração: {dur:mm\\:ss}\nCusto: {cost}"
            };

            return new HoverTip($"Especialidade ({spec.Id})", desc);
        }


        // ===================== Tecnologia =====================
        private HoverTip BuildTecnologiaHover(string id)
        {
            // tenta pegar o "live" sem usar ?. em if
            TechModel? live = null;
            var techs = _game.Current.Technologies;
            if (techs != null) techs.TryGetValue(id, out live);

            // catálogo (pode falhar)
            TechModel? def = null;
            try { def = TechData.GetDef(id); } catch { /* safe */ }

            // se não houver nem live nem def, retorna placeholder
            if (live is null && def is null)
                return new HoverTip($"Tecnologia {id}", "Dados indisponíveis.");

            var name = NameOrId(live?.Name ?? def?.Name, id);

            // destino
            var destId = def?.DestinationId ?? live?.DestinationId ?? "";
            var destName = string.IsNullOrWhiteSpace(destId)
                ? "—"
                : TryName(() => DestinationData.GetDef(destId).Name, destId);

            // custo (knowledge)
            var kId = def?.CostKnowledgeId ?? live?.CostKnowledgeId ?? "k??";
            var cost = Math.Max(0, def?.Cost ?? live?.Cost ?? 0);

            // saldo do conhecimento
            string haveStr = "";
            try
            {
                int have = 0;
                if (_game.Current.Knowledges != null &&
                    _game.Current.Knowledges.TryGetValue(kId, out var k) && k is not null)
                    have = k.Points;
                haveStr = $" (você tem {have:N0})";
            }
            catch { /* safe */ }

            // estado
            bool unlocked = live?.Unlocked == true;
            bool avaliable = live?.Avaliable == true;
            string state = unlocked ? "Pesquisada"
                        : (avaliable ? "Disponível para pesquisa" : "Bloqueada");

            // motivo de bloqueio mais comum
            string reason = "";
            try
            {
                if (!unlocked && !avaliable && !string.IsNullOrWhiteSpace(destId))
                {
                    if (_game.Current.Destinations != null &&
                        _game.Current.Destinations.TryGetValue(destId, out var d) &&
                        d is not null && !d.Unlocked)
                    {
                        reason = $"\nRequer destino: {destName} ({destId}).";
                    }
                }
            }
            catch { /* safe */ }

            var title = $"{name} ({id})";
            var body =
                $"Destino: {destName} ({destId})\n" +
                $"Custo: {cost:N0} {kId}{haveStr}\n" +
                $"Status: {state}{reason}";

            return new HoverTip(title, body);
        }


        // ===================== Destino =====================
        private HoverTip BuildDestinoHover(string id)
        {
            DestinationModel? def = null;
            try { def = DestinationData.GetDef(id); } catch { /* seguro */ }

            if (def is null) return new HoverTip($"Destino {id}", "Dados indisponíveis.");

            var resName = TryName(() => ResourceData.GetDef(def.CostResourceId).Name, def.CostResourceId);
            var stageName = TryName(() => StageData.GetDef(def.StageId).Name, def.StageId);

            var title = $"{def.Name} ({id})";
            var body =
                $"Stage: {stageName} ({def.StageId})\n" +
                $"Custo: {def.Cost:N0} {resName} ({def.CostResourceId})\n" +
                $"Estado: {(def.Unlocked ? "Desbloqueado" : "Bloqueado")}";

            return new HoverTip(title, body);
        }

        // ===================== Melhoria (Upgrade) =====================
        private HoverTip BuildMelhoriaHover(string id)
        {
            // catálogo + vivo
            UpgradeModel? def = null;
            try { def = UpgradeData.GetDef(id); } catch { /* seguro */ }
            _game.Current.Upgrades.TryGetValue(id, out var live);

            var title = NameOrId(def?.Name ?? live?.Name, id) + $" ({id})";

            if (def is null && live is null)
                return new HoverTip(title, "Dados indisponíveis.");

            int buys = live?.Buys ?? 0;
            int max = (live?.MaxBuys ?? def?.MaxBuys) ?? 0;
            bool isMax = (live?.IsMaxed ?? false) || (max > 0 && buys >= max);

            // custo próximo
            string costLine = "—";
            try
            {
                if (isMax) costLine = "Maxeada";
                else
                {
                    var uref = live ?? def!;
                    var resId = string.IsNullOrWhiteSpace(uref.CostResourceId) ? "r001" : uref.CostResourceId;
                    var resName = TryName(() => ResourceData.GetDef(resId).Name, resId);
                    double next = Helpers.UpgradePricingHelper.NextPrice(uref);
                    costLine = $"Próximo custo: {next:N0} {resName} ({resId})";
                }
            }
            catch { /* seguro */ }

            // efeitos legíveis
            var effects = SummarizeEffects(def?.Effects ?? live?.Effects);

            // gating por tecnologia (se houver TechId no catálogo)
            string techReq = string.Empty;
            var techId = def?.TechId ?? live?.TechId;
            if (!string.IsNullOrWhiteSpace(techId))
            {
                bool unlocked = _game.Current.Technologies != null
                    && _game.Current.Technologies.TryGetValue(techId!, out var t) && t.Unlocked;
                techReq = unlocked ? "" : $"\nRequer Tecnologia {techId}.";
            }

            var body =
                $"{effects}\n" +
                $"Compras: {buys}/{(max <= 0 ? "∞" : max)}\n" +
                $"{costLine}{techReq}";

            return new HoverTip(title, body);
        }

        // ===================== Helpers =====================
        private static string NameOrId(string? name, string id)
            => string.IsNullOrWhiteSpace(name) ? id : name!;

        private static string TryName(Func<string?> getter, string fallbackId)
        {
            try
            {
                var s = getter?.Invoke();
                return string.IsNullOrWhiteSpace(s) ? fallbackId : s!;
            }
            catch { return fallbackId; }
        }

        private string SummarizeEffects(List<UpgradeEffectModel>? list)
        {
            if (list is null || list.Count == 0) return "Sem efeitos.";
            var sb = new StringBuilder();
            foreach (var e in list)
            {
                var scope = string.IsNullOrWhiteSpace(e.ScopeId) ? "all" : e.ScopeId;
                switch (e.Target)
                {
                    case EffectTarget.ContractGain:
                        sb.AppendLine($"Contrato {scope}: ganho x{e.Value:0.##}");
                        break;
                    case EffectTarget.ContractTime:
                        sb.AppendLine($"Contrato {scope}: tempo x{e.Value:0.##}");
                        break;
                    case EffectTarget.ClicksGain:
                        sb.AppendLine($"Clicks: x{e.Value:0.##}");
                        break;
                    case EffectTarget.ResourceGen:
                        sb.AppendLine($"Geração de recurso {scope}: {(e.Value >= 0 ? "+" : "")}{e.Value:0.##}/s");
                        break;
                    case EffectTarget.ContractCap:
                        sb.AppendLine($"Cap. de contratos ({scope}): {(e.Value >= 0 ? "+" : "")}{e.Value:0.##} por personagem");
                        break;
                    default:
                        sb.AppendLine($"{e.Target} {scope}: {e.Value}");
                        break;
                }
            }
            return sb.ToString().TrimEnd();
        }
    }
}
