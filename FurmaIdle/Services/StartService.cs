using FurmaIdle.Data;
using FurmaIdle.Models;
using FurmaIdle.Storage;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static FurmaIdle.Storage.GameStorage;

namespace FurmaIdle.Services
{
    public interface IStartService
    {
        GameModel Current { get; }
        Task<GameModel> InitAsync();
    }

    public sealed class StartService : IStartService
    {
        private readonly IGameStore _store;
        public StartService(IGameStore store) { _store = store; }

        public GameModel Current { get; private set; } = new();

        public async Task<GameModel> InitAsync()
        {
            var loaded = await _store.LoadAsync("main");
            if (loaded is not null)
            {
                Current = loaded;
                return Current;
            }

            Current = new GameModel
            {
                Clicks = new(),
                SchemaVersion = 1,

                Stages = Seed("Stages", () => StageData.CreateInitialStages()),
                Destinations = Seed("Destinations", () => DestinationData.CreateInitialDestinations()),
                Technologies = Seed("Technologies", () => TechData.CreateInitialTechs()),
                Upgrades = Seed("Upgrades", () => UpgradeData.CreateInitialUpgrades()),
                Resources = Seed("Resources", () => ResourceData.CreateInitialResources()),
                Characters = Seed("Characters", () => CharacterData.CreateInitialStates()),
                Contracts = Seed("Contracts", () => ContractData.CreateInitialContracts()),
                Knowledges = Seed("Knowledges", () => KnowledgeData.CreateInitialKnowledges())
            };

            foreach (var (sid, stage) in Current.Stages)
            {
                if (!stage.Unlocked) continue;
                Current.Clicks[sid] = new ClickModel
                {
                    StageId = sid,
                    BaseGain = 1,
                    Modifier = 1,
                    TotalGain = 0
                };
            }

            await _store.SaveAsync(Current, "main");
            return Current;
        }

        private static T Seed<T>(string name, Func<T> factory)
        {
            try
            {
                var sw = System.Diagnostics.Stopwatch.StartNew();
                var result = factory();
                sw.Stop();
                Console.WriteLine($"[StartService] {name} ok ({sw.ElapsedMilliseconds} ms)");
                return result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[StartService] ERRO em {name}: {ex.Message}\n{ex}");
                throw; // re-lança com o contexto original
            }
        }
    }
}
