using FurmaIdle.Helpers;
using FurmaIdle.Models;
using FurmaIdle.Data;

namespace FurmaIdle.Services
{
    public interface IUnlockService
    {
        Task UnlockCharacter(string characterId);
        Task UnlockCoin(string coinId);
        Task UnlockContract(string contractId);
        Task UnlockExpansion(string expansionId);
        Task UnlockKnowledge(string knowledgeId);
        Task UnlockLocal(string localId);
        Task UnlockStage(string stageId);
        Task UnlockTech(string techId);
        Task UnlockResource(string resourceId);
        Task UnlockUpgrade(string upgradeId);
    }

    public sealed class UnlockService : IUnlockService
    {
        private readonly ICurrentGameService _game;
        private readonly ILocateService _locate;

        public UnlockService(ICurrentGameService game, ILocateService locate)
        {
            _game = game;
            _locate = locate;
        }

        #region Character Unlock
        public async Task UnlockCharacter(string characterId)
        {
            await _game.Mutate(game =>
            {
                var character = _locate.LocateCharacter(characterId);

                foreach (var upgradeId in UpgradeData.ShowOrder)
                {
                    var up = _locate.LocateUpgrade(upgradeId);
                    if (string.Equals(up.UnlockId, character.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        up.State = UnlockHelper.State.Available;
                    }
                }

                character.State = UnlockHelper.State.Unlocked;
            });
        }
        #endregion

        #region Coin Unlock
        public async Task UnlockCoin(string coinId)
        {
            await _game.Mutate(game =>
            {
                var coin = _locate.LocateCoin(coinId);

                coin.State = UnlockHelper.State.Unlocked;
            });
        }
        #endregion

        #region Contract Unlock
        public async Task UnlockContract(string contractId)
        {
            await _game.Mutate(game =>
            {
                var contract = _locate.LocateContract(contractId);

                foreach (var upgradeId in UpgradeData.ShowOrder)
                {
                    var up = _locate.LocateUpgrade(upgradeId);
                    if (string.Equals(up.UnlockId, contract.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        up.State = UnlockHelper.State.Available;
                    }
                }

                contract.State = UnlockHelper.State.Unlocked;
            });
        }

        #endregion

        #region Expansion Unlock
        public async Task UnlockExpansion(string expansionId)
        {
            await _game.Mutate(game =>
            {
                var expansion = _locate.LocateExpansion(expansionId);

                foreach (var upgradeId in UpgradeData.ShowOrder)
                {
                    var up = _locate.LocateUpgrade(upgradeId);
                    if (string.Equals(up.UnlockId, expansion.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        up.State = UnlockHelper.State.Available;
                    }
                }

                expansion.State = UnlockHelper.State.Unlocked;
                Console.WriteLine($"[Unlock] Expansion: {expansion.State}");
            });

        }
        #endregion

        #region Knowledge Unlock
        public async Task UnlockKnowledge(string knowledgeId)
        {
            await _game.Mutate(game =>
            {
                var know = _locate.LocateKnowledge(knowledgeId);

                know.State = UnlockHelper.State.Unlocked;
            });
        }
        #endregion

        #region Local Unlock
        public async Task UnlockLocal(string localId)
        {
            await _game.Mutate(game =>
            {
                var local = _locate.LocateLocal(localId);

                foreach (var techId in TechData.ShowOrder)
                {
                    var tech = _locate.LocateTech(techId);
                    if (string.Equals(tech.UnlockId, local.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        tech.State = UnlockHelper.State.Available;
                    }
                }

                foreach (var expansionId in ExpansionData.ShowOrder)
                {
                    var expansion = _locate.LocateExpansion(expansionId);
                    if (string.Equals(expansion.UnlockId, local.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        expansion.State = UnlockHelper.State.Available;
                    }
                }

                foreach (var upgradeId in UpgradeData.ShowOrder)
                {
                    var up = _locate.LocateUpgrade(upgradeId);
                    if (string.Equals(up.UnlockId, local.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        up.State = UnlockHelper.State.Available;
                    }
                }

                local.State = UnlockHelper.State.Unlocked;
            });
        }
        #endregion

        #region Stage Unlock
        public async Task UnlockStage(string stageId)
        {
            await _game.Mutate(game =>
            {
                var stage = _locate.LocateStage(stageId);

                foreach (var coinId in CoinsData.ShowOrder)
                {
                    var coin = _locate.LocateCoin(coinId);
                    if (string.Equals(coin.UnlockId, stage.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        if(coin.Id == "m01")
                        {
                            coin.State = UnlockHelper.State.Unlocked;
                        } else
                        {
                            coin.State = UnlockHelper.State.Available;
                        }
                    }
                }

                foreach (var upgradeId in UpgradeData.ShowOrder)
                {
                    var up = _locate.LocateUpgrade(upgradeId);
                    if (string.Equals(up.UnlockId, stage.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        up.State = UnlockHelper.State.Available;
                    }
                }

                stage.State = UnlockHelper.State.Unlocked;

                Console.WriteLine($"[Unlock] Stage {stage.State}");
            });
        }
        #endregion

        #region Tech Unlock
        public async Task UnlockTech(string techId)
        {
            await _game.Mutate(game =>
            {
                var tech = _locate.LocateTech(techId);

                foreach (var upgradeId in UpgradeData.ShowOrder)
                {
                    var up = _locate.LocateUpgrade(upgradeId);
                    if (string.Equals(up.UnlockId, tech.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        up.State = UnlockHelper.State.Available;
                    }
                }

                tech.State = UnlockHelper.State.Unlocked;
            });
        }
        #endregion

        #region Resource Unlock
        public async Task UnlockResource(string resourceId)
        {
            await _game.Mutate(game =>
            {
                var resource = _locate.LocateResource(resourceId);

                resource.State = UnlockHelper.State.Unlocked;
            });
        }
        #endregion

        #region Upgrade Unlock
        public async Task UnlockUpgrade(string upgradeId)
        {
            await _game.Mutate(game =>
            {
                var up = _locate.LocateUpgrade(upgradeId);

                switch (up.EffectType)
                {
                    // Unlocks
                    case EffectHelper.EffectType.ContractUnlock:
                        UnlockContract(up.TargetId);
                        break;
                    case EffectHelper.EffectType.KnowledgeUnlock:
                        UnlockKnowledge(up.TargetId);
                        break;
                    case EffectHelper.EffectType.LocalUnlock:
                        UnlockLocal(up.TargetId);
                        break;
                    case EffectHelper.EffectType.CharacterUnlock:
                        UnlockCharacter(up.TargetId);
                        break;
                    case EffectHelper.EffectType.ResourceUnlock:
                        UnlockResource(up.TargetId);
                        break;
                    case EffectHelper.EffectType.StageUnlock:
                        UnlockStage(up.TargetId);
                        break;

                    // Outros
                    case EffectHelper.EffectType.ContractGain:
                        break;
                    case EffectHelper.EffectType.ContractTime:
                        break;
                    case EffectHelper.EffectType.ContractCost:
                        break;
                    case EffectHelper.EffectType.ClickGain:
                        break;
                    case EffectHelper.EffectType.ContractCapUnlock:
                        break;
                    case EffectHelper.EffectType.ContractLevelUnlock:
                        break;
                    case EffectHelper.EffectType.ResourceGain:
                        break;

                    default:
                        break;

                }

                up.State = UnlockHelper.State.Unlocked;
            });
        }
        #endregion
    }
}
