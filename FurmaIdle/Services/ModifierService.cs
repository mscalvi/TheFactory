using FurmaIdle.Helpers;
using FurmaIdle.Models;
using static FurmaIdle.Helpers.EffectHelper;
using static FurmaIdle.Helpers.ItemHelper;
using static FurmaIdle.Helpers.UnlockHelper;

namespace FurmaIdle.Services
{
    public interface IModifierService 
    {
        (double AddMod, double MultMod) GetModifiers(ItemType type, string itemId, string stageId, EffectSupertype group);
    }

    public sealed class ModifierService : IModifierService
    {
        private readonly ICurrentGameService _game;
        private readonly ILocateService _locate;
        private readonly IUiLogService _log;

        public ModifierService(ICurrentGameService Game, IUiLogService Log, ILocateService Locate)
        {
            _game = Game;
            _locate = Locate;
            _log = Log;
        }
        public (double AddMod, double MultMod) GetModifiers(ItemType type, string itemId, string stageId, EffectSupertype group)
        {
            var game = _game.CurrentGame;

            double AddMod = 0;
            double MultMod = 1;

            var stage = _locate.LocateStage(game, game.SelectedStageId);
            var expansion = _locate.LocateExpansion(game, game.CurrentExpansionId);

            var expedition = new ExpeditionModel();

            if(type != ItemType.Expedition)
            {
                expedition = _locate.LocateExpedition(game, stage.Id);
            } else
            {
                expedition = _locate.LocateExpedition(game, itemId);
            }

            switch (group)
            {
                #region Cost
                case EffectSupertype.Cost:
                    switch (type)
                    {
                        case ItemType.Tech:
                            var tech = _locate.LocateTech(game, itemId);
                            foreach (var modifier in tech.Modifiers)
                            {
                                if (modifier.Type == EffectType.CharacterCost)
                                {
                                    if (modifier.Operation == EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value;
                                    }
                                    if (modifier.Operation == EffectOperation.Multiplicative)
                                    {
                                        MultMod *= modifier.Value;
                                    }
                                }
                            }
                            break;

                        case ItemType.Character:
                            var character = _locate.LocateCharacter(game, itemId);
                            foreach (var modifier in character.Modifiers)
                            {
                                if (modifier.Type == EffectType.CharacterCost)
                                {
                                    if (modifier.Operation == EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value;
                                    }
                                    if (modifier.Operation == EffectOperation.Multiplicative)
                                    {
                                        MultMod *= modifier.Value;
                                    }
                                }
                            }
                            break;

                        case ItemType.Contract:
                            var contract = _locate.LocateContract(game, itemId);
                            foreach (var modifier in contract.Modifiers)
                            {
                                if (modifier.Type == EffectType.ContractCost)
                                {
                                    if (modifier.Operation == EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value;
                                    }
                                    if (modifier.Operation == EffectOperation.Multiplicative)
                                    {
                                        MultMod *= modifier.Value;
                                    }
                                }
                            }
                            break;

                        case ItemType.Upgrade:
                            var upgrade = _locate.LocateUpgrade(game, itemId);
                            string upgradeKind = upgrade.Id.Length >= 2
                                ? upgrade.Id.Substring(0, 2)
                                : upgrade.Id;

                            if (upgrade.EffectOp == EffectOperation.Unlock)
                            {
                                switch (upgradeKind)
                                {
                                    case "uk":
                                        var knowledgeupgrade = _locate.LocateKnowledge(game, upgrade.TargetId);
                                        foreach (var modifier in knowledgeupgrade.Modifiers)
                                        {
                                            if (modifier.Type == EffectType.KnowledgeCost)
                                            {
                                                if (modifier.Operation == EffectOperation.Additive)
                                                {
                                                    AddMod += modifier.Value;
                                                }
                                                if (modifier.Operation == EffectOperation.Multiplicative)
                                                {
                                                    MultMod *= modifier.Value;
                                                }
                                            }
                                        }
                                        break;
                                    case "up":
                                        var characterupgrade = _locate.LocateCharacter(game, upgrade.TargetId);
                                        foreach (var modifier in characterupgrade.Modifiers)
                                        {
                                            if (modifier.Type == EffectType.CharacterCost)
                                            {
                                                if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                                                {
                                                    AddMod += modifier.Value;
                                                }
                                                if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                                                {
                                                    MultMod *= modifier.Value;
                                                }
                                            }
                                        }
                                        break;
                                }
                            }

                            foreach (var modifier in upgrade.Modifiers)
                            {
                                if (modifier.Type == EffectType.UpgradeCost)
                                {
                                    if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value;
                                    }
                                    if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        MultMod *= modifier.Value;
                                    }
                                }

                            }
                            break;

                        case ItemType.Expedition:
                            foreach (var modifier in expedition.Modifiers)
                            {
                                if (modifier.Type == EffectType.ExpeditionCost)
                                {
                                    if (modifier.Operation == EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value;
                                    }
                                    if (modifier.Operation == EffectOperation.Multiplicative)
                                    {
                                        MultMod *= modifier.Value;
                                    }
                                }
                            }
                            break;

                        case ItemType.Expansion:
                            foreach (var modifier in expansion.Modifiers)
                            {
                                if (modifier.Type == EffectType.ExpansionCost)
                                {
                                    if (modifier.Operation == EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value;
                                    }
                                    if (modifier.Operation == EffectOperation.Multiplicative)
                                    {
                                        MultMod *= modifier.Value;
                                    }
                                }
                            }
                            break;

                        case ItemType.Specialty:
                            var specialty = _locate.LocateSpecialty(game, itemId);

                            foreach (var characterTarget in game.Characters)
                            {
                                if (characterTarget.Value.SpecialtyId == specialty.Id && characterTarget.Value.State == State.Unlocked)
                                {
                                    foreach (var modifier in characterTarget.Value.Modifiers)
                                    {
                                        if (modifier.Type == EffectType.SpecialtyCost)
                                        {
                                            if (modifier.Operation == EffectOperation.Additive)
                                            {
                                                AddMod += modifier.Value;
                                            }
                                            if (modifier.Operation == EffectOperation.Multiplicative)
                                            {
                                                MultMod *= modifier.Value;
                                            }
                                        }
                                    }
                                }
                            }

                            foreach (var modifier in specialty.Modifiers)
                            {
                                if (modifier.Type == EffectType.SpecialtyCost)
                                {
                                    if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value;
                                    }
                                    if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        MultMod *= modifier.Value;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                #endregion

                #region Gain
                case EffectSupertype.Gain:
                    switch (type)
                    {
                        case ItemType.Click:
                            var click = _locate.LocateClick(game, itemId);

                            foreach (var modifier in click.Modifiers)
                            {
                                if (modifier.Type == EffectHelper.EffectType.ClickGain)
                                {
                                    if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value;
                                    }
                                    if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        MultMod *= modifier.Value;
                                    }
                                }

                                if (modifier.Type == EffectHelper.EffectType.ClickGainCent)
                                {
                                    if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value * expedition.CurrentCoinPerSec;
                                    }
                                    if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        MultMod *= Math.Pow(modifier.Value, 1);
                                    }
                                }
                            }

