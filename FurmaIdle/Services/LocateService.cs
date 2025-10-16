using FurmaIdle.Data;
using FurmaIdle.Models;
using System.Diagnostics.Contracts;

namespace FurmaIdle.Services
{
    public interface ILocateService
    {
        StageModel LocateStage(string stageId);
        CoinModel LocateCoin(string coinId);
        UpgradeModel LocateUpgrade(string upgradeId);
        CharacterModel LocateCharacter(string characterId);
        LocalModel LocateLocal(string localId);
        ResourceModel LocateResource(string resourceId);
        ContractModel LocateContract(string contractId);
        ExpeditionModel LocateExpedition(string stageId);
        ExpansionModel LocateExpansion(string expansionId);
        TechModel LocateTech(string techId);
        KnowledgeModel LocateKnowledge(string knowledgeId);
        ClickModel LocateClick(string clickId);
        ClickModel LocateStageClick(string clickId);
        bool TryLocateClickByStage(string stageId, out ClickModel? click);
    }

    public sealed class LocateService : ILocateService
    {
        private readonly ICurrentGameService _game;
        public LocateService(ICurrentGameService game)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
        }
        private GameModel Game
            => _game.CurrentGame ?? throw new InvalidOperationException("Jogo atual não anexado.");


        public StageModel LocateStage(string stageId)
        {
            if (string.IsNullOrWhiteSpace(stageId))
                throw new ArgumentException("stageId inválido.", nameof(stageId));

            if (Game.Stages.TryGetValue(stageId, out var stage)) return stage;

            throw new KeyNotFoundException($"Stage '{stageId}' não encontrada no jogo atual.");
        }
        public CoinModel LocateCoin(string coinId)
        {
            if (string.IsNullOrWhiteSpace(coinId))
                throw new ArgumentException("coinId inválido.", nameof(coinId));

            if (Game.Coins.TryGetValue(coinId, out var coin)) return coin;

            throw new KeyNotFoundException($"Coin '{coinId}' não encontrada no jogo atual.");
        }
        public UpgradeModel LocateUpgrade(string upgradeId)
        {
            if (string.IsNullOrWhiteSpace(upgradeId))
                throw new ArgumentException("upgradeId inválido.", nameof(upgradeId));

            if (Game.Upgrades.TryGetValue(upgradeId, out var upgrade)) return upgrade;

            throw new KeyNotFoundException($"Upgrade '{upgradeId}' não encontrado no jogo atual.");
        }
        public CharacterModel LocateCharacter(string characterId)
        {
            if (string.IsNullOrWhiteSpace(characterId))
                throw new ArgumentException("characterId inválido.", nameof(characterId));

            if (Game.Characters.TryGetValue(characterId, out var character)) return character;

            throw new KeyNotFoundException($"Character '{characterId}' não encontrado no jogo atual.");
        }
        public LocalModel LocateLocal(string localId)
        {
            if (string.IsNullOrWhiteSpace(localId))
                throw new ArgumentException("localId inválido.", nameof(localId));

            if (Game.Locals.TryGetValue(localId, out var local)) return local;

            throw new KeyNotFoundException($"Local '{localId}' não encontrado no jogo atual.");
        }
        public ResourceModel LocateResource(string resourceId)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
                throw new ArgumentException("resourceId inválido.", nameof(resourceId));

            if (Game.Resources.TryGetValue(resourceId, out var resource)) return resource;

            throw new KeyNotFoundException($"Resource '{resourceId}' não encontrado no jogo atual.");
        }
        public ContractModel LocateContract(string contractId)
        {
            if (string.IsNullOrWhiteSpace(contractId))
                throw new ArgumentException("contractId inválido.", nameof(contractId));

            if (Game.Contracts.TryGetValue(contractId, out var contract)) return contract;

            throw new KeyNotFoundException($"Contract '{contractId}' não encontrado no jogo atual.");
        }
        public ExpeditionModel LocateExpedition(string stageId)
        {
            var stage = LocateStage(stageId);

            if (stage.ActiveExpedition is null)
                throw new InvalidOperationException($"Stage '{stageId}' não possui expedição ativa.");

            return stage.ActiveExpedition;
        }
        public ExpansionModel LocateExpansion(string expansionId)
        {
            if (string.IsNullOrWhiteSpace(expansionId))
                throw new ArgumentException("expansionId inválido.", nameof(expansionId));

            if (Game.Expansions.TryGetValue(expansionId, out var expansion)) return expansion;

            throw new KeyNotFoundException($"Expansion '{expansionId}' não encontrada no jogo atual.");
        }
        public TechModel LocateTech(string techId)
        {
            if (string.IsNullOrWhiteSpace(techId))
                throw new ArgumentException("techId inválido.", nameof(techId));

            if(Game.Techs.TryGetValue(techId, out var tech)) return tech;

            throw new KeyNotFoundException($"Tech '{techId}' não encontrada no jogo atual.");

        }
        public KnowledgeModel LocateKnowledge(string knowId)
        {
            if (string.IsNullOrWhiteSpace(knowId))
                throw new ArgumentException("knowId inválido.", nameof(knowId));

            if (Game.Knowledges.TryGetValue(knowId, out var know)) return know;

            throw new KeyNotFoundException($"Knowledge '{knowId}' não encontrada no jogo atual.");

        }
        public ClickModel LocateClick(string clickId)
        {
            if (string.IsNullOrWhiteSpace(clickId))
                throw new ArgumentException("clickId inválido.", nameof(clickId));

            if (Game.Clicks.TryGetValue(clickId, out var cm)) return cm;

            throw new KeyNotFoundException($"Click '{clickId}' não encontrado no jogo atual.");
        }
        public ClickModel LocateStageClick(string stageId)
        {
            if (string.IsNullOrWhiteSpace(stageId))
                throw new ArgumentException("stageId inválido.", nameof(stageId));

            // 1) catálogo: encontra o click def daquele stage
            var defId = ClickData.ShowOrder
                .Select(id => ClickData.GetDef(id))
                .FirstOrDefault(d => string.Equals(d?.StageId, stageId, StringComparison.OrdinalIgnoreCase))
                ?.Id;

            if (string.IsNullOrWhiteSpace(defId))
                throw new KeyNotFoundException($"Nenhum Click definido para o Stage '{stageId}'.");

            // 2) runtime: retorna o modelo vivo
            if (Game.Clicks.TryGetValue(defId, out var cm)) return cm;

            throw new KeyNotFoundException($"Click '{defId}' (do Stage '{stageId}') não encontrado no jogo atual.");
        }
        public bool TryLocateClickByStage(string stageId, out ClickModel? click)
        {
            click = null;
            if (string.IsNullOrWhiteSpace(stageId)) return false;

            // catálogo → acha o clickId do stage
            var defId = ClickData.ShowOrder
                ?.Select(id => ClickData.GetDef(id))
                ?.FirstOrDefault(d => d != null &&
                                      string.Equals(d.StageId, stageId, StringComparison.OrdinalIgnoreCase))
                ?.Id;

            if (string.IsNullOrWhiteSpace(defId)) return false;

            // runtime
            return Game.Clicks.TryGetValue(defId, out click);
        }
    }
}
