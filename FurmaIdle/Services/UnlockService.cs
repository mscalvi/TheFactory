using FurmaIdle.Data;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Diagnostics.Contracts;

namespace FurmaIdle.Services
{
    public interface IUnlockService
    {
        Task UnlockInitialState();
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
        private readonly IUpgradeService _upgrades;

        public UnlockService(ICurrentGameService game, ILocateService locate, IUpgradeService upgrades)
        {
            _game = game;
            _locate = locate;
            _upgrades = upgrades;
        }

        #region Initial State
        public async Task UnlockInitialState()
        {

            await UnlockStage("s00");
            await UnlockExpansion("x00");

            await _game.Mutate(g =>
            {
                g.SelectedStageId ??= "s00";
            }, save: true);
        }
        #endregion

        #region Character Unlock
        public async Task UnlockCharacter(string characterId)
        {
            await _game.Mutate(game =>
            {
                var character = _locate.LocateCharacter(_game.CurrentGame, characterId);

                foreach (var upgradeId in UpgradeData.ShowOrder)
                {
                    var up = _locate.LocateUpgrade(_game.CurrentGame, upgradeId);
                    if (string.Equals(up.UnlockId, character.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        up.State = UnlockHelper.State.Available;
                        Console.WriteLine($"[Unlock] Update {up.Id}: vinculado ao {character.Id}, {up.State}");
                    }
                }

                character.State = UnlockHelper.State.Unlocked;
                character.CharState = UnlockHelper.CharState.InBase;
                Console.WriteLine($"[Unlock] Character {character.Id}: {character.State}");
            }, save: true);
        }
        #endregion

        #region Coin Unlock
        public async Task UnlockCoin(string coinId)
        {
            await _game.Mutate(game =>
            {
                var coin = _locate.LocateCoin(_game.CurrentGame, coinId);

                coin.State = UnlockHelper.State.Unlocked;
                Console.WriteLine($"[Unlock] Coin {coin.Id}: {coin.State}");
            }, save: true);
        }
        #endregion

        #region Contract Unlock
        public async Task UnlockContract(string contractId)
        {
            await _game.Mutate(game =>
            {
                var contract = _locate.LocateContract(_game.CurrentGame, contractId);

                foreach (var upgradeId in UpgradeData.ShowOrder)
                {
                    var up = _locate.LocateUpgrade(_game.CurrentGame, upgradeId);
                    if (string.Equals(up.UnlockId, contract.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        up.State = UnlockHelper.State.Available;
                        Console.WriteLine($"[Unlock] {up.Id}: vinculado ao {contract.Id}, {up.State}");
                    }
                }

                contract.State = UnlockHelper.State.Unlocked;
                Console.WriteLine($"[Unlock] Character {contract.Id}: {contract.State}");
            }, save: true);
        }

        #endregion

        #region Expansion Unlock
        public async Task UnlockExpansion(string expansionId)
        {
            Console.WriteLine($"[Unlock] Expansion {expansionId}: Starting Unlock");
            await _game.Mutate(game =>
            {
                var expansion = _locate.LocateExpansion(_game.CurrentGame, expansionId);

                // Conferindo Instância do Jogo
                var fromDict = game.Expansions.TryGetValue(expansionId, out var ex)
                     ? ex
                     : null;

                Console.WriteLine($"[DBG] LocateExpansion id='{expansion.Id}', same instance? {ReferenceEquals(expansion, fromDict)}");

                foreach (var upgradeId in UpgradeData.ShowOrder)
                {
                    var up = _locate.LocateUpgrade(_game.CurrentGame, upgradeId);
                    if (string.Equals(up.UnlockId, expansion.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        up.State = UnlockHelper.State.Available;
                        Console.WriteLine($"[Unlock] {up.Id}: vinculado ao {expansion.Id}, {up.State}");
                    }
                }

                expansion.State = UnlockHelper.State.Unlocked;
                Console.WriteLine($"[Unlock] Expansion {expansion.Id}: {expansion.State}");
            }, save: true);
        }
        #endregion

        #region Knowledge Unlock
        public async Task UnlockKnowledge(string knowledgeId)
        {
            await _game.Mutate(game =>
            {
                var know = _locate.LocateKnowledge(_game.CurrentGame, knowledgeId);

                know.State = UnlockHelper.State.Unlocked;
                Console.WriteLine($"[Unlock] Character {know.Id}: {know.State}");
            }, save: true);
        }
        #endregion

        #region Local Unlock
        public async Task UnlockLocal(string localId)
        {
            await _game.Mutate(game =>
            {
                var local = _locate.LocateLocal(_game.CurrentGame, localId);

                foreach (var techId in TechData.ShowOrder)
                {
                    var tech = _locate.LocateTech(_game.CurrentGame, techId);
                    if (string.Equals(tech.UnlockId, local.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        tech.State = UnlockHelper.State.Available;
                        Console.WriteLine($"[Unlock] Tech {tech.Id}: vinculado ao {local.Id}, {tech.State}");
                    }
                }

                foreach (var expansionId in ExpansionData.ShowOrder)
                {
                    var expansion = _locate.LocateExpansion(_game.CurrentGame, expansionId);
                    if (string.Equals(expansion.UnlockId, local.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        expansion.State = UnlockHelper.State.Available;
                        Console.WriteLine($"[Unlock] Expansion {expansion.Id}: vinculado ao {local.Id}, {expansion.State}");
                    }
                }

                foreach (var upgradeId in UpgradeData.ShowOrder)
                {
                    var up = _locate.LocateUpgrade(_game.CurrentGame, upgradeId);
                    if (string.Equals(up.UnlockId, local.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        up.State = UnlockHelper.State.Available;
                        Console.WriteLine($"[Unlock] Update {up.Id}: vinculado ao {local.Id}, {up.State}");
                    }
                }

                local.State = UnlockHelper.State.Unlocked;
                Console.WriteLine($"[Unlock] Local {local.Id}: {local.State}");
            }, save: true);
        }
        #endregion

        #region Stage Unlock
        public async Task UnlockStage(string stageId)
        {
            Console.WriteLine($"[Unlock] Stage {stageId}: Starting Unlock");
            await _game.Mutate(game =>
            {
                var stage = _locate.LocateStage(_game.CurrentGame, stageId);

                foreach (var coinId in CoinsData.ShowOrder)
                {
                    var coin = _locate.LocateCoin(_game.CurrentGame, coinId);
                    if (string.Equals(coin.UnlockId, stage.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        coin.State = UnlockHelper.State.Unlocked;
                        Console.WriteLine($"[Unlock] Coin {coin.Id}: vinculado ao {stage.Id}, {coin.State}");
                    }
                }

                foreach (var upgradeId in UpgradeData.ShowOrder)
                {
                    var up = _locate.LocateUpgrade(_game.CurrentGame, upgradeId);
                    if (string.Equals(up.UnlockId, stage.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        up.State = UnlockHelper.State.Available;
                        Console.WriteLine($"[Unlock] Upgrade {up.Id}: vinculado ao {stage.Id}, {up.State}");
                    }
                }

                stage.State = UnlockHelper.State.Unlocked;

                Console.WriteLine($"[Unlock] Stage {stage.Id}: {stage.State}");
            }, save: true);
        }
        #endregion

        #region Tech Unlock
        public async Task UnlockTech(string techId)
        {
            await _game.Mutate(game =>
            {
                var tech = _locate.LocateTech(_game.CurrentGame, techId);

                foreach (var upgradeId in UpgradeData.ShowOrder)
                {
                    var up = _locate.LocateUpgrade(_game.CurrentGame, upgradeId);
                    if (string.Equals(up.UnlockId, tech.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        up.State = UnlockHelper.State.Available;
                        Console.WriteLine($"[Unlock] Update {up.Id}: vinculado ao {tech.Id}, {up.State}");
                    }
                }

                tech.State = UnlockHelper.State.Unlocked;
                Console.WriteLine($"[Unlock] Character {tech.Id}: {tech.State}");
            }, save: true);
        }
        #endregion

        #region Resource Unlock
        public async Task UnlockResource(string resourceId)
        {
            await _game.Mutate(game =>
            {
                var resource = _locate.LocateResource(_game.CurrentGame, resourceId);

                resource.State = UnlockHelper.State.Unlocked;
                Console.WriteLine($"[Unlock] Character {resource.Id}: {resource.State}");
            }, save: true);
        }
        #endregion

        #region Upgrade Unlock
        public async Task UnlockUpgrade(string upgradeId)
        {
            var up = _locate.LocateUpgrade(_game.CurrentGame, upgradeId);

            if (up.EffectOp == EffectHelper.EffectOperation.Unlock)
            {
                switch (up.EffectType)
                {
                    case EffectHelper.EffectType.ContractUnlock:
                        await UnlockContract(up.TargetId);
                        break;
                    case EffectHelper.EffectType.KnowledgeUnlock:
                        await UnlockKnowledge(up.TargetId);
                        break;
                    case EffectHelper.EffectType.LocalUnlock:
                        await UnlockLocal(up.TargetId);
                        break;
                    case EffectHelper.EffectType.CharacterUnlock:
                        await UnlockCharacter(up.TargetId);
                        break;
                    case EffectHelper.EffectType.ResourceUnlock:
                        await UnlockResource(up.TargetId);
                        break;
                    case EffectHelper.EffectType.StageUnlock:
                        await UnlockStage(up.TargetId);
                        break;
                    case EffectHelper.EffectType.ExpansionUnlock:
                        await UnlockExpansion(up.TargetId);
                        break;
                    case EffectHelper.EffectType.TechUnlock:
                        await UnlockTech(up.TargetId);
                        break;

                    default:
                        break;
                }

            } else
            {
                await _upgrades.ApplyUpgrade(_game.CurrentGame, up.Id);
            }


            await _game.Mutate(g => { 
                up.State = UnlockHelper.State.Unlocked;

                Console.WriteLine($"[Unlock] Upgrade {up.Id}: {up.State}");

            }, save: true);
        }
        #endregion
    }
}
