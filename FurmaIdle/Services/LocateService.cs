using FurmaIdle.Data;
using FurmaIdle.Models;

namespace FurmaIdle.Services
{
    public interface ILocateService
    {
        StageModel LocateStage(GameModel game, string stageId);
        CoinModel LocateCoin(GameModel game, string coinId);
        ClickModel LocateClick(GameModel game, string clickId);
        UpgradeModel LocateUpgrade(GameModel game, string upgradeId);
        CharacterModel LocateCharacter(GameModel game, string characterId);
        LocalModel LocateLocal(GameModel game, string localId);
        ResourceModel LocateResource(GameModel game, string resourceId);
        ContractModel LocateContract(GameModel game, string contractId);
        ExpeditionModel LocateExpedition(GameModel game, string stageId);
        ExpansionModel LocateExpansion(GameModel game, string expansionId);
        TechModel LocateTech(GameModel game, string techId);
        KnowledgeModel LocateKnowledge(GameModel game, string knowledgeId);
        SpecialtyModel LocateSpecialty(GameModel game, string specialtyId);
        ClickModel LocateStageClick(GameModel game, string stageId);
        TraitModel LocateTrait(GameModel game, string traitId);
    }

    public sealed class LocateService : ILocateService
    {
        public StageModel LocateStage(GameModel game, string stageId)
        {
            if (string.IsNullOrWhiteSpace(stageId))
                throw new ArgumentException("stageId inválido.", nameof(stageId));

            if (game.Stages.TryGetValue(stageId, out var stage)) return stage;
            throw new KeyNotFoundException($"Stage '{stageId}' não encontrada no jogo atual.");
        }

        public CoinModel LocateCoin(GameModel game, string coinId)
        {
            if (string.IsNullOrWhiteSpace(coinId))
                throw new ArgumentException("coinId inválido.", nameof(coinId));

            if (game.Coins.TryGetValue(coinId, out var coin)) return coin;
            throw new KeyNotFoundException($"Coin '{coinId}' não encontrada no jogo atual.");
        }

        public ClickModel LocateClick(GameModel game, string clickId)
        {
            if (string.IsNullOrWhiteSpace(clickId))
                throw new ArgumentException("clickId inválido.", nameof(clickId));

            if (game.Clicks.TryGetValue(clickId, out var click)) return click;
            throw new KeyNotFoundException($"Click '{clickId}' não encontrada no jogo atual.");
        }

        public UpgradeModel LocateUpgrade(GameModel game, string upgradeId)
        {
            if (string.IsNullOrWhiteSpace(upgradeId))
                throw new ArgumentException("upgradeId inválido.", nameof(upgradeId));

            if (game.Upgrades.TryGetValue(upgradeId, out var up)) return up;
            throw new KeyNotFoundException($"Upgrade '{upgradeId}' não encontrada no jogo atual.");
        }

        public CharacterModel LocateCharacter(GameModel game, string characterId)
        {
            if (string.IsNullOrWhiteSpace(characterId))
                throw new ArgumentException("characterId inválido.", nameof(characterId));

            if (game.Characters.TryGetValue(characterId, out var c)) return c;
            throw new KeyNotFoundException($"Character '{characterId}' não encontrado no jogo atual.");
        }

        public LocalModel LocateLocal(GameModel game, string localId)
        {
            if (string.IsNullOrWhiteSpace(localId))
                throw new ArgumentException("localId inválido.", nameof(localId));

            if (game.Locals.TryGetValue(localId, out var l)) return l;
            throw new KeyNotFoundException($"Local '{localId}' não encontrado no jogo atual.");
        }

        public ResourceModel LocateResource(GameModel game, string resourceId)
        {
            if (string.IsNullOrWhiteSpace(resourceId))
                throw new ArgumentException("resourceId inválido.", nameof(resourceId));

            if (game.Resources.TryGetValue(resourceId, out var r)) return r;
            throw new KeyNotFoundException($"Resource '{resourceId}' não encontrado no jogo atual.");
        }

        public ContractModel LocateContract(GameModel game, string contractId)
        {
            if (string.IsNullOrWhiteSpace(contractId))
                throw new ArgumentException("contractId inválido.", nameof(contractId));

            if (game.Contracts.TryGetValue(contractId, out var c)) return c;
            throw new KeyNotFoundException($"Contract '{contractId}' não encontrado no jogo atual.");
        }

        public ExpeditionModel LocateExpedition(GameModel game, string stageId)
        {
            var stage = LocateStage(game, stageId);
            if (stage.Expedition is null)
                throw new InvalidOperationException($"Stage '{stageId}' não possui expedição ativa.");
            return stage.Expedition;
        }

        public ExpansionModel LocateExpansion(GameModel game, string expansionId)
        {
            if (string.IsNullOrWhiteSpace(expansionId))
                throw new ArgumentException("expansionId inválido.", nameof(expansionId));

            if (game.Expansions.TryGetValue(expansionId, out var e)) return e;
            throw new KeyNotFoundException($"Expansion '{expansionId}' não encontrada no jogo atual.");
        }

        public TechModel LocateTech(GameModel game, string techId)
        {
            if (string.IsNullOrWhiteSpace(techId))
                throw new ArgumentException("techId inválido.", nameof(techId));

            if (game.Techs.TryGetValue(techId, out var t)) return t;
            throw new KeyNotFoundException($"Tech '{techId}' não encontrada no jogo atual.");
        }

        public KnowledgeModel LocateKnowledge(GameModel game, string knowledgeId)
        {
            if (string.IsNullOrWhiteSpace(knowledgeId))
                throw new ArgumentException("knowledgeId inválido.", nameof(knowledgeId));

            if (game.Knowledges.TryGetValue(knowledgeId, out var k)) return k;
            throw new KeyNotFoundException($"Knowledge '{knowledgeId}' não encontrada no jogo atual.");
        }

        public SpecialtyModel LocateSpecialty(GameModel game, string specialtyId)
        {
            if (string.IsNullOrWhiteSpace(specialtyId))
                throw new ArgumentException("specialtyId inválido.", nameof(specialtyId));

            if (game.Specialties.TryGetValue(specialtyId, out var s)) return s;
            throw new KeyNotFoundException($"Specialty '{specialtyId}' não encontrada no jogo atual.");
        }

        public TraitModel LocateTrait(GameModel game, string traitId)
        {
            if (string.IsNullOrWhiteSpace(traitId))
                throw new ArgumentException("traitId inválido.", nameof(traitId));

            if (game.Traits.TryGetValue(traitId, out var trait)) return trait;
            throw new KeyNotFoundException($"Trait '{traitId}' não encontrada no jogo atual.");
        }

        public ClickModel LocateStageClick(GameModel game, string stageId)
        {
            if (string.IsNullOrWhiteSpace(stageId))
                throw new ArgumentException("stageId inválido.", nameof(stageId));

            if (game.Stages.TryGetValue(stageId, out var st))
            {
                if (game.Clicks.TryGetValue(st.ClickId, out var cl)) return cl;
            }

            throw new KeyNotFoundException($"Click do Stage '{stageId}' não encontrado no jogo atual.");
        }
    }
}
    
