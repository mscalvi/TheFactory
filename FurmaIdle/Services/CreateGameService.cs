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

        bool LoadedFromOlderSchema { get; }
        bool LoadedFromOlderVersion { get; }
    }
    public sealed class CreateGameService : ICreateGameService
    {
        // Método para Salvar o Novo Jogo
        private readonly IGameStore _store;
        private readonly ICurrentGameService _game;
        private readonly IUnlockService _unlock;
        private readonly IExpeditionService _expedition;
        private readonly IGameMigrationService _migration;
        private readonly INotificationService _notification;

        public CreateGameService(IGameStore store, ICurrentGameService current, IUnlockService unlock, IExpeditionService expedition, IGameMigrationService migration, INotificationService notification)
        {
            _store = store;
            _game = current;
            _unlock = unlock;
            _expedition = expedition;
            _migration = migration;
            _notification = notification;
        }

        public GameModel NewGame { get; private set; } = new();

        private bool _loadedFromOlderSchema;
        public bool LoadedFromOlderSchema => _loadedFromOlderSchema;

        private bool _loadedFromOlderVersion;
        public bool LoadedFromOlderVersion => _loadedFromOlderVersion;

        public async Task<GameModel> InitAsync()
        {
            Console.WriteLine("[CGS] Iniciando Load/Create");

            var loaded = await _store.LoadAsync("main");

            bool invalid = loaded is null
                || loaded.Stages is null
                || loaded.Stages.Count == 0;

            if (invalid)
            {
                Console.WriteLine("[CGS] Não existe jogo salvo, ou está corrompido. Criando novo jogo");

                var model = new GameModel
                {
                    SchemaVersion = VersionHelper.SchemaVersion,
                    GameVersion = VersionHelper.GameVersion,
                    BuildVersion = VersionHelper.BuildVersion,
                    StartTime = DateTime.Now,
                    LastTick = DateTime.UtcNow,
                    GameStats = new StatsModel(),
                    SelectedStageId = "s00",
                    CurrentExpansionId = "x000",
                    Ui = new UiState(),
                    Characters = Seed("[CGS] Characters", () => CharacterData.CreateInitialStates()),
                    Clicks = Seed("[CGS] Clicks", () => ClickData.CreateInitialStates()),
                    Coins = Seed("[CGS] Coins", () => CoinsData.CreateInitialStates()),
                    Contracts = Seed("[CGS] Contracts", () => ContractData.CreateInitialStates()),
                    Expansions = Seed("[CGS] Expansions", () => ExpansionData.CreateInitialStates()),
                    Knowledges = Seed("[CGS] Knowledges", () => KnowledgeData.CreateInitialStates()),
                    Locals = Seed("[CGS] Locals", () => LocalData.CreateInitialStates()),
                    Resources = Seed("[CGS] Resources", () => ResourceData.CreateInitialStates()),
                    Routes = Seed("[CGS] Routes", () => RouteData.CreateInitialStates()),
                    Ships = Seed("[CGS] Ships", () => ShipData.CreateInitialStates()),
                    Specialties = Seed("[CGS] Specialties", () => SpecialtyData.CreateInitialStates()),
                    Stages = Seed("[CGS] Stages", () => StageData.CreateInitialStates()),
                    Techs = Seed("[CGS] Techs", () => TechData.CreateInitialStates()),
                    Traits = Seed("[CGS] Traits", () => TraitData.CreateInitialStates()),
                    Upgrades = Seed("[CGS] Upgrades", () => UpgradeData.CreateInitialStates()),
                };

                _game.Attach(model);
                Console.WriteLine("[CGS] Jogo criado.");


                await _unlock.UnlockInitialState();
                Console.WriteLine("[CGS] Estágio Inicial Desbloqueado.");

                await _expedition.FirstExpeditionStart();
                Console.WriteLine("[CGS] Primeira Expedição Iniciada.");

                _notification.InitialTabs(model);
                Console.WriteLine("[CGS] Menus carregados.");

                await _store.SaveAsync(model, "main");
                Console.WriteLine("[CGS] Jogo salvo.");

                _loadedFromOlderSchema = false;
                _loadedFromOlderVersion = false;

                return model;
            }
            else
            {
                var originalSchema = loaded.SchemaVersion <= 0 ? 1 : loaded.SchemaVersion;
                var originalVersion = loaded.GameVersion <= 0 ? 1 : loaded.GameVersion;

                _loadedFromOlderSchema = originalSchema < VersionHelper.SchemaVersion;
                _loadedFromOlderVersion = originalVersion < VersionHelper.GameVersion;

                var changed = _migration.Migrate(loaded);

                _game.Attach(loaded);
                Console.WriteLine("[CGS] Jogo carregado e anexado");

                if (changed)
                {
                    await _store.SaveAsync(loaded, "main");
                    Console.WriteLine("[CGS] Jogo atualizado e salvo");
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
    }
}
