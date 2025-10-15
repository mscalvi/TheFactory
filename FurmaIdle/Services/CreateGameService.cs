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
        public CreateGameService(IGameStore store) { Store = store; }
        public GameModel NewGame { get; private set; } = new();
        public async Task<GameModel> InitAsync()
        {
            // Método para conferir Jogo Salvo
            var loaded = await Store.LoadAsync("main");
            if (loaded is not null)
            {
                NewGame = loaded;
                return NewGame;
            }

            NewGame = new GameModel
            {
                SchemaVersion = 1,
                Clicks = Seed("Clicks", () => ClickData.CreateInitialStates()),
                Stages = Seed("Stages", () => StageData.CreateInitialStates()),
                Locals = Seed("Locals", () => LocalData.CreateInitialStates()),
                Techs = Seed("Techs", () => TechData.CreateInitialStates()),
                Upgrades = Seed("Upgrades", () => UpgradeData.CreateInitialStates()),
                Resources = Seed("Resources", () => ResourceData.CreateInitialStates()),
                Characters = Seed("Characters", () => CharacterData.CreateInitialStates()),
                Contracts = Seed("Contracts", () => ContractData.CreateInitialStates()),
                Knowledges = Seed("Knowledges", () => KnowledgeData.CreateInitialStates())
            };

            NewGame.SelectedStageId = "s00";

            //await Store.SaveAsync(NewGame, "main");
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