                            foreach (var modifier in stage.Modifiers)
                            {
                                if (modifier.Type == EffectHelper.EffectType.ClickGain)
                                {
                                    if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value;
                                    }
                                    if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                                    {
                                        MultMod *= modifier.Value;
                                    }
                                }
                            }
                            break;

                        case ItemType.Coin:
                            var coin = _locate.LocateCoin(game, itemId);
                            foreach (var modifier in coin.Modifiers)
                            {
                                if (modifier.Type == EffectType.CoinGain)
                                {
                                    if (modifier.Operation == EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value;
                                    }
                                    if (modifier.Operation == EffectOperation.Multiplicative)
                                    {
                                        MultMod *= modifier.Value;
                                    }
                                }
                            }
                            break;

                        case ItemType.Contract:
                            var contract = _locate.LocateContract(game, itemId);
                            foreach (var modifier in contract.Modifiers)
                            {
                                if (modifier.Type == EffectType.ContractGain)
                                {
                                    if (modifier.Operation == EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value;
                                    }
                                    if (modifier.Operation == EffectOperation.Multiplicative)
                                    {
                                        MultMod *= modifier.Value;
                                    }
                                }
                                if (modifier.Type == EffectType.ContractGainPerTech)
                                {
                                    if (modifier.Operation == EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value * game.GameStats.TechUnlocked;
                                    }
                                    if (modifier.Operation == EffectOperation.Multiplicative)
                                    {
                                        MultMod *= Math.Pow(modifier.Value, game.GameStats.TechUnlocked);
                                    }
                                }
                                if (modifier.Type == EffectType.ContractGainPerLocal)
                                {
                                    if (modifier.Operation == EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value * game.GameStats.LocalsUnlocked;
                                    }
                                    if (modifier.Operation == EffectOperation.Multiplicative)
                                    {
                                        MultMod *= Math.Pow(modifier.Value, game.GameStats.LocalsUnlocked);
                                    }
                                }
                            }
                            break;

                        case ItemType.Knowledge:
                            var knowledge = _locate.LocateKnowledge(game, itemId);
                            foreach (var modifier in knowledge.Modifiers)
                            {
                                if (modifier.Type == EffectType.KnowledgeGain)
                                {
                                    if (modifier.Operation == EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value;
                                    }
                                    if (modifier.Operation == EffectOperation.Multiplicative)
                                    {
                                        MultMod *= modifier.Value;
                                    }
                                }
                                if (modifier.Type == EffectType.KnowledgeGainPerTech)
                                {
                                    if (modifier.Operation == EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value * game.GameStats.TechUnlocked;
                                    }
                                    if (modifier.Operation == EffectOperation.Multiplicative)
                                    {
                                        MultMod *= Math.Pow(modifier.Value, game.GameStats.TechUnlocked);
                                    }
                                }
                                if (modifier.Type == EffectType.KnowledgeGainPerLocal)
                                {
                                    if (modifier.Operation == EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value * game.GameStats.LocalsUnlocked;
                                    }
                                    if (modifier.Operation == EffectOperation.Multiplicative)
                                    {
                                        MultMod *= Math.Pow(modifier.Value, game.GameStats.LocalsUnlocked);
                                    }
                                }
                            }
                            break;

                        case ItemType.Resource:
                            var resource = _locate.LocateResource(game, itemId);
                            foreach (var modifier in resource.Modifiers)
                            {
                                if (modifier.Type == EffectType.ResourceGain)
                                {
                                    if (modifier.Operation == EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value;
                                    }
                                    if (modifier.Operation == EffectOperation.Multiplicative)
                                    {
                                        MultMod *= modifier.Value;
                                    }
                                }
                                if (modifier.Type == EffectType.ResourceGainPerTech)
                                {
                                    if (modifier.Operation == EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value * game.GameStats.TechUnlocked;
                                    }
                                    if (modifier.Operation == EffectOperation.Multiplicative)
                                    {
                                        MultMod *= Math.Pow(modifier.Value, game.GameStats.TechUnlocked);
                                    }
                                }
                                if (modifier.Type == EffectType.ResourceGainPerLocal)
                                {
                                    if (modifier.Operation == EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value * game.GameStats.LocalsUnlocked;
                                    }
                                    if (modifier.Operation == EffectOperation.Multiplicative)
                                    {
                                        MultMod *= Math.Pow(modifier.Value, game.GameStats.LocalsUnlocked);
                                    }
                                }
                            }
                            break;
                    }
                    break;
                #endregion

                #region Time
                case EffectSupertype.Time:
                    switch (type)
                    {
                        case ItemType.Contract:
                            var contract = _locate.LocateContract(game, itemId);
                            foreach (var modifier in contract.Modifiers)
                            {
                                if (modifier.Type == EffectType.ContractTime)
                                {
                                    if (modifier.Operation == EffectOperation.Additive)
                                    {
                                        AddMod += modifier.Value;
                                    }
                                    if (modifier.Operation == EffectOperation.Multiplicative)
                                    {
                                        MultMod *= modifier.Value;
                                    }
                                }
                            }
                            break;
                    }
                    break;
                #endregion

                #region Offline
                case EffectSupertype.Offline:
                    MultMod *= 0.1;
                    break;
                #endregion

                #region Cap
                // ContractLevel e ContractCap estão no ContractService
                #endregion

                #region Unlock
                #endregion
            }

            return (AddMod, MultMod);
        }
    }
}
