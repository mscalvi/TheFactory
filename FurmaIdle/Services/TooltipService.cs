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

        public TooltipModel? Current { get; private set; }
        public event Action? Changed;

        public TooltipService(ICurrentGameService game, ILocateService locate, ICostService cost, IContractsService contract, IExpeditionService expedition)
        {
            _game = game;
            _locate = locate;
            _cost = cost;
            _contract = contract;
            _expedition = expedition;
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
            var stageIn = _locate.LocateStage(game, game.SelectedStageId);

            string nome = upgrade.Name;
            var cost = _cost.ComputeCost(ItemHelper.ItemType.Upgrade, upgrade.Id, stageIn.Id);

            string upIdType = upgrade.Id.Length >= 2
                ? upgrade.Id.Substring(0, 2)
                : upgrade.Id;

            var costCoin = new CoinModel();
            var costResource = new ResourceModel();
            var costKnowledge = new KnowledgeModel();

            string custo = NumbersHelper.Padronize(cost.costValue);

            if (upIdType == "xx")
            {
                costResource = _locate.LocateResource(game, cost.costId);
                nome += " - " + custo + " " + costResource.Name;
            }
            else if (upIdType == "uh")
            {
                costKnowledge = _locate.LocateKnowledge(game, cost.costId);
                nome += " - " + custo + " " + costKnowledge.Name;
            }
            else
            {
                costCoin = _locate.LocateCoin(game, cost.costId);
                nome += " - " + custo + " " + costCoin.Name;
            }

            string tipo = "";
            string target = "";
            string valor = "";
            string operation = "";
            string intro = "";

            switch (upIdType)
            {
                // Unlocks
                case "uu": // Contracts
                    tipo = "Desbloqueio";
                    var contract = _locate.LocateContract(game, upgrade.TargetId);
                    target = "para " + contract.Name;
                    operation = "Desbloqueio de Contrato ";
                    break;
                case "uk": // Knowledge
                    tipo = "Desbloqueio";
                    var knowledge = _locate.LocateKnowledge(game, upgrade.TargetId);
                    target = "do tipo " + knowledge.Name;
                    operation = "Desbloqueio de Conhecimento ";
                    break;
                case "up": // Characters
                    tipo = "Desbloqueio";
                    var character = _locate.LocateCharacter(game, upgrade.TargetId);
                    target = character.Name;
                    operation = "Desbloqueio do Personagem ";
                    break;
                case "ul": // Locals
                    tipo = "Desbloqueio";
                    var local = _locate.LocateLocal(game, upgrade.TargetId);
                    target = local.Name;
                    operation = "Desbloqueio do Local ";
                    break;
                case "us": // Stages
                    tipo = "Reset";
                    var stage = _locate.LocateStage(game, upgrade.TargetId);
                    target = stage.Name;
                    operation = "Desbloqueio da Região ";
                    break;
                case "uh": // Techs
                    tipo = "Desbloqueio";
                    var tech = _locate.LocateTech(game, upgrade.TargetId);
                    target = tech.Name;
                    operation = "Desbloqueio para ";
                    break;
                case "ur": // Resources
                    tipo = "Desbloqueio";
                    var resource = _locate.LocateResource(game, upgrade.TargetId);
                    target = "do tipo " + resource.Name;
                    operation = "Desbloqueio de Recurso ";
                    break;

                // Unlocks Diferentes
                case "ue": // Expansions
                    var expansion = _locate.LocateExpansion(game, upgrade.TargetId);
                    tipo = "Reset";
                    target = expansion.Name;
                    operation = "Inicia o próximo passo: ";
                    break;
                case "ua": // Party Size
                    tipo = "Desbloqueio";
                    target = stageIn.Name;
                    operation = "Aumenta o limite de membros ativos da Guilda em ";
                    break;

                // Expeditions
                case "ui": // Click
                    tipo = "Melhoria";
                    target = stageIn.Name;
                    intro = "Aumenta o ganho do Click em ";
                    valor = upgrade.EffectValue.ToString("N2");
                    break;
                case "uc": // Contracts Modifiers
                    tipo = "Melhoria";

                    if (upgrade.TargetId == "aContracts")
                    {
                        target = "todos os Contratos";
                    } else
                    {
                        var contractMod = _locate.LocateContract(game, upgrade.TargetId);
                        target = contractMod.Name;
                    }

                    if(upgrade.EffectSupertype == EffectHelper.EffectSupertype.Cost)
                    {
                        intro = "Diminui o Custo de ";
                    }
                    if (upgrade.EffectSupertype == EffectHelper.EffectSupertype.Gain)
                    {
                        intro = "Aumenta o Ganho de ";
                    }
                    if (upgrade.EffectSupertype == EffectHelper.EffectSupertype.Time)
                    {
                        intro = "Diminui o Tempo de ";
                    }

                    valor = upgrade.EffectValue.ToString("N2");
                    break;

                // Tech Upgrades
                case "ut": // Target é c ou r
                    tipo = "Melhoria";

                    if (upgrade.TargetId != "aContracts")
                    {
                        if (upgrade.TargetId.StartsWith('c'))
                        {
                            var contractMod = _locate.LocateContract(game, upgrade.TargetId);
                            target = contractMod.Name;
                        }
                        if (upgrade.TargetId.StartsWith('r'))
                        {
                            var resourceMod = _locate.LocateResource(game, upgrade.TargetId);
                            target = resourceMod.Name;
                        }
                    }
                    else
                    {
                        target = "todos os Contratos";
                    }

                    if (upgrade.EffectSupertype == EffectHelper.EffectSupertype.Cost)
                    {
                        intro = "Diminui o Custo de ";
                    }
                    if (upgrade.EffectSupertype == EffectHelper.EffectSupertype.Gain)
                    {
                        intro = "Aumenta o Ganho de ";
                    }
                    if (upgrade.EffectSupertype == EffectHelper.EffectSupertype.Time)
                    {
                        intro = "Diminui o Tempo de ";
                    }

                    valor = upgrade.EffectValue.ToString("N2");
                    break;

                // Expansions
                case "um": // ContractCap
                    tipo = "Melhoria";
                    intro = "Aumenta o limite de Contratos ";
                    target = "de todos os Personagens";
                    valor = upgrade.EffectValue.ToString("N2");
                    break;
                case "ub": // Contract Level Unlock
                    tipo = "Melhoria";
                    operation = "Aumenta o nível máximo dos Contratos em ";
                    target = stageIn.Name;
                    break;
                case "ux": // Target pode ser i, r, aResources, aKnowledges, aContracts, 
                    tipo = "Melhoria";
                    if (upgrade.TargetId != "aContracts" && upgrade.TargetId != "aKnowledges" && upgrade.TargetId != "aResources")
                    {
                        if (upgrade.TargetId.StartsWith("i"))
                        {
                            var clickMod = _locate.LocateClick(game, upgrade.TargetId);
                            var stageMod = _locate.LocateStage(game, clickMod.StageId);
                            target = "Click em " + stageMod.Name;
                        }
                        if (upgrade.TargetId.StartsWith('r'))
                        {
                            var resourceMod = _locate.LocateResource(game, upgrade.TargetId);
                            target = resourceMod.Name;
                        }
                    }
                    else
                    {
                        if (upgrade.TargetId == "aContracts")
                        {
                            target = "todos os Contratos";
                        }
                        if (upgrade.TargetId == "aKnowledges")
                        {
                            target = "todos os Conhecimentos";
                        }
                        if (upgrade.TargetId == "aResources")
                        {
                            target = "todos os Recursos";
                        }
                    }

                    if (upgrade.EffectSupertype == EffectHelper.EffectSupertype.Cost)
                    {
                        intro = "Diminui o Custo de ";
                    }
                    if (upgrade.EffectSupertype == EffectHelper.EffectSupertype.Gain)
                    {
                        intro = "Aumenta o Ganho de ";
                    }
                    if (upgrade.EffectSupertype == EffectHelper.EffectSupertype.Time)
                    {
                        intro = "Diminui o Tempo de ";
                    }

                    valor = upgrade.EffectValue.ToString("N3");
                    break;
            }

            if(operation == "")
            {
                if (upgrade.EffectOp == EffectHelper.EffectOperation.Additive)
                {
                    operation = "Base +";
                }
                if (upgrade.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                {
                    operation = "Total x";
                }
            }

            // alvo
            // operação
            // valor?

            string function = "";
            if (valor == "")
            {
                function = operation + target;
            } else
            {
                function = intro + target + " -> " + operation + valor;
            }

            var modifiers = upgrade.Modifiers.Count;

            tooltip.Type = tipo;
            tooltip.Name = nome;
            tooltip.Lore = upgrade.Lore;
            tooltip.Info.Add("Função", function);
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
            string custo = NumbersHelper.Padronize(cost.costValue);

            nome += " - " + custo + " " + coin.Name;


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
            perSec = baseInfo.CoinsPerCycle / baseInfo.SecondsPerCycle;
            geraBase = baseInfo.CoinsPerCycle.ToString("N2") + " " + coin.Name + " a cada " + baseInfo.SecondsPerCycle.ToString("N2") + "s -> " + perSec.ToString("N2") + " " + coin.Name + "/s";

            string geraAtual = "";
            var actualInfo = _contract.GetContractInfo(contract, stage);
            perSec = actualInfo.CoinsPerCycle / actualInfo.SecondsPerCycle;
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

            var local = _locate.LocateLocal(game, id);

            tooltip.Type = "Lugar";
            tooltip.Name = local.Name;
            tooltip.Lore = local.Lore;
            tooltip.Info.Add("Descrição", local.Description);

            return tooltip;
        }

        // Techs
        private TooltipModel BuildTechHover(string id, GameModel game)
        {
            var tooltip = new TooltipModel();

            var tech = _locate.LocateTech(game, id);

            string know = "";

            string techKnow = tech.Id.Length >= 3
                ? tech.Id.Substring(0, 2)
                : tech.Id;

            switch (techKnow)
            {
                case "t01":
                    know = "Cultural";
                    break;
                case "t02":
                    know = "Geográfico";
                    break;
                case "t03":
                    know = "Sobrevivência";
                    break;
                case "t04":
                    know = "Navegação";
                    break;
                case "t05":
                    know = "Caça";
                    break;
            }

            tooltip.Type = "Pesquisa";
            tooltip.Name = tech.Name;
            tooltip.Lore = tech.Lore;
            tooltip.Info.Add("Nível", tech.Level.ToString());
            tooltip.Info.Add("Conhecimento", know);
            tooltip.Info.Add("Descrição", tech.Description);

            return tooltip;
        }

        // Knowledge
        private TooltipModel BuildKnowledgeHover(string id, GameModel game)
        {
            var tooltip = new TooltipModel();

            var knowledge = _locate.LocateKnowledge(game, id);

            tooltip.Type = "Conhecimento";
            tooltip.Name = knowledge.Name;
            tooltip.Lore = knowledge.Lore;
            tooltip.Info.Add("Descrição", knowledge.Description);

            return tooltip;
        }

        // Specialty
        private TooltipModel BuildSpecialtyHover(string id, GameModel game)
        {
            var tooltip = new TooltipModel();

            var specialty = _locate.LocateSpecialty(game, id);
            var stageIn = _locate.LocateStage(game, game.SelectedStageId);

            string nome = specialty.Name;
            var cost = _cost.ComputeCost(ItemHelper.ItemType.Specialty, specialty.Id, stageIn.Id);

            var costResource = new ResourceModel();

            costResource = _locate.LocateResource(game, cost.costId);
            string custo = NumbersHelper.Padronize(cost.costValue);
            nome += " - " + custo + " " + costResource.Name;

            string target = "";
            string valor = "";
            string operation = "";
            string intro = "";

            string specTarget = specialty.TargetId.Length >= 2
                ? specialty.TargetId.Substring(0, 1)
                : specialty.TargetId;

            switch (specTarget)
            {
                case "i": // Click
                    target = stageIn.Name;
                    intro = "Aumenta o ganho do Click em ";
                    valor = specialty.EffectValue.ToString("N2");
                    break;
                case "c": // Contracts
                    var contract = _locate.LocateContract(game, specialty.TargetId);
                    target = contract.Name;

                    if (specialty.EffectSupertype == EffectHelper.EffectSupertype.Cost)
                    {
                        intro = "Diminui o Custo de ";
                    }
                    if (specialty.EffectSupertype == EffectHelper.EffectSupertype.Gain)
                    {
                        intro = "Aumenta o Ganho de ";
                    }
                    if (specialty.EffectSupertype == EffectHelper.EffectSupertype.Time)
                    {
                        intro = "Diminui o Tempo de ";
                    }

                    valor = specialty.EffectValue.ToString("N2");
                    break;
                case "a":
                    if (specialty.TargetId == "aContracts")
                    {
                        target = "todos os Contratos";
                    }
                    if (specialty.TargetId == "aSpecialties")
                    {
                        target = "todas as Especialidades";
                    }
                    if (specialty.TargetId == "aResources")
                    {
                        target = "todos os Recursos";
                    }


                    if (specialty.EffectSupertype == EffectHelper.EffectSupertype.Cost)
                    {
                        intro = "Diminui o Custo de ";
                    }
                    if (specialty.EffectSupertype == EffectHelper.EffectSupertype.Gain)
                    {
                        intro = "Aumenta o Ganho de ";
                    }
                    if (specialty.EffectSupertype == EffectHelper.EffectSupertype.Time)
                    {
                        intro = "Diminui o Tempo de ";
                    }

                    valor = specialty.EffectValue.ToString("N2");
                    break;
            }

            if (operation == "")
            {
                if (specialty.EffectOp == EffectHelper.EffectOperation.Additive)
                {
                    operation = "Base +";
                }
                if (specialty.EffectOp == EffectHelper.EffectOperation.Multiplicative)
                {
                    operation = "Total x";
                }
            }

            string function = intro + target + " -> " + operation + valor;

            var modifiers = specialty.Modifiers.Count;

            tooltip.Type = "Especialidade";
            tooltip.Name = nome;
            tooltip.Lore = specialty.Lore;
            tooltip.Info.Add("Função", function);
            tooltip.Info.Add("Modificadores Ativos", modifiers.ToString());

            return tooltip;
        }

        // Resources
        private TooltipModel BuildResourcesHover(string id, GameModel game)
        {
            var tooltip = new TooltipModel();

            var resource = _locate.LocateResource(game, id);

            tooltip.Type = "Recurso";
            tooltip.Name = resource.Name;
            tooltip.Lore = resource.Lore;
            tooltip.Info.Add("Descrição", resource.Description);

            return tooltip;
        }

        // Coins
        private TooltipModel BuildCoinsHover(string id, GameModel game)
        {
            var tooltip = new TooltipModel();

            var coin = _locate.LocateCoin(game, id);

            tooltip.Type = "Moeda";
            tooltip.Name = coin.Name;
            tooltip.Lore = coin.Lore;
            tooltip.Info.Add("Descrição", coin.Description);

            return tooltip;
        }

        // Stage
        private TooltipModel BuildStageHover(string id, GameModel game)
        {
            var tooltip = new TooltipModel();

            var stage = _locate.LocateStage(game, id);

            tooltip.Type = "Região";
            tooltip.Name = stage.Name;
            tooltip.Lore = stage.Lore;
            tooltip.Info.Add("Descrição", stage.Description);

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

            tooltip.Type = "Reset";
            tooltip.Name = "Expedição";
            tooltip.Info.Add("Membros", partySize);
            tooltip.Lore = "Toda aventura precisa terminar.";

            return tooltip;
        }
    }
}
