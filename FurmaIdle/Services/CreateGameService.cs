using FurmaIdle.Data;
using FurmaIdle.Models;
using FurmaIdle.Storage;

namespace FurmaIdle.Services
{
    public interface ICreateGameService
    {
        GameModel NewGame { get; }
        Task<GameModel> InitAsync();
    }
    public sealed class CreateGameService : ICreateGameService
    {
        // Método para Salvar o Novo Jogo
        private readonly IGameStore Store;
        private readonly ICurrentGameService CurrentGame;
        private readonly ILogger<CreateGameService> _log;
        public GameModel NewGame { get; private set; } = new();

        public CreateGameService(IGameStore store, ICurrentGameService current, ILogger<CreateGameService> log)
        {
            Store = store;
            CurrentGame = current;
            _log = log;
        }

        public async Task<GameModel> InitAsync()
        {
            _log.LogInformation("InitAsync: started");

            // Método para conferir Jogo Salvo
            var loaded = await Store.LoadAsync("main");
            if (loaded is not null)
            {
                CurrentGame.Attach(loaded);
                _log.LogInformation("InitAsync: loaded save. Stages={Count}", loaded.Stages?.Count);
                return NewGame;
            }

            NewGame = new GameModel
            {
                SchemaVersion = 1,
                LastTick = DateTime.UtcNow,
                Characters = Seed("Characters", () => CharacterData.CreateInitialStates()),
                Clicks = Seed("Clicks", () => ClickData.CreateInitialStates()),
                Coins = Seed("Coins", () => CoinsData.CreateInitialStates()),
                Contracts = Seed("Contracts", () => ContractData.CreateInitialStates()),
                Expansions = Seed("Expansions", () => ExpansionData.CreateInitialStates()),
                Knowledges = Seed("Knowledges", () => KnowledgeData.CreateInitialStates()),
                Locals = Seed("Locals", () => LocalData.CreateInitialStates()),
                Resources = Seed("Resources", () => ResourceData.CreateInitialStates()),
                Specialties = Seed("Specialties", () => SpecialtyData.CreateInitialStates()),
                Stages = Seed("Stages", () => StageData.CreateInitialStates()),
                Techs = Seed("Techs", () => TechData.CreateInitialStates()),
                Traits = Seed("Traits", () => TraitData.CreateInitialStates()),
                Upgrades = Seed("Upgrades", () => UpgradeData.CreateInitialStates()),
            };

            NewGame.SelectedStageId = "s00";

            CurrentGame.Attach(NewGame);

            await Store.SaveAsync(NewGame, "main");

            _log.LogInformation("InitAsync: new game. Stages={Count}", NewGame.Stages?.Count);
            return NewGame;
        }
        private static T Seed<T>(string name, Func<T> factory)
        {
            try
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                var result = factory();
                sw.Stop();
                Console.WriteLine($"[CreateGameService] {name} ok ({sw.ElapsedMilliseconds} ms)");
                return result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[CreateGameService] ERRO em {name}: {ex.Message}\n{ex}");
                throw;
            }
        }
    }
}