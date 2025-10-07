using FurmaIdle.Data;
using FurmaIdle.Models;
using System;

namespace FurmaIdle.Services
{
    public interface IGameService
    {
        GameModel Current { get; }
        void Attach(GameModel model);
        ResourceModel? Get(string id);
        void Add(string id, double amount = 1);
        void Click(string stageId);
        event Action? Changed;
    }

    public sealed class GameService : IGameService
    {
        public GameModel Current { get; private set; } = new();
        public event Action? Changed;

        public void Attach(GameModel model)
        {
            Current = model ?? throw new ArgumentNullException(nameof(model));
            Changed?.Invoke();
        }

        public ResourceModel? Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            return Current.Resources.TryGetValue(id, out var r) ? r : null;
        }

        public void Add(string id, double amount = 1)
        {
            if (amount == 0) return;
            var r = Get(id);
            if (r is null) return;

            r.Actual += amount;
            if (amount > 0) r.Total += amount; // se usar Total como acumulado
            Changed?.Invoke();
        }
        private string StageToResource(string stageId) => "r01";

        public void Click(string stageId)
        {
            if (string.IsNullOrWhiteSpace(stageId)) return;
            if (!Current.Clicks.TryGetValue(stageId, out var click)) return;

            var gain = click.BaseGain * click.Modifier;
            if (gain <= 0 || double.IsNaN(gain) || double.IsInfinity(gain)) return;

            var resId = StageData.GetResourceId(stageId);
            Add(resId, gain);
            click.TotalGain += gain;
        }
    }
}
