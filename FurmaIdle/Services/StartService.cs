// Services/StartService.cs
using System.Threading.Tasks;
using System.Linq;
using FurmaIdle.Data;
using FurmaIdle.Models;

namespace FurmaIdle.Services
{
    public interface IStartService
    {
        GameModel Current { get; }
        Task<GameModel> InitAsync();
    }

    public sealed class StartService : IStartService
    {
        public GameModel Current { get; private set; } = new();

        public Task<GameModel> InitAsync()
        {
            Current = new GameModel
            {
                Resources = ResourceData.CreateInitialResources(),
                Stages = StageData.CreateInitialStages(),
                SchemaVersion = ResourceData.SchemaVersion
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


            return Task.FromResult(Current);
        }
    }
}
