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

            var loaded = await Store.LoadAsync("main");

            bool invalid = loaded is null
            || loaded.SchemaVersion <= 0
            || loaded.Stages is null
            || loaded.Stages.Count == 0;

            if (invalid)
            {
                Console.WriteLine("[CGS] Não existe jogo salvo, ou está corrompido. Criando novo jogo");

                var model = new GameModel
                {
                    SchemaVersion = 1,
                    LastTick = DateTime.UtcNow,
                    ExpansionStats = new StatsModel(),
                    GameStats = new StatsModel(),
                    LastExpansionId = "x00",
                    UnlockedMenus = new(StringComparer.OrdinalIgnoreCase),
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

                CurrentGame.Attach(model);
                Console.WriteLine("[CGS] Jogo criado");

                await Unlock.UnlockInitialState();
                Console.WriteLine("[CGS] Estágio Inicial Desbloqueado");

                await Store.SaveAsync(model, "main");
                Console.WriteLine("[CGS] Jogo salvo");

                return model;
            }
            else
            {
                var changed = BackfillLoad(loaded);

                CurrentGame.Attach(loaded);
                Console.WriteLine("[CGS] Jogo carregado e anexado");

                if (changed)
                {
                    await Store.SaveAsync(loaded, "main");
                    Console.WriteLine("[CGS] Jogo atualizado (backfill) e salvo");
                }

                return loaded;
            }
        }

        private static T Seed<T>(string name, Func<T> factory)
        {
            try
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                var result = factory();
                sw.Stop();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static bool BackfillLoad(GameModel g)
        {
            // TO DO
            return false;
        }
    }
}
