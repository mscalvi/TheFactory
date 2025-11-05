using FurmaIdle.Data;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using FurmaIdle.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FurmaIdle.Services
{
    public enum HoverType { Character, Specialty, Tech, Local, Upgrade, Contract, Stage, Expansion, Expedition, Knowledge, Coins, Resources }
    public sealed record HoverTip(string Title, string Body);

    public interface ITooltipService
    {
        HoverTip GetHover(HoverType type, string id, string? stageId = null);

        HoverTip? Current { get; }
        void Show(HoverTip tip);
        void Clear();
        event Action? Changed;
    }

    public sealed class TooltipService : ITooltipService
    {
        private readonly ICurrentGameService _game;
        private readonly ILocateService _locate;
        private readonly ICostService _cost;
        private readonly IContractsService _contract;

        public HoverTip? Current { get; private set; }
        public event Action? Changed;

        public TooltipService(ICurrentGameService game, ILocateService locate, ICostService cost, IContractsService contract)
        {
            _game = game;
            _locate = locate;
            _cost = cost;
            _contract = contract;
        }

        public void Show(HoverTip tip)
        {
            Current = tip;
            Changed?.Invoke();
        }

        public void Clear()
        {
            if (Current is null) return;
            Current = null;
            Changed?.Invoke();
        }

        public HoverTip GetHover(HoverType type, string id, string? stageId = null)
        {
            var g = _game.CurrentGame;
            return type switch
            {
                HoverType.Character => BuildCharacterHover(id, g),
                HoverType.Contract => BuildContractHover(id, g),
                HoverType.Specialty => BuildSpecialtyHover(id, g),
                HoverType.Tech => BuildTechHover(id, g),
                HoverType.Local => BuildLocalHover(id, g),
                HoverType.Upgrade => BuildUpgradeHover(id, g),
                HoverType.Knowledge => BuildKnowledgeHover(id, g),
                HoverType.Resources => BuildResourcesHover(id, g),
                HoverType.Coins => BuildCoinsHover(id, g),
                _ => new HoverTip("—", "—")
            };
        }

        // Character
        // dentro do TooltipService (ou onde você já faz os builders)
        private HoverTip BuildCharacterHover(string charId, GameModel g)
        {
            var ch = _locate.LocateCharacter(g, charId);

            // Nomes/descrições vindos do Locate (ajuste os métodos conforme seu LocateService)
            var spec = !string.IsNullOrWhiteSpace(ch.SpecialtyId) ? _locate.LocateSpecialty(g, ch.SpecialtyId) : null;
            var trait = !string.IsNullOrWhiteSpace(ch.TraitId) ? _locate.LocateTrait(g, ch.TraitId) : null;
            var k1 = !string.IsNullOrWhiteSpace(ch.KnowledgeFactor1) ? _locate.LocateKnowledge(g, ch.KnowledgeFactor1) : null;
            var k2 = !string.IsNullOrWhiteSpace(ch.KnowledgeFactor2) ? _locate.LocateKnowledge(g, ch.KnowledgeFactor2) : null;

            // Contratos possíveis (IDs -> nomes)
            var contracts = new List<string>();
            if (ch.ContractsIds != null)
            {
                foreach (var cid in ch.ContractsIds)
                {
                    var c = _locate.LocateContract(g, cid);
                    contracts.Add(!string.IsNullOrWhiteSpace(c?.Name) ? c.Name : cid);
                }
            }

            // Monta HTML (corpo) seguindo o layout pedido
            // Obs.: ch.Lore pode estar vazio pelos seus dados atuais; deixei a seção, mas só renderiza se tiver conteúdo.
            string body = $@"
                <div class='tt'>
                  <div class='tt-name'>{HtmlEncode(ch.Name)}</div>
                  {(string.IsNullOrWhiteSpace(ch.Lore) ? "" : $"<div class='tt-lore'><em>{HtmlEncode(ch.Lore)}</em></div>")}

                  <div class='tt-list'>
                    {(spec is null ? "" : $"<div><strong>{HtmlEncode(spec.Name)}:</strong> {HtmlEncode(spec.Description ?? "")}</div>")}
                    {(trait is null ? "" : $"<div>{HtmlEncode(trait.Description ?? "")}</div>")}
                  </div>

                  <div class='tt-know'>
                    {(k1 is null && k2 is null ? "" : $"{HtmlEncode(k1?.Name ?? "")}{(k1 != null && k2 != null ? " — " : "")}{HtmlEncode(k2?.Name ?? "")}")}
                  </div>

                  {(contracts.Count == 0 ? "" : $"<div class='tt-footnote'>{HtmlEncode(string.Join(" • ", contracts))}</div>")}
                </div>";

            // Title pode ficar só o nome (você queria centralizado/negrito — isso está no HTML do body).
            return new HoverTip(
                Title: ch.Name,  // mantém por compatibilidade (não precisa usar visualmente)
                Body: body
            );
        }


        // Upgrade
        private HoverTip BuildUpgradeHover(string upgradeId, GameModel game)
        {
            var up = _locate.LocateUpgrade(game, upgradeId);

            var stageId = game.SelectedStageId;

            var (costValue, costId) = _cost.ComputeCost(ItemHelper.ItemType.Upgrade, upgradeId, stageId);

            string costCoinName = costId;
            try
            {
                var coin = _locate.LocateCoin(game, costId);
                if (!string.IsNullOrWhiteSpace(coin?.Name))
                    costCoinName = coin.Name;
            }
            catch { /* ok manter o id */ }

            string title = up?.Name ?? upgradeId;
            string lore = up?.Lore ?? "";
            string desc = up?.Description ?? "";

            string costLine = $"{Format(costValue)} {costCoinName}";

            string body = $@"
                <div class='tt'>
                  <div class='tt-name'>{HtmlEncode(title)}</div>
                  {(string.IsNullOrWhiteSpace(lore) ? "" : $"<div class='tt-lore'><em>{HtmlEncode(lore)}</em></div>")}
                  {(string.IsNullOrWhiteSpace(desc) ? "" : $"<div class='tt-list'><div>{HtmlEncode(desc)}</div></div>")}
                  {(string.IsNullOrWhiteSpace(up.TargetId) ? "" : $"<div class='tt-list'><div>{HtmlEncode(up.TargetId)}</div></div>")}
                  <div class='tt-know tt-cost'>Custo: {HtmlEncode(costLine)}</div>
                </div>";

            return new HoverTip(title, body);
        }

        // Contract
        private HoverTip BuildContractHover(string id, GameModel game)
        {
            game.Contracts.TryGetValue(id, out var contract);
            if (contract is null)
                return new HoverTip("Contract (?)", "—");

            var stage = _locate.LocateStage(game, game.SelectedStageId);
            var coin = _locate.LocateCoin(game, contract.CoinId);

            var real = _contract.GetContractInfo(contract, stage);

            // Lista de Knowledges (filtra nulos)
            var knows = new List<string>();
            if (!string.IsNullOrWhiteSpace(contract.KnowledgeFactor1) && game.Knowledges.TryGetValue(contract.KnowledgeFactor1, out var k1) && k1 is not null)
                knows.Add(k1.Name);
            if (!string.IsNullOrWhiteSpace(contract.KnowledgeFactor2) && game.Knowledges.TryGetValue(contract.KnowledgeFactor2, out var k2) && k2 is not null)
                knows.Add(k2.Name);
            if (!string.IsNullOrWhiteSpace(contract.KnowledgeFactor3) && game.Knowledges.TryGetValue(contract.KnowledgeFactor3, out var k3) && k3 is not null)
                knows.Add(k3.Name);

            double perSecond = real.CoinsPerCycle / real.SecondsPerCycle;
            var coinName = coin?.Name ?? contract.CoinId;
            var title = $"Contrato ({contract.Name})";
            var body = $"Gera: ~{perSecond:0.##} {coinName}/s"
                      + (knows.Count > 0 ? $"\nConhecimentos: {knows.ToString()}" : "\nConhecimentos: —");

            return new HoverTip(title, body);
            }


        // Local
        private HoverTip BuildLocalHover(string id, GameModel game)
        {
            game.Locals.TryGetValue(id, out var local);

            return new HoverTip($"Local ({local.Name})", $"{local.State}");
        }

        // Techs
        private HoverTip BuildTechHover(string id, GameModel game)
        {
            game.Techs.TryGetValue(id, out var tech);

            return new HoverTip($"Tech ({tech.Name})", $"{tech.State}");
        }

        // Techs
        private HoverTip BuildKnowledgeHover(string id, GameModel game)
        {
            game.Knowledges.TryGetValue(id, out var know);

            return new HoverTip($"Knowledge ({know.Name})", $"{know.State}");
        }

        // Techs
        private HoverTip BuildSpecialtyHover(string id, GameModel game)
        {
            game.Specialties.TryGetValue(id, out var specialty);

            return new HoverTip($"Specialty ({specialty.Name})", $"{specialty.Description}");
        }

        // Resources
        private HoverTip BuildResourcesHover(string id, GameModel game)
        {
            game.Resources.TryGetValue(id, out var resource);

            return new HoverTip($"Resource ({resource.Name})", $"{resource.Lore}");
        }

        // Coins
        private HoverTip BuildCoinsHover(string id, GameModel game)
        {
            game.Coins.TryGetValue(id, out var coin);

            return new HoverTip($"Coin ({coin.Name})", $"{coin.Lore}");
        }


        // Utilitário simples pra escapar (caso seus textos venham “soltos”)
        private static string HtmlEncode(string? s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return System.Net.WebUtility.HtmlEncode(s);
        }

        private static string Format(long v)
          => v.ToString("N0", CultureInfo.InvariantCulture);
    }
}
