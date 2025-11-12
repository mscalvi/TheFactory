using FurmaIdle.Data;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using FurmaIdle.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Text;

namespace FurmaIdle.Services
{
    public enum HoverType { Character, Specialty, Tech, Local, Upgrade, Contract, Stage, Expansion, Expedition, Knowledge, Coins, Resources }

    public interface ITooltipService
    {
        TooltipModel GetHover(HoverType type, string id, string? stageId = null);

        TooltipModel? Current { get; }
        void Show(TooltipModel tip);
        void Clear();
        event Action? Changed;
    }

    public sealed class TooltipService : ITooltipService
    {
        private readonly ICurrentGameService _game;
        private readonly ILocateService _locate;
        private readonly ICostService _cost;
        private readonly IContractsService _contract;

        public TooltipModel? Current { get; private set; }
        public event Action? Changed;

        public TooltipService(ICurrentGameService game, ILocateService locate, ICostService cost, IContractsService contract)
        {
            _game = game;
            _locate = locate;
            _cost = cost;
            _contract = contract;
        }

        public void Show(TooltipModel tip)
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

        public TooltipModel GetHover(HoverType type, string id, string? stageId = null)
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
                _ => new TooltipModel()
            };
        }

        // Character
        private TooltipModel BuildCharacterHover(string charId, GameModel game)
        {
            var tooltip = new TooltipModel();

            var character = _locate.LocateCharacter(game, charId);

            var spec = _locate.LocateSpecialty(game, character.SpecialtyId);
            string specialty = spec.Name + " -> "+ spec.Description;

            var trait = _locate.LocateTrait(game, character.TraitId);

            var knowledge = new KnowledgeModel();
            string knows = "";
            if(character.KnowledgeFactor2 is not null)
            {
                knowledge = _locate.LocateKnowledge(game, character.KnowledgeFactor2);
                knows += "2x " + knowledge.Name + " ";
            }
            if(character.KnowledgeFactor1  is not null)
            {
                knowledge = _locate.LocateKnowledge(game, character.KnowledgeFactor1);
                knows += "1x " + knowledge.Name;
            }

            var contract = new ContractModel();
            string contracts = "";
            if (character.ContractsIds != null)
            {
                foreach (var contractId in character.ContractsIds)
                {
                    if(contractId != null)
                    {
                        contract = _locate.LocateContract(game, contractId);
                        if (contracts == "")
                        {
                            contracts += contract.Name;
                        }
                        else
                        {
                            contracts += " - " + contract.Name;
                        }
                    }
                }
            }

            var stage = new StageModel();
            string charState = character.Name;
            if (character.CharState == UnlockHelper.CharState.Blocked)
            {
                charState += " - Não Contratado";
            }
            if (character.CharState == UnlockHelper.CharState.InLine)
            {
                charState += " - Esperando Expedição";
            }
            if (character.CharState == UnlockHelper.CharState.InBase)
            {
                charState += " - Na Base";
            }
            if (character.CharState == UnlockHelper.CharState.InStage)
            {
                stage = _locate.LocateStage(game, character.InStageId);
                charState += " - Trabalhando em " + stage.Name;
            }

            var modifiers = character.Modifiers.Count;

            tooltip.Type = "Personagem";
            tooltip.Name = charState;
            tooltip.Lore = character.Lore;
            tooltip.Info.Add("Especialidade", specialty);
            tooltip.Info.Add("Traço", trait.Description);
            tooltip.Info.Add("Fatores", knows);
            tooltip.Info.Add("Contratos", contracts);
            tooltip.Info.Add("Modificadores Ativos", modifiers.ToString());

            return tooltip;
        }


        // Upgrade
        private TooltipModel BuildUpgradeHover(string upgradeId, GameModel game)
        {
            var tooltip = new TooltipModel();

            var upgrade = _locate.LocateUpgrade(game, upgradeId);
            var stage = _locate.LocateStage(game, game.SelectedStageId);

            string nome = upgrade.Name;
            var cost = _cost.ComputeCost(ItemHelper.ItemType.Upgrade, upgrade.Id, stage.Id);

            var coin = _locate.LocateCoin(game, cost.costId);
            nome += " - " + cost.costValue.ToString() + " " + coin.Name;


            string upIdType = upgrade.Id.Length >= 2
                ? upgrade.Id.Substring(0, 1)
                : upgrade.Id;

            switch (upIdType)
            {
                // Unlocks
                case "uu": // Contracts
                    break;
                case "uk": // Knowledge
                    break;
                case "up": // Characters
                    break;
                case "ul": // Locals
                    break;
                case "us": // Stages
                    break;
                case "uh": // Techs
                    break;
                case "ue": // Expansions
                    break;
                case "ur": // Resources
                    break;
                case "ua": // Party Size
                    break;

                // Expeditions
                case "ui": // Click
                    break;
                case "uc": // Contracts Modifiers
                    break;

                // Tech Upgrades
                case "ut": // Target é c ou r
                    break;

                // Expansions
                case "um": // ContractCap
                    break;
                case "ub": // Contract Level Unlock
                    break;
                case "ux": // Target pode ser i, r, aResources, aKnowledges, aContracts, 
                    break;

            }

            var modifiers = upgrade.Modifiers.Count;

            tooltip.Type = "Melhoria";
            tooltip.Name = nome;
            tooltip.Lore = upgrade.Lore;
            tooltip.Info.Add("Modificadores Ativos", modifiers.ToString());

            return tooltip;
        }

        // Contract
        private TooltipModel BuildContractHover(string id, GameModel game)
        {
            var tooltip = new TooltipModel();

            var contract = _locate.LocateContract(game, id);
            var stage = _locate.LocateStage(game, game.SelectedStageId);

            string nome = contract.Name;
            var cost = _cost.ComputeCost(ItemHelper.ItemType.Contract, contract.Id, stage.Id);

            var coin = _locate.LocateCoin(game, cost.costId);
            nome += " - " + cost.costValue.ToString() + " " + coin.Name;

            string nivel = "";
            switch (contract.Level) 
            { 
                case 1:
                    nivel = "Trivial";
                    break;
                case 2:
                    nivel = "Aprendiz";
                    break;
                case 3:
                    nivel = "Iniciante";
                    break;
                case 4:
                    nivel = "Profissional";
                    break;
                case 5:
                    nivel = "Mestre";
                    break;
                case 6:
                    nivel = "Especialista";
                    break;
            }

            double perSec = 0;

            string geraBase = "";
            var baseInfo = ContractHelper.GetContractBase(contract);
            perSec = baseInfo.CoinsPerCycle * baseInfo.SecondsPerCycle;
            geraBase = baseInfo.CoinsPerCycle.ToString("N2") + " " + coin.Name + " a cada " + baseInfo.SecondsPerCycle.ToString("N2") + "s -> " + perSec.ToString("N2") + " " + coin.Name + "/s";

            string geraAtual = "";
            var actualInfo = _contract.GetContractInfo(contract, stage);
            perSec = actualInfo.CoinsPerCycle * actualInfo.SecondsPerCycle;
            geraAtual = actualInfo.CoinsPerCycle.ToString("N2") + " " + coin.Name + " a cada " + actualInfo.SecondsPerCycle.ToString("N2") + "s -> " + perSec.ToString("N2") + " " + coin.Name + "/s";

            var knowledge = new KnowledgeModel();
            string knows = "";
            if (contract.KnowledgeFactor3 is not null)
            {
                knowledge = _locate.LocateKnowledge(game, contract.KnowledgeFactor3);
                knows += "3x " + knowledge.Name + " ";
            }
            if (contract.KnowledgeFactor2 is not null)
            {
                knowledge = _locate.LocateKnowledge(game, contract.KnowledgeFactor2);
                knows += "2x " + knowledge.Name + " ";
            }
            if (contract.KnowledgeFactor1 is not null)
            {
                knowledge = _locate.LocateKnowledge(game, contract.KnowledgeFactor1);
                knows += "1x " + knowledge.Name;
            }
            if (knows == "")
            {
                knows = "-";
            }

            var modifiers = contract.Modifiers.Count;

            tooltip.Type = "Contrato";
            tooltip.Name = nome;
            tooltip.Lore = contract.Lore;
            tooltip.Info.Add("Nível", nivel);
            tooltip.Info.Add("Base", geraBase);
            tooltip.Info.Add("Atual", geraAtual);
            tooltip.Info.Add("Fatores", knows);
            tooltip.Info.Add("Modificadores Ativos", modifiers.ToString());

            return tooltip;
        }


        // Local
        private TooltipModel BuildLocalHover(string id, GameModel game)
        {
            var tooltip = new TooltipModel();

            return tooltip;
        }

        // Techs
        private TooltipModel BuildTechHover(string id, GameModel game)
        {
            var tooltip = new TooltipModel();

            return tooltip;
        }

        // Knowledge
        private TooltipModel BuildKnowledgeHover(string id, GameModel game)
        {
            var tooltip = new TooltipModel();

            return tooltip;
        }

        // Specialty
        private TooltipModel BuildSpecialtyHover(string id, GameModel game)
        {
            var tooltip = new TooltipModel();

            return tooltip;
        }

        // Resources
        private TooltipModel BuildResourcesHover(string id, GameModel game)
        {
            var tooltip = new TooltipModel();

            return tooltip;
        }

        // Coins
        private TooltipModel BuildCoinsHover(string id, GameModel game)
        {
            var tooltip = new TooltipModel();

            return tooltip;
        }
    }
}
