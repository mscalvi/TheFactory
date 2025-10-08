using FurmaIdle.Data;
using FurmaIdle.Models;
using FurmaIdle.Storage;
using System.Linq;
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
                Stages = StageData.CreateInitialStages(),
                Destinations = DestinationData.CreateInitialDestinations(),
                Technologies = TechData.CreateInitialTechs(),
                Upgrades = UpgradeData.CreateInitialUpgrades(),
                Resources = ResourceData.CreateInitialResources(),
                Characters = CharacterData.CreateInitialStates(),
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
    }
}
