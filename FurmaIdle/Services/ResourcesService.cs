using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Diagnostics.Contracts;
using static FurmaIdle.Helpers.ItemHelper;

namespace FurmaIdle.Services
{
    public interface IResourcesService
    {
        void TickResources(GameModel game, double dtSeconds);
        (double rsRegen, long rsCap) GetResourceInfo(GameModel game, string resourceId);
    }

    public sealed class ResourcesTickSink : ITickSink, IDisposable
    {
        private readonly ITickService _ticks;
        private readonly IResourcesService _resources;

        public ResourcesTickSink(ITickService ticks, IResourcesService resources)
        {
            _ticks = ticks;
            _resources = resources;
            _ticks.Subscribe(this);
        }

        public void OnTick(GameModel game, double dtSeconds)
        {
            _resources.TickResources(game, dtSeconds);
        }

        public void Dispose() => _ticks.Unsubscribe(this);
    }

    public sealed class ResourcesService : IResourcesService
    {
        private readonly IIncomeService _income;
        private readonly ILocateService _locate;
        private readonly ICurrentGameService _game;

        public ResourcesService(IIncomeService income, ILocateService locate, ICurrentGameService game)
        {
            _income = income;
            _locate = locate;
            _game = game;
        }

        // acumula tempo para processar de 1 em 1 segundo
        private double _acc;
        
        public void TickResources(GameModel game, double dtSeconds)
        {
            if (game is null || dtSeconds <= 0) return;

            _acc += dtSeconds;
            if (_acc < 1.0) return;
            var steps = (int)Math.Floor(_acc);
            _acc -= steps;

            for (int s = 0; s < steps; s++)
                RegenOnce(game);
        }

        private void RegenOnce(GameModel game)
        {
            foreach (var res in game.Resources.Values)
            {
                if (res is null || res.State != UnlockHelper.State.Unlocked) continue;

                var resource = GetResourceInfo(game, res.Id);

                if (resource.rsRegen <= 0) continue;

                long current = 0;
                foreach (var stage in game.Stages)
                {
                    if (stage.Value.ExpeditionStats.Resources.TryGetValue(res.Id, out var cur) == true)
                        current += cur;
                }

                if (resource.rsCap > 0 && current >= resource.rsCap) continue;

                var room = resource.rsCap > 0 ? Math.Max(0, resource.rsCap - current) : long.MaxValue;
                var amount = Math.Min(room, resource.rsRegen);

                if (amount > 0)
                {
                    _ = _income.AddAsync(ItemType.Resource, res.Id, amount, sourceType: null, sourceId: null, null);
                }
            }
        }

        public (double rsRegen, long rsCap) GetResourceInfo(GameModel game, string resourceId)
        {
            var resource = _locate.LocateResource(game, resourceId);

            var regenModifier = GetModifiers(resource, EffectHelper.EffectType.ResourceGain);

            var capModifier = GetModifiers(resource, EffectHelper.EffectType.ResourceCap);

            var regen = (resource.RsPerSecond + regenModifier.AddMod) * regenModifier.MultMod;
            if (regen < 0) regen = 0;

            long baseCap = 0;

            foreach(var character in game.Characters)
            {
                if (character.Value.State == UnlockHelper.State.Unlocked)
                {
                    baseCap += resource.RsPerChar;
                }
            }

            long cap = (long)((baseCap + capModifier.AddMod) * capModifier.MultMod);

            return (regen, cap);
        }

        private static (double AddMod, double MultMod) GetModifiers(ResourceModel resource, EffectHelper.EffectType type)
        {
            double AddMod = 0;
            double MultMod = 1;

            foreach (var modifier in resource.Modifiers)
            {
                if (type == modifier.Type)
                {
                    if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                    {
                        AddMod += modifier.Value;
                    }
                    if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                    {
                        MultMod *= modifier.Value;
                    }
                }
            }

            return (AddMod, MultMod);
        }

    }
}
