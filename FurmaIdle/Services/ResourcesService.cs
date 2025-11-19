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
        private readonly IModifierService _modifier;

        public ResourcesService(IIncomeService income, ILocateService locate, ICurrentGameService game, IModifierService modifier)
        {
            _income = income;
            _locate = locate;
            _game = game;
            _modifier = modifier;
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

                var expansion = _locate.LocateExpansion(game, game.CurrentExpansionId);

                current = expansion.ExpansionStats.Resources.GetValueOrDefault(res.Id);

                if (resource.rsCap > 0 && current >= resource.rsCap) continue;

                var room = resource.rsCap > 0 ? Math.Max(0, resource.rsCap - current) : long.MaxValue;
                var amount = Math.Min(room, resource.rsRegen);

                if (amount > 0)
                {
                    _ = _income.AddAsync(ItemType.Resource, res.Id, amount, sourceType: null, sourceId: null, "s01");
                }
            }
        }

        public (double rsRegen, long rsCap) GetResourceInfo(GameModel game, string resourceId)
        {
            var resource = _locate.LocateResource(game, resourceId);

            var regenModifier = _modifier.GetModifiers(ItemType.Resource, resourceId, "s01", EffectHelper.EffectSupertype.Gain);

            var capModifier = GetResourceCap(resource);

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

            resource.RegenActual = regen;

            return (regen, cap);
        }

        private static (double AddMod, double MultMod) GetResourceCap(ResourceModel resource)
        {
            double AddMod = 0;
            double MultMod = 1;

            foreach (var modifier in resource.Modifiers)
            {
                if (modifier.Type == EffectHelper.EffectType.ResourceCap)
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
