using FurmaIdle.Helpers;
using FurmaIdle.Models;
using FurmaIdle.Data;

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
            }, save: true);
        }
        #endregion

        #region Coin Unlock
        public async Task UnlockCoin(string coinId)
        {
            await _game.Mutate(game =>
            {
                var coin = _locate.LocateCoin(coinId);

                coin.State = UnlockHelper.State.Unlocked;
            }, save: true);
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
            }, save: true);
        }

        #endregion

        #region Expansion Unlock
        public async Task UnlockExpansion(string expansionId)
        {
            Console.WriteLine($"[Unlock] Expansion {expansionId}: Starting Unlock");
            await _game.Mutate(game =>
            {
                var expansion = _locate.LocateExpansion(expansionId);

                // Conferindo Instância do Jogo
                var fromDict = game.Expansions.TryGetValue(expansionId, out var ex)
                     ? ex
                     : null;

                Console.WriteLine($"[DBG] LocateExpansion id='{expansion.Id}', same instance? {ReferenceEquals(expansion, fromDict)}");

                foreach (var upgradeId in UpgradeData.ShowOrder)
                {
                    var up = _locate.LocateUpgrade(upgradeId);
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
                var know = _locate.LocateKnowledge(knowledgeId);

                know.State = UnlockHelper.State.Unlocked;
            }, save: true);
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
            }, save: true);
        }
        #endregion

        #region Stage Unlock
        public async Task UnlockStage(string stageId)
        {
            Console.WriteLine($"[Unlock] Stage {stageId}: Starting Unlock");
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
                            Console.WriteLine($"[Unlock] Coin {coin.Id}: vinculado ao {stage.Id}, {coin.State}");
                        }
                        else
                        {
                            coin.State = UnlockHelper.State.Available;
                            Console.WriteLine($"[Unlock] Coin {coin.Id}: vinculado ao {stage.Id}, {coin.State}");
                        }
                    }
                }

                foreach (var upgradeId in UpgradeData.ShowOrder)
                {
                    var up = _locate.LocateUpgrade(upgradeId);
                    if (string.Equals(up.UnlockId, stage.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        up.State = UnlockHelper.State.Available;
                        Console.WriteLine($"[Unlock] Upgrade {up.Id}: vinculado ao {stage.Id}, {up.State}");
                    }
                }

                stage.State = UnlockHelper.State.Unlocked;

                Console.WriteLine($"[Unlock] Stage {stage.Id} - {stage.Name}: {stage.State}");
            }, save: true);
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
            }, save: true);
        }
        #endregion

        #region Resource Unlock
        public async Task UnlockResource(string resourceId)
        {
            await _game.Mutate(game =>
            {
                var resource = _locate.LocateResource(resourceId);

                resource.State = UnlockHelper.State.Unlocked;
            }, save: true);
        }
        #endregion

        #region Upgrade Unlock
        public async Task UnlockUpgrade(string upgradeId)
        {
            var up = _locate.LocateUpgrade(upgradeId);

            // 1) marca a própria upgrade como unlocked e salva
            await _game.Mutate(g => { up.State = UnlockHelper.State.Unlocked; }, save: true);

            // 2) agora, fora do Mutate, encadeia o efeito com await
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
                default:
                    break;
            }
        }

        #endregion

    }
}
