using FurmaIdle.Data;
using FurmaIdle.Models;

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
        ClickModel LocateStageClick(string stageId);
    }

    public sealed class LocateService : ILocateService
    {
        private readonly ICurrentGameService _game;

        public LocateService(ICurrentGameService game)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
        }

        // Se quiser, troque este helper para validar "readiness" (ex.: _game.IsReady)
        private GameModel G => _game.CurrentGame ?? throw new InvalidOperationException("GameModel não anexado.");

        public StageModel LocateStage(string stageId)
        {
            if (string.IsNullOrWhiteSpace(stageId))
                throw new ArgumentException("stageId inválido.", nameof(stageId));

            if (G.Stages.TryGetValue(stageId, out var stage)) return stage;
            throw new KeyNotFoundException($"Stage '{stageId}' não encontrada no jogo atual.");
        }

        public CoinModel LocateCoin(string coinId)
        {
            if (string.IsNullOrWhiteSpace(coinId))
                throw new ArgumentException("coinId inválido.", nameof(coinId));

            if (G.Coins.TryGetValue(coinId, out var coin)) return coin;
            throw new KeyNotFoundException($"Coin '{coinId}' não encontrada no jogo atual.");
        }

        public UpgradeModel LocateUpgrade(string upgradeId)
        {
            if (string.IsNullOrWhiteSpace(upgradeId))
                throw new ArgumentException("upgradeId inválido.", nameof(upgradeId));

            if (G.Upgrades.TryGetValue(upgradeId, out var up)) return up;
            throw new KeyNotFoundException($"Upgrade '{upgradeId}' não encontrada no jogo atual.");
        }

        public CharacterModel LocateCharacter(string characterId)
        {
            if (string.IsNullOrWhiteSpace(characterId))
                throw new ArgumentException("characterId inválido.", nameof(characterId));

            if (G.Characters.TryGetValue(characterId, out var c)) return c;
            throw new KeyNotFoundException($"Character '{characterId}' não encontrado no jogo atual.");
        }

        public LocalModel LocateLocal(string localId)
        {
            if (string.IsNullOrWhiteSpace(localId))
                throw new ArgumentException("localId inválido.", nameof(localId));

            if (G.Locals.TryGetValue(localId, out var l)) return l;
            throw new KeyNotFoundException($"Local '{localId}' não encontrado no jogo atual.");
        }

        public ResourceModel LocateResource(string resourceId)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
                throw new ArgumentException("resourceId inválido.", nameof(resourceId));

            if (G.Resources.TryGetValue(resourceId, out var r)) return r;
            throw new KeyNotFoundException($"Resource '{resourceId}' não encontrado no jogo atual.");
        }

        public ContractModel LocateContract(string contractId)
        {
            if (string.IsNullOrWhiteSpace(contractId))
                throw new ArgumentException("contractId inválido.", nameof(contractId));

            if (G.Contracts.TryGetValue(contractId, out var c)) return c;
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

            if (G.Expansions.TryGetValue(expansionId, out var e)) return e;
            throw new KeyNotFoundException($"Expansion '{expansionId}' não encontrada no jogo atual.");
        }

        public TechModel LocateTech(string techId)
        {
            if (string.IsNullOrWhiteSpace(techId))
                throw new ArgumentException("techId inválido.", nameof(techId));

            if (G.Techs.TryGetValue(techId, out var t)) return t;
            throw new KeyNotFoundException($"Tech '{techId}' não encontrada no jogo atual.");
        }

        public KnowledgeModel LocateKnowledge(string knowledgeId)
        {
            if (string.IsNullOrWhiteSpace(knowledgeId))
                throw new ArgumentException("knowledgeId inválido.", nameof(knowledgeId));

            if (G.Knowledges.TryGetValue(knowledgeId, out var k)) return k;
            throw new KeyNotFoundException($"Knowledge '{knowledgeId}' não encontrada no jogo atual.");
        }

        public ClickModel LocateStageClick(string stageId)
        {
            if (string.IsNullOrWhiteSpace(stageId))
                throw new ArgumentException("stageId inválido.", nameof(stageId));


            if (G.Stages.TryGetValue(stageId, out var st))
            {
                if (G.Clicks.TryGetValue(st.ClickId, out var cl)) return cl;
            }

            throw new KeyNotFoundException($"Click do Stage '{stageId}' não encontrado no jogo atual.");
        }
    }
}
    
