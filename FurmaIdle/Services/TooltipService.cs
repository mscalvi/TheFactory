using FurmaIdle.Data;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using FurmaIdle.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using static FurmaIdle.Helpers.EffectHelper;
using static FurmaIdle.Helpers.UnlockHelper;

namespace FurmaIdle.Services
{
    public enum HoverType { Character, Specialty, Tech, Local, Upgrade, Contract, Stage, Expedition, Knowledge, Coins, Resources }

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
        private readonly IExpeditionService _expedition;
        private readonly IModifierService _modifier;
        private readonly IResourcesService _resources;

        public TooltipModel? Current { get; private set; }
        public event Action? Changed;

        public TooltipService(ICurrentGameService game, ILocateService locate, ICostService cost, IContractsService contract, IExpeditionService expedition, IModifierService modifier, IResourcesService resources)
        {
            _game = game;
            _locate = locate;
            _cost = cost;
            _contract = contract;
            _expedition = expedition;
            _modifier = modifier;
            _resources = resources;
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
                HoverType.Expedition => BuildExpeditionHover(id, g),
                HoverType.Stage => BuildStageHover(id, g),
                _ => new TooltipModel()
            };
        }

        // Upgrade
        private TooltipModel BuildUpgradeHover(string upgradeId, GameModel game)
        {
            var tooltip = new TooltipModel();

            var upgrade = _locate.LocateUpgrade(game, upgradeId);
            var stageIn = _locate.LocateStage(game, game.SelectedStageId);

            var cost = _cost.ComputeCost(ItemHelper.ItemType.Upgrade, upgrade.Id, stageIn.Id);

            string upIdType = upgrade.Id.Length >= 2
                ? upgrade.Id.Substring(0, 2)
                : upgrade.Id;

            var costCoin = new CoinModel();
            var costResource = new ResourceModel();
            var costKnowledge = new KnowledgeModel();

            string costAmount = NumbersHelper.Padronize(cost.costValue);
            string costIcon = "";
            string costName = "";

            if (upIdType == "xx")
            {
                costResource = _locate.LocateResource(game, cost.costId);
                costIcon = costResource.Image;
                costName = costResource.Name;
            }
            else if (upIdType == "uh")
            {
                costKnowledge = _locate.LocateKnowledge(game, cost.costId);
                costIcon = costKnowledge.Image;
                costName = costKnowledge.Name;
            }
            else
            {
                costCoin = _locate.LocateCoin(game, cost.costId);
                costIcon = costCoin.Image;
                costName = costCoin.Name;
            }

            string type = "";
            switch (upgrade.EffectSupertype)
            {
                case EffectHelper.EffectSupertype.Gain:
                    type = "icons/tooltip/types/gain.svg";
                    break;
                case EffectHelper.EffectSupertype.Cost:
                    type = "icons/tooltip/types/cost.svg";
                    break;
                case EffectHelper.EffectSupertype.Time:
                    type = "icons/tooltip/types/time.svg";
                    break;
                case EffectHelper.EffectSupertype.Unlock:
                    type = "icons/tooltip/types/unlock.svg";
                    break;
                case EffectHelper.EffectSupertype.Cap:
                    type = "icons/tooltip/types/cap.svg";
                    break;
            }

            string target = "";

            string upTargetId = upgrade.TargetId.Length >= 1
                ? upgrade.TargetId.Substring(0, 1)
                : upgrade.TargetId;

            switch (upTargetId)
            {
                case "a":
                    switch (upgrade.TargetId)
                    {
                        case "aCharacters":
                            target = "icons/tooltip/targets/characters.svg";
                            break;
                        case "aContracts":
                            target = "icons/tooltip/targets/contracts.svg";
                            break;
                        case "aSpecialties":
                            target = "icons/tooltip/targets/specialties.svg";
                            break;
                        case "aKnowledges":
                            target = "icons/tooltip/targets/knowledges.svg";
                            break;
                        case "aResources":
                            target = "icons/tooltip/targets/resources.svg";
                            break;
                    }
                    break;
                case "c":
                    var contract = _locate.LocateContract(game, upgrade.TargetId);
                    target = contract.Icon;
                    break;
                case "m":
                    var coin = _locate.LocateCoin(game, upgrade.TargetId);
                    target = coin.Icon;
                    break;
                case "i":
                    var click = _locate.LocateClick(game, upgrade.TargetId);
                    target = click.Icon;
                    break;
                case "s":
                    var stage = _locate.LocateStage(game, upgrade.TargetId);
                    target = stage.Icon;
                    break;
                case "l":
                    var local = _locate.LocateLocal(game, upgrade.TargetId);
                    target = local.Icon;
                    break;
                case "x":
                    var expansion = _locate.LocateExpansion(game, upgrade.TargetId);
                    target = expansion.Icon;
                    break;
                case "p":
                    var character = _locate.LocateCharacter(game, upgrade.TargetId);
                    target = character.Icon;
                    break;
                case "u":
                    var upgradeTarget = _locate.LocateUpgrade(game, upgrade.TargetId);
                    target = upgradeTarget.Icon;
                    break;
                case "t":
                    var tech = _locate.LocateTech(game, upgrade.TargetId);
                    target = tech.Icon;
                    break;
                case "k":
                    var know = _locate.LocateKnowledge(game, upgrade.TargetId);
                    target = know.Icon;
                    break;
                case "r":
                    var resour = _locate.LocateResource(game, upgrade.TargetId);
                    target = resour.Icon;
                    break;
            }

            string value = "";
            if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
            {
                value = "Base +" + NumbersHelper.Padronize(upgrade.EffectValue);
            }
            if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
            {
                value = "Total x" + NumbersHelper.Padronize(upgrade.EffectValue);
            }
            if (upgrade.EffectOp == EffectHelper.EffectOperation.Unlock)
            {
                value = "icons/tooltip/operations/new.svg";
            }

            string permanence = "";
            if(upgrade.Persistence == Persistence.Permanent)
            {
                permanence = "icons/tooltip/permanence/permanent.svg";
            }
            if (upgrade.Persistence == Persistence.untilExpansion)
            {
                permanence = "icons/tooltip/permanence/expansion.svg";
            }
            if (upgrade.Persistence == Persistence.untilExpedition)
            {
                permanence = "icons/tooltip/permanence/expedition.svg";
            }
            if (upgrade.Persistence == Persistence.untilTimer)
            {
                permanence = "icons/tooltip/permanence/timer.svg";
            }

            tooltip.Name = upgrade.Name;
            tooltip.CostAmount = costAmount;
            tooltip.CostIcon = costIcon;
            tooltip.CostName = costName;
            tooltip.Description = upgrade.Description;
            tooltip.Info.Add("Tipo", type);
            tooltip.Info.Add("Alvo", target);
            tooltip.Info.Add("Valor", value);
            tooltip.Info.Add("Permanência", permanence);
            tooltip.Lore = upgrade.Lore;

            return tooltip;
        }

        // Contract
        private TooltipModel BuildContractHover(string id, GameModel game)
        {
            var tooltip = new TooltipModel();

            var contract = _locate.LocateContract(game, id);
            var stage = _locate.LocateStage(game, game.SelectedStageId);

            var cost = _cost.ComputeCost(ItemHelper.ItemType.Contract, contract.Id, stage.Id);

            var coin = _locate.LocateCoin(game, cost.costId);
            string custo = NumbersHelper.Padronize(cost.costValue);

            string costAmount = custo;
            string costIcon = coin.Image;
            string costName = coin.Name;

            string level = "";
            switch (contract.Level)
            {
                case 1:
                    level = "icons/tooltip/contracts/trivial.svg";
                    break;
                case 2:
                    level = "icons/tooltip/contracts/aprendiz.svg";
                    break;
                case 3:
                    level = "icons/tooltip/contracts/novato.svg";
                    break;
                case 4:
                    level = "icons/tooltip/contracts/profissional.svg";
                    break;
                case 5:
                    level = "icons/tooltip/contracts/mestre.svg";
                    break;
                case 6:
                    level = "icons/tooltip/contracts/especialista.svg";
                    break;
            }

            double perSec = 0;

            string gainBase = "";
            var baseInfo = ContractHelper.GetContractBase(contract);
            perSec = baseInfo.CoinsPerCycle / baseInfo.SecondsPerCycle;
            gainBase = NumbersHelper.Padronize(perSec) + " " + coin.Name + "/s";

            string gainActual = "";
            var actualInfo = _contract.GetContractInfo(contract, stage);
            perSec = actualInfo.CoinsPerCycle / actualInfo.SecondsPerCycle;
            gainActual = NumbersHelper.Padronize(perSec) + " " + coin.Name + "/s";
            
            string knows = "";
            if (!string.IsNullOrWhiteSpace(contract.KnowledgeFactor1))
            {
                knows += contract.KnowledgeFactor1;
            } else
            {
                knows += "k00";
            }
            if (!string.IsNullOrWhiteSpace(contract.KnowledgeFactor2))
            {
                knows += contract.KnowledgeFactor2;
            }
            else
            {
                knows += "k00";
            }
            knows += ".svg";

            tooltip.Name = contract.Name;
            tooltip.CostAmount = costAmount;
            tooltip.CostIcon = costIcon;
            tooltip.CostName = costName;
            tooltip.Description = contract.Description;
            tooltip.Info.Add("Nível", level);
            tooltip.Info.Add("Fator", knows);
            tooltip.Info.Add("Base", gainBase);
            tooltip.Info.Add("Atual", gainActual);
            tooltip.Lore = contract.Lore;

            return tooltip;
        }

        // Specialty
        private TooltipModel BuildSpecialtyHover(string id, GameModel game)
        {
            var tooltip = new TooltipModel();

            var specialty = _locate.LocateSpecialty(game, id);
            var stageIn = _locate.LocateStage(game, game.SelectedStageId);

            var cost = _cost.ComputeCost(ItemHelper.ItemType.Specialty, specialty.Id, stageIn.Id);

            var costResource = new ResourceModel();

            costResource = _locate.LocateResource(game, cost.costId);
            string custo = NumbersHelper.Padronize(cost.costValue);

            string costAmount = custo;
            string costIcon = costResource.Image;
            string costName = costResource.Name;

            string specTarget = specialty.TargetId.Length >= 2
                ? specialty.TargetId.Substring(0, 1)
                : specialty.TargetId;

            string target = "";
            switch (specTarget)
            {
                case "a":
                    switch (specialty.TargetId)
                    {
                        case "aCharacters":
                            target = "icons/tooltip/targets/characters.svg";
                            break;
                        case "aContracts":
                            target = "icons/tooltip/targets/contracts.svg";
                            break;
                        case "aSpecialties":
                            target = "icons/tooltip/targets/specialties.svg";
                            break;
                        case "aKnowledges":
                            target = "icons/tooltip/targets/knowledges.svg";
                            break;
                        case "aResources":
                            target = "icons/tooltip/targets/resources.svg";
                            break;
                    }
                    break;
                case "c":
                    var contract = _locate.LocateContract(game, specialty.TargetId);
                    target = contract.Icon;
                    break;
                case "m":
                    var coin = _locate.LocateCoin(game, specialty.TargetId);
                    target = coin.Icon;
                    break;
                case "i":
                    var click = _locate.LocateClick(game, specialty.TargetId);
                    target = click.Icon;
                    break;
                case "s":
                    var stage = _locate.LocateStage(game, specialty.TargetId);
                    target = stage.Icon;
                    break;
                case "l":
                    var local = _locate.LocateLocal(game, specialty.TargetId);
                    target = local.Icon;
                    break;
                case "x":
                    var expansion = _locate.LocateExpansion(game, specialty.TargetId);
                    target = expansion.Icon;
                    break;
                case "p":
                    var character = _locate.LocateCharacter(game, specialty.TargetId);
                    target = character.Icon;
                    break;
                case "u":
                    var upgradeTarget = _locate.LocateUpgrade(game, specialty.TargetId);
                    target = upgradeTarget.Icon;
                    break;
                case "t":
                    var tech = _locate.LocateTech(game, specialty.TargetId);
                    target = tech.Icon;
                    break;
                case "k":
                    var know = _locate.LocateKnowledge(game, specialty.TargetId);
                    target = know.Icon;
                    break;
                case "r":
                    var resour = _locate.LocateResource(game, specialty.TargetId);
                    target = resour.Icon;
                    break;
            }

            string value = "";
            if (specialty.EffectOp == EffectHelper.EffectOperation.Additive)
            {
                value = "Base +" + NumbersHelper.Padronize(specialty.EffectValue);
            }
            if (specialty.EffectOp == EffectHelper.EffectOperation.Multiplicative)
            {
                value = "Total x" + NumbersHelper.Padronize(specialty.EffectValue);
            }
            if (specialty.EffectOp == EffectHelper.EffectOperation.Unlock)
            {
                value = "icons/tooltip/operations/new.svg";
            }

            string type = "";
            switch (specialty.EffectSupertype)
            {
                case EffectHelper.EffectSupertype.Gain:
                    type = "icons/tooltip/types/gain.svg";
                    break;
                case EffectHelper.EffectSupertype.Cost:
                    type = "icons/tooltip/types/cost.svg";
                    break;
                case EffectHelper.EffectSupertype.Time:
                    type = "icons/tooltip/types/time.svg";
                    break;
                case EffectHelper.EffectSupertype.Unlock:
                    type = "icons/tooltip/types/unlock.svg";
                    break;
                case EffectHelper.EffectSupertype.Cap:
                    type = "icons/tooltip/types/cap.svg";
                    break;
            }


            var timerModifiers = _modifier.GetModifiers(ItemHelper.ItemType.Specialty, specialty.Id, stageIn.Id, EffectSupertype.Time);
            double duration = (specialty.Duration + timerModifiers.AddMod) * timerModifiers.MultMod;

            string specDuration = NumbersHelper.Padronize(duration);

            tooltip.Name = specialty.Name;
            tooltip.CostAmount = costAmount;
            tooltip.CostIcon = costIcon;
            tooltip.CostName = costName;
            tooltip.Description = specialty.Description;
            tooltip.Info.Add("Tipo", type);
            tooltip.Info.Add("Alvo", target);
            tooltip.Info.Add("Valor", value);
            tooltip.Info.Add("Duração", specDuration);
            tooltip.Lore = specialty.Lore;

            return tooltip;
        }

        // Character
        private TooltipModel BuildCharacterHover(string charId, GameModel game)
        {
            var tooltip = new TooltipModel();

            var character = _locate.LocateCharacter(game, charId);

            var specialty = _locate.LocateSpecialty(game, character.SpecialtyId);

            var trait = _locate.LocateTrait(game, character.TraitId);

            string knows = "";
            if (!string.IsNullOrWhiteSpace(character.KnowledgeFactor1))
            {
                knows += character.KnowledgeFactor1;
            }
            else
            {
                knows += "k00";
            }
            if (!string.IsNullOrWhiteSpace(character.KnowledgeFactor2))
            {
                knows += character.KnowledgeFactor2;
            }
            else
            {
                knows += "k00";
            }
            knows += ".svg";

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
            string state = "";
            if (character.CharState == UnlockHelper.CharState.Blocked)
            {
                state = "icons/tooltip/state/blocked.svg";
            }
            if (character.CharState == UnlockHelper.CharState.InLine)
            {
                state = "icons/tooltip/state/inline.svg";
            }
            if (character.CharState == UnlockHelper.CharState.InBase)
            {
                state = "icons/tooltip/state/inbase.svg";
            }
            if (character.CharState == UnlockHelper.CharState.InStage)
            {
                stage = _locate.LocateStage(game, character.InStageId);
                state = stage.Icon;
            }

            tooltip.Name = character.Name;
            tooltip.CostAmount = "";
            tooltip.CostIcon = "";
            tooltip.CostName = "";
            tooltip.Description = character.Description + " " + trait.Description;
            tooltip.Info.Add("Estado", state);
            tooltip.Info.Add("Fatores", knows);
            tooltip.Info.Add("Especialidade", specialty.Icon);
            tooltip.Info.Add("Contratos", contracts);
            tooltip.Lore = character.Lore;

            return tooltip;
        }

        // Local
        private TooltipModel BuildLocalHover(string id, GameModel game)
        {
            var tooltip = new TooltipModel();

            var local = _locate.LocateLocal(game, id);
            var stage = _locate.LocateStage(game, local.StageId);

            tooltip.Name = local.Name;
            tooltip.CostAmount = "";
            tooltip.CostIcon = "";
            tooltip.CostName = "";
            tooltip.Description = local.Description;
            tooltip.Info.Add("Região", stage.Icon);
            tooltip.Info.Add("1", " ");
            tooltip.Info.Add("2", " ");
            tooltip.Info.Add("3", " ");
            tooltip.Lore = local.Lore;

            return tooltip;
        }

        // Techs
        private TooltipModel BuildTechHover(string id, GameModel game)
        {
            var tooltip = new TooltipModel();

            var tech = _locate.LocateTech(game, id);

            string know = "";
            switch (tech.PricingId)
            {
                case PricingHelper.PricingId.TechUnlockk01:
                    know = "icons/tooltip/knowledges/cultural.svg";
                    break;
                case PricingHelper.PricingId.TechUnlockk02:
                    know = "icons/tooltip/knowledges/geografico.svg";
                    break;
                case PricingHelper.PricingId.TechUnlockk03:
                    know = "icons/tooltip/knowledges/sobrevivencia.svg";
                    break;
                case PricingHelper.PricingId.TechUnlockk04:
                    know = "icons/tooltip/knowledges/navegacao.svg";
                    break;
                case PricingHelper.PricingId.TechUnlockk05:
                    know = "icons/tooltip/knowledges/caca.svg";
                    break;
            }

            string level = "";
            switch (tech.Level)
            {
                case 1:
                    level = "icons/tooltip/techs/basica.svg";
                    break;
                case 2:
                    level = "icons/tooltip/techs/complexa.svg";
                    break;
                case 3:
                    level = "icons/tooltip/techs/profissional.svg";
                    break;
                case 4:
                    level = "icons/tooltip/techs/especialista.svg";
                    break;
            }

            tooltip.Name = tech.Name;
            tooltip.CostAmount = "";
            tooltip.CostIcon = "";
            tooltip.CostName = "";
            tooltip.Description = tech.Description;
            tooltip.Info.Add("Nível", level);
            tooltip.Info.Add("Conhecimento", know);
            tooltip.Info.Add("1", " ");
            tooltip.Info.Add("2", " ");
            tooltip.Lore = tech.Lore;

            return tooltip;
        }

        // Knowledge
        private TooltipModel BuildKnowledgeHover(string id, GameModel game)
        {
            var tooltip = new TooltipModel();

            var knowledge = _locate.LocateKnowledge(game, id);

            tooltip.Name = knowledge.Name;
            tooltip.CostAmount = "";
            tooltip.CostIcon = "";
            tooltip.CostName = "";
            tooltip.Description = knowledge.Description;
            tooltip.Info.Add("1", " ");
            tooltip.Info.Add("2", " ");
            tooltip.Info.Add("3", " ");
            tooltip.Info.Add("4", " ");
            tooltip.Lore = knowledge.Lore;

            return tooltip;
        }

        // Resources
        private TooltipModel BuildResourcesHover(string id, GameModel game)
        {
            var tooltip = new TooltipModel();

            var resource = _locate.LocateResource(game, id);

            var rsInfo = _resources.GetResourceInfo(game, id);
            string gain = NumbersHelper.Padronize(rsInfo.rsRegen);

            string cap = NumbersHelper.Padronize(rsInfo.rsCap);

            tooltip.Name = resource.Name;
            tooltip.CostAmount = "";
            tooltip.CostIcon = "";
            tooltip.CostName = "";
            tooltip.Description = resource.Description;
            tooltip.Info.Add("Ganho", gain);
            tooltip.Info.Add("Capacidade", cap);
            tooltip.Info.Add("1", " ");
            tooltip.Info.Add("2", " ");
            tooltip.Lore = resource.Lore;

            return tooltip;
        }

        // Coins
        private TooltipModel BuildCoinsHover(string id, GameModel game)
        {
            var tooltip = new TooltipModel();

            var coin = _locate.LocateCoin(game, id);
            var stage = _locate.LocateStage(game, game.SelectedStageId);
            var expansion = _locate.LocateExpansion(game, game.CurrentExpansionId);

            var stageGain = _contract.GetStageContractsPerSecond(game, stage.Id);
            stageGain.TryGetValue(id, out var gainS);
            string gainStage = NumbersHelper.Padronize(gainS) + "/s";

            var gameGain = _contract.GetGameContractsPerSecond(game);
            gameGain.TryGetValue(id, out var gainT);
            string gainTotal = NumbersHelper.Padronize(gainT) + "/s";

            stage.ExpeditionStats.Coins.TryGetValue(id, out var amountS);
            var amountStage = NumbersHelper.Padronize(amountS);

            expansion.ExpansionStats.Coins.TryGetValue(id, out var amountT);
            var amountTotal = NumbersHelper.Padronize(amountT);

            tooltip.Name = coin.Name;
            tooltip.CostAmount = "";
            tooltip.CostIcon = "";
            tooltip.CostName = "";
            tooltip.Description = coin.Description;
            tooltip.Info.Add("Na Região", amountStage);
            tooltip.Info.Add("Ganho Região", gainStage);
            tooltip.Info.Add("No Total", amountTotal);
            tooltip.Info.Add("Ganho Total", gainTotal);
            tooltip.Lore = coin.Lore;

            return tooltip;
        }

        // Stage
        private TooltipModel BuildStageHover(string id, GameModel game)
        {
            var tooltip = new TooltipModel();

            var stage = _locate.LocateStage(game, id);

            tooltip.Name = stage.Name;
            tooltip.CostAmount = "";
            tooltip.CostIcon = "";
            tooltip.CostName = "";
            tooltip.Description = stage.Description;
            tooltip.Info.Add("1", " ");
            tooltip.Info.Add("2", " ");
            tooltip.Info.Add("3", " ");
            tooltip.Info.Add("4", " ");
            tooltip.Lore = stage.Lore;

            return tooltip;
        }

        // Expedition
        private TooltipModel BuildExpeditionHover(string id, GameModel game)
        {
            var stage = _locate.LocateStage(game, id);

            var tooltip = new TooltipModel();

            int countLine = 0;
            foreach (var characters in _game.CurrentGame.Characters)
            {
                if (characters.Value.CharState == CharState.InLine) countLine++;
            }

            int partyCap = _expedition.GetPartyCap(stage);
            
            string partySize = countLine + " / " + partyCap;

            tooltip.Name = "Expedição";
            tooltip.CostAmount = "";
            tooltip.CostIcon = "";
            tooltip.CostName = "";
            tooltip.Description = "Encerra ou Inicia uma Expedição.";
            tooltip.Info.Add("Membros", partySize);
            tooltip.Info.Add("1", " ");
            tooltip.Info.Add("2", " ");
            tooltip.Info.Add("3", " ");
            tooltip.Lore = "Toda aventura precisa terminar";

            return tooltip;
        }
    }
}
