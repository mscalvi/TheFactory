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
    }

    public sealed class LocateService : ILocateService
    {
        private readonly ICurrentGameService game;
        public LocateService(ICurrentGameService _game)
        {
            game = _game ?? throw new ArgumentNullException(nameof(game));
        }

        public StageModel LocateStage(string stageId)
        {
            StageModel noGame = new StageModel();
            GameModel Game = game.CurrentGame;
            if (!Game.On)
            {
                return noGame;
            }

            if(string.IsNullOrWhiteSpace(stageId))
                throw new ArgumentException("stageId inválido.", nameof(stageId));

            if(Game.Stages.TryGetValue(stageId, out var stage)) return stage;

            throw new KeyNotFoundException($"Stage '{stageId}' não encontrada no jogo atual.");
        }
        public CoinModel LocateCoin(string coinId)
        {
            CoinModel noGame = new CoinModel();
            GameModel Game = game.CurrentGame;
            if (!Game.On)
            {
                return noGame;
            }
            if (string.IsNullOrWhiteSpace(coinId))
                throw new ArgumentException("coinId inválido.", nameof(coinId));

            if (Game.Coins.TryGetValue(coinId, out var coin)) return coin;

            throw new KeyNotFoundException($"Coin '{coinId}' não encontrada no jogo atual.");
        }
        public UpgradeModel LocateUpgrade(string upgradeId)
        {
            UpgradeModel noGame = new UpgradeModel();
            GameModel Game = game.CurrentGame;
            if (!Game.On)
            {
                return noGame;
            }
            if (string.IsNullOrWhiteSpace(upgradeId))
                throw new ArgumentException("upgradeId inválido.", nameof(upgradeId));

            if (Game.Upgrades.TryGetValue(upgradeId, out var upgrade)) return upgrade;

            throw new KeyNotFoundException($"Upgrade '{upgradeId}' não encontrado no jogo atual.");
        }
        public CharacterModel LocateCharacter(string characterId)
        {
            CharacterModel noGame = new CharacterModel();
            GameModel Game = game.CurrentGame;
            if (!Game.On)
            {
                return noGame;
            }
            if (string.IsNullOrWhiteSpace(characterId))
                throw new ArgumentException("characterId inválido.", nameof(characterId));

            if (Game.Characters.TryGetValue(characterId, out var character)) return character;

            throw new KeyNotFoundException($"Character '{characterId}' não encontrado no jogo atual.");
        }
        public LocalModel LocateLocal(string localId)
        {
            LocalModel noGame = new LocalModel();
            GameModel Game = game.CurrentGame;
            if (!Game.On)
            {
                return noGame;
            }
            if (string.IsNullOrWhiteSpace(localId))
                throw new ArgumentException("localId inválido.", nameof(localId));

            if (Game.Locals.TryGetValue(localId, out var local)) return local;

            throw new KeyNotFoundException($"Local '{localId}' não encontrado no jogo atual.");
        }
        public ResourceModel LocateResource(string resourceId)
        {
            ResourceModel noGame = new ResourceModel();
            GameModel Game = game.CurrentGame;
            if (!Game.On)
            {
                return noGame;
            }

            if (string.IsNullOrWhiteSpace(resourceId))
                throw new ArgumentException("resourceId inválido.", nameof(resourceId));

            if (Game.Resources.TryGetValue(resourceId, out var resource)) return resource;

            throw new KeyNotFoundException($"Resource '{resourceId}' não encontrado no jogo atual.");
        }
        public ContractModel LocateContract(string contractId)
        {
            ContractModel noGame = new ContractModel();
            GameModel Game = game.CurrentGame;
            if (!Game.On)
            {
                return noGame;
            }

            if (string.IsNullOrWhiteSpace(contractId))
                throw new ArgumentException("contractId inválido.", nameof(contractId));

            if (Game.Contracts.TryGetValue(contractId, out var contract)) return contract;

            throw new KeyNotFoundException($"Contract '{contractId}' não encontrado no jogo atual.");
        }
        public ExpeditionModel LocateExpedition(string stageId)
        {
            ExpeditionModel noGame = new ExpeditionModel();
            GameModel Game = game.CurrentGame;
            if (!Game.On)
            {
                return noGame;
            }

            var stage = LocateStage(stageId);

            if (stage.ActiveExpedition is null)
                throw new InvalidOperationException($"Stage '{stageId}' não possui expedição ativa.");

            return stage.ActiveExpedition;
        }
        public ExpansionModel LocateExpansion(string expansionId)
        {
            ExpansionModel noGame = new ExpansionModel();
            GameModel Game = game.CurrentGame;
            if (!Game.On)
            {
                return noGame;
            }
            if (string.IsNullOrWhiteSpace(expansionId))
                throw new ArgumentException("expansionId inválido.", nameof(expansionId));

            if (Game.Expansions.TryGetValue(expansionId, out var expansion)) return expansion;

            throw new KeyNotFoundException($"Expansion '{expansionId}' não encontrada no jogo atual.");
        }
        public TechModel LocateTech(string techId)
        {
            TechModel noGame = new TechModel();
            GameModel Game = game.CurrentGame;
            if (!Game.On)
            {
                return noGame;
            }
            if (string.IsNullOrWhiteSpace(techId))
                throw new ArgumentException("techId inválido.", nameof(techId));

            if(Game.Techs.TryGetValue(techId, out var tech)) return tech;

            throw new KeyNotFoundException($"Tech '{techId}' não encontrada no jogo atual.");

        }
        public KnowledgeModel LocateKnowledge(string knowId)
        {
            KnowledgeModel noGame = new KnowledgeModel();
            GameModel Game = game.CurrentGame;
            if (!Game.On)
            {
                return noGame;
            }
            if (string.IsNullOrWhiteSpace(knowId))
                throw new ArgumentException("knowId inválido.", nameof(knowId));

            if (Game.Knowledges.TryGetValue(knowId, out var know)) return know;

            throw new KeyNotFoundException($"Knowledge '{knowId}' não encontrada no jogo atual.");

        }
        public ClickModel LocateClick(string clickId)
        {
            ClickModel noGame = new ClickModel();
            GameModel Game = game.CurrentGame;
            if (!Game.On)
            {
                return noGame;
            }
            if (string.IsNullOrWhiteSpace(clickId))
                throw new ArgumentException("clickId inválido.", nameof(clickId));

            if (Game.Clicks.TryGetValue(clickId, out var click)) return click;

            throw new KeyNotFoundException($"Click '{clickId}' não encontrado no jogo atual.");
        }
        public ClickModel LocateStageClick(string stageId)
        {
            ClickModel noGame = new ClickModel();
            GameModel Game = game.CurrentGame;
            if (!Game.On)
            {
                return noGame;
            }
            if (string.IsNullOrWhiteSpace(stageId))
                throw new ArgumentException("stageId inválido.", nameof(stageId));

            foreach (var clickId in ClickData.ShowOrder)
            {
                if (Game.Clicks.TryGetValue(clickId, out var click))
                {
                    if(click.StageId == stageId) return click;
                }
            }
            
            throw new KeyNotFoundException($"Click do Stage '{stageId}' não encontrado no jogo atual.");
        }
    }
}
