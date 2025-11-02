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

        public UnlockService(ICurrentGameService game, ILocateService locate)
        {
            _game = game;
            _locate = locate;
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
                var character = _locate.LocateCharacter(game, characterId);

                foreach (var up in game.Upgrades)
                {
                    if (string.Equals(up.Value.UnlockId, character.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        up.Value.State = UnlockHelper.State.Available;
                        Console.WriteLine($"[Unlock] Upgrade {up.Value.Id}: {up.Value.State}");
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
                var coin = _locate.LocateCoin(game, coinId);

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
                var contract = _locate.LocateContract(game, contractId);

                foreach (var up in game.Upgrades)
                {
                    if (string.Equals(up.Value.UnlockId, contract.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        up.Value.State = UnlockHelper.State.Available;
                        Console.WriteLine($"[Unlock] Upgrade {up.Value.Id}: {up.Value.State}");
                    }
                }

                contract.State = UnlockHelper.State.Unlocked;
                Console.WriteLine($"[Unlock] Contract {contract.Id}: {contract.State}");
            }, save: true);
        }

        #endregion

        #region Expansion Unlock
        public async Task UnlockExpansion(string expansionId)
        {
            await _game.Mutate(game =>
            {
                var expansion = _locate.LocateExpansion(game, expansionId);

                foreach (var up in game.Upgrades)
                {
                    if (string.Equals(up.Value.UnlockId, expansion.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        up.Value.State = UnlockHelper.State.Available;
                        Console.WriteLine($"[Unlock] Upgrade {up.Value.Id}: {up.Value.State}");
                    }
                }
                if (expansion.State != UnlockHelper.State.Unlocked)
                {
                    expansion.StartedAt = DateTime.Now;
                }

                expansion.State = UnlockHelper.State.Unlocked;
                Console.WriteLine($"[Unlock] Expansion {expansion.Id}: {expansion.State} -> {expansion.StartedAt}");
            }, save: true);
        }
        #endregion

        #region Knowledge Unlock
        public async Task UnlockKnowledge(string knowledgeId)
        {
            await _game.Mutate(game =>
            {
                var know = _locate.LocateKnowledge(game, knowledgeId);

                know.State = UnlockHelper.State.Unlocked;
                Console.WriteLine($"[Unlock] Knowledge {know.Id}: {know.State}");
            }, save: true);
        }
        #endregion

        #region Local Unlock
        public async Task UnlockLocal(string localId)
        {
            await _game.Mutate(game =>
            {
                var local = _locate.LocateLocal(game, localId);

                foreach (var tech in game.Techs)
                {
                    if (string.Equals(tech.Value.UnlockId, local.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        tech.Value.State = UnlockHelper.State.Available;
                        Console.WriteLine($"[Unlock] Tech {tech.Value.Id}: {tech.Value.State}");
                    }
                }

                foreach (var expansion in game.Expansions)
                {
                    if (string.Equals(expansion.Value.UnlockId, local.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        expansion.Value.State = UnlockHelper.State.Available;
                        Console.WriteLine($"[Unlock] Expansion {expansion.Value.Id}: {expansion.Value.State}");
                    }
                }

                foreach (var up in game.Upgrades)
                {
                    if (string.Equals(up.Value.UnlockId, local.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        up.Value.State = UnlockHelper.State.Available;
                        Console.WriteLine($"[Unlock] Upgrade {up.Value.Id}: {up.Value.State}");
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
            await _game.Mutate(game =>
            {
                var stage = _locate.LocateStage(game, stageId);

                foreach (var coin in game.Coins)
                {
                    if (string.Equals(coin.Value.UnlockId, stage.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        coin.Value.State = UnlockHelper.State.Unlocked;
                        Console.WriteLine($"[Unlock] Coin {coin.Value.Id}: {coin.Value.State}");
                    }
                }

                foreach (var up in game.Upgrades)
                {
                    if (string.Equals(up.Value.UnlockId, stage.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        up.Value.State = UnlockHelper.State.Available;
                        Console.WriteLine($"[Unlock] Upgrade {up.Value.Id}: {up.Value.State}");
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
                var tech = _locate.LocateTech(game, techId);

                foreach (var up in game.Upgrades)
                {
                    if (string.Equals(up.Value.UnlockId, tech.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        up.Value.State = UnlockHelper.State.Available;
                        Console.WriteLine($"[Unlock] Upgrade {up.Value.Id}: {up.Value.State}");
                    }
                }

                tech.State = UnlockHelper.State.Unlocked;
                Console.WriteLine($"[Unlock] Tech {tech.Id}: {tech.State}");
            }, save: true);
        }
        #endregion

        #region Resource Unlock
        public async Task UnlockResource(string resourceId)
        {
            await _game.Mutate(game =>
            {
                var resource = _locate.LocateResource(game, resourceId);

                resource.State = UnlockHelper.State.Unlocked;
                Console.WriteLine($"[Unlock] Resource {resource.Id}: {resource.State}");
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
                    case EffectHelper.EffectType.CoinUnlock:
                        await UnlockCoin(up.TargetId);
                        break;

                    default:
                        break;
                }
            }

            await _game.Mutate(g => { 
                up.State = UnlockHelper.State.Unlocked;

                Console.WriteLine($"[Unlock] Upgrade {up.Id}: {up.State}");

            }, save: true);
        }
        #endregion
    }
}
