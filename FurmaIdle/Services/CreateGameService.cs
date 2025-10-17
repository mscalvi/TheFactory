using FurmaIdle.Data;
using FurmaIdle.Helpers;
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
        private readonly IUnlockService Unlock;
        public GameModel NewGame { get; private set; } = new();

        public CreateGameService(IGameStore store, ICurrentGameService current, IUnlockService unlock)
        {
            Store = store;
            CurrentGame = current;
            Unlock = unlock;
        }

        public async Task<GameModel> InitAsync()
        {
            Console.WriteLine("[CGS] Iniciando Load/Create");

            var LoadGame = await Store.LoadAsync("main");

            if (LoadGame == null)
            {
                Console.WriteLine("[CGS] Não existe jogo Salvo. Criando novo jogo");
                NewGame = new GameModel
                {
                    SchemaVersion = 1,
                    LastTick = DateTime.UtcNow,
                    Characters = Seed("[CGS] Characters", () => CharacterData.CreateInitialStates()),
                    Clicks = Seed("[CGS] Clicks", () => ClickData.CreateInitialStates()),
                    Coins = Seed("[CGS] Coins", () => CoinsData.CreateInitialStates()),
                    Contracts = Seed("[CGS] Contracts", () => ContractData.CreateInitialStates()),
                    Expansions = Seed("[CGS] Expansions", () => ExpansionData.CreateInitialStates()),
                    Knowledges = Seed("[CGS] Knowledges", () => KnowledgeData.CreateInitialStates()),
                    Locals = Seed("[CGS] Locals", () => LocalData.CreateInitialStates()),
                    Resources = Seed("[CGS] Resources", () => ResourceData.CreateInitialStates()),
                    Specialties = Seed("[CGS] Specialties", () => SpecialtyData.CreateInitialStates()),
                    Stages = Seed("[CGS] Stages", () => StageData.CreateInitialStates()),
                    Techs = Seed("[CGS] Techs", () => TechData.CreateInitialStates()),
                    Traits = Seed("[CGS] Traits", () => TraitData.CreateInitialStates()),
                    Upgrades = Seed("[CGS] Upgrades", () => UpgradeData.CreateInitialStates()),
                };

                CurrentGame.Attach(NewGame);

                Console.WriteLine($"[CGS] Jogo criado");

                await Store.SaveAsync(NewGame, "main");

                Console.WriteLine("[CGS] Jogo salvo.");

                return NewGame;
            }else
            {
                return null;
            }
        }

        private static T Seed<T>(string name, Func<T> factory)
        {
            try
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                var result = factory();
                sw.Stop();
                Console.WriteLine($"[CGS] {name} ok ({sw.ElapsedMilliseconds} ms)");
                return result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[CGS] ERRO em {name}: {ex.Message}\n{ex}");
                throw;
            }
        }

        private static bool BackfillLoad(GameModel g)
        {
            return true;
        }
    }
}
