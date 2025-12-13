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
        Task UnlockShip(string shipId);
        Task UnlockRoute(string shipId);
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

            await UnlockExpansion("x000");

            await UnlockCharacter("p0001");

            await UnlockContract("c001");

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
                        if (up.Value.State != UnlockHelper.State.Blocked) continue;
                        up.Value.State = UnlockHelper.State.Available;
                    }
                }

                character.State = UnlockHelper.State.Unlocked;
                character.CharState = UnlockHelper.CharState.InBase;
                game.GameStats.CharactersUnlocked++;
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
                game.GameStats.CoinsUnlocked++;
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
                        if (up.Value.State != UnlockHelper.State.Blocked) continue;
                        up.Value.State = UnlockHelper.State.Available;
                    }
                }

                contract.State = UnlockHelper.State.Unlocked;
                game.GameStats.ContractsUnlocked++;
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
                        if (up.Value.State != UnlockHelper.State.Blocked) continue;
                        up.Value.State = UnlockHelper.State.Available;
                    }
                }

                foreach (var nextExpansion in game.Expansions)
                {
                    if (string.Equals(nextExpansion.Value.UnlockId, expansion.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        if (nextExpansion.Value.State != UnlockHelper.State.Blocked) continue;

                        nextExpansion.Value.State = UnlockHelper.State.Available;
                    }
                }

                expansion.State = UnlockHelper.State.Unlocked;
                game.GameStats.ExpansionsUnlocked++;

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
                game.GameStats.KnowledgesUnlocked++;
            }, save: true);
        }
        #endregion

        #region Local Unlock
        public async Task UnlockLocal(string localId)
        {
            await _game.Mutate(game =>
            {
                var local = _locate.LocateLocal(game, localId);

                foreach (var up in game.Upgrades)
                {
                    if (string.Equals(up.Value.UnlockId, local.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        if (up.Value.State != UnlockHelper.State.Blocked) continue;
                        up.Value.State = UnlockHelper.State.Available;
                    }
                }

                local.State = UnlockHelper.State.Unlocked;
                game.GameStats.LocalsUnlocked++;
            }, save: true);
        }
        #endregion

        #region Stage Unlock
        public async Task UnlockStage(string stageId)
        {
            bool newCoin = false;
            string newCoinId = "";

            bool newLocal = false;
            string newLocalId = "";

            await _game.Mutate(game =>
            {
                var stage = _locate.LocateStage(game, stageId);

                foreach (var coin in game.Coins)
                {
                    if (string.Equals(coin.Value.UnlockId, stage.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        if (coin.Value.State != UnlockHelper.State.Blocked) continue;
                        coin.Value.State = UnlockHelper.State.Available;
                        newCoin = true;
                        newCoinId = coin.Value.Id;
                    }
                }

                foreach (var local in game.Locals)
                {
                    if (string.Equals(local.Value.UnlockId, stage.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        if (local.Value.State != UnlockHelper.State.Blocked) continue;
                        local.Value.State = UnlockHelper.State.Available;
                        newLocal = true;
                        newLocalId = local.Value.Id;
                    }
                }

                foreach (var up in game.Upgrades)
                {
                    if (string.Equals(up.Value.UnlockId, stage.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        if (up.Value.State != UnlockHelper.State.Blocked) continue;
                        up.Value.State = UnlockHelper.State.Available;
                    }
                }

                stage.State = UnlockHelper.State.Unlocked;

                game.GameStats.StagesUnlocked++;
            }, save: true);

            if (newCoin)
            {
                await UnlockCoin(newCoinId);
            }

            if (newLocal)
            {
                await UnlockLocal(newLocalId);
            }

            if(stageId == "s01")
            {
                await UnlockExpansion("x010");
            }
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
                        if (up.Value.State != UnlockHelper.State.Blocked) continue;
                        up.Value.State = UnlockHelper.State.Available;
                    }
                }

                tech.State = UnlockHelper.State.Unlocked;
                game.GameStats.TechUnlocked++;
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
                game.GameStats.ResourcesUnlocked++;
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
                    case EffectHelper.EffectType.ShipUnlock:
                        await UnlockShip(up.TargetId);
                        break;
                    case EffectHelper.EffectType.RouteUnlock:
                        await UnlockRoute(up.TargetId);
                        break;

                    default:
                        break;
                }
            }

            await _game.Mutate(game => {

                foreach (var upgrade in game.Upgrades)
                {
                    if (string.Equals(upgrade.Value.UnlockId, up.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        if (upgrade.Value.State != UnlockHelper.State.Blocked) continue;
                        upgrade.Value.State = UnlockHelper.State.Available;
                    }
                }

                up.State = UnlockHelper.State.Unlocked;
                                
                game.GameStats.UpgradesUnlocked++;

            }, save: true);
        }
        #endregion

        #region Ship Unlock
        public async Task UnlockShip(string shipId)
        {
            await _game.Mutate(game =>
            {
                var ship = _locate.LocateShip(game, shipId);

                foreach (var up in game.Upgrades)
                {
                    if (string.Equals(up.Value.UnlockId, ship.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        if (up.Value.State != UnlockHelper.State.Blocked) continue;
                        up.Value.State = UnlockHelper.State.Available;
                    }
                }

                ship.State = UnlockHelper.State.Unlocked;
                ship.ShipState = UnlockHelper.ShipState.InStage;
                ship.InStageId = "s01";
                game.GameStats.ShipsUnlocked++;
            }, save: true);
        }
        #endregion

        #region Route Unlock
        public async Task UnlockRoute(string routeId)
        {
            await _game.Mutate(game =>
            {
                var route = _locate.LocateRoute(game, routeId);

                foreach (var up in game.Upgrades)
                {
                    if (string.Equals(up.Value.UnlockId, route.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        if (up.Value.State != UnlockHelper.State.Blocked) continue;
                        up.Value.State = UnlockHelper.State.Available;
                    }
                }

                route.State = UnlockHelper.State.Unlocked;
                route.RouteState = UnlockHelper.RouteState.Available;
                game.GameStats.RoutesUnlocked++;
            }, save: true);
        }
        #endregion
    }
}
