using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Diagnostics.Contracts;
using static FurmaIdle.Helpers.ItemHelper;

namespace FurmaIdle.Services
{
    public interface IResourcesService
    {
        void TickResources(GameModel game, double dtSeconds);
        double GetPerSecond(GameModel game, string resourceId);
        long GetCap(GameModel game, string resourceId);

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
            if (_acc < 1.0) return;          // só processa de 1 em 1 segundo
            var steps = (int)Math.Floor(_acc);
            _acc -= steps;

            // processa 'steps' segundos de uma vez (normalmente 1)
            for (int s = 0; s < steps; s++)
                RegenOnce(game);
        }

        private void RegenOnce(GameModel game)
        {
            foreach (var res in game.Resources.Values)
            {
                if (res is null || res.State != UnlockHelper.State.Unlocked) continue;

                var perSec = GetPerSecond(game, res.Id);
                if (perSec <= 0) continue;

                // cap atual (opcional, mas útil p/ não passar do máximo)
                var cap = GetCap(game, res.Id);

                // valor atual
                long current = 0;
                if (game.ExpeditionStats?.Resources?.TryGetValue(res.Id, out var cur) == true)
                    current = cur;

                // Se há cap (>0), limita o ganho
                if (cap > 0 && current >= cap) continue;

                var room = cap > 0 ? Math.Max(0, cap - current) : long.MaxValue;
                var amount = Math.Min(room, perSec);

                if (amount > 0)
                {
                    // credita usando IncomeService (ele já trata frações e acumula em Expedition/Expansion/GameStats)
                    _ = _income.AddAsync(ItemType.Resource, res.Id, amount, sourceType: null, sourceId: null);
                }
            }
        }

        public double GetPerSecond(GameModel game, string resourceId)
        {
            if (string.IsNullOrWhiteSpace(resourceId)) return 0;
            if (!game.Resources.TryGetValue(resourceId, out var r) || r is null) return 0;

            var basePerSec = Math.Max(0, r.RsPerSecond);

            var modifier = GetModifiers(r, EffectHelper.EffectType.ResourceGain);

            var add = modifier.AddMod;
            var mult = modifier.MultMod <= 0 ? 1 : modifier.MultMod;

            var effective = (basePerSec + add) * mult;
            if (effective < 0) effective = 0;

            return effective;
        }

        public long GetCap(GameModel game, string resourceId)
        {
            if (string.IsNullOrWhiteSpace(resourceId)) return 0;
            if (!game.Resources.TryGetValue(resourceId, out var r) || r is null) return 0;

            var perChar = Math.Max(0, r.RsPerChar);
            if (perChar <= 0) return 0;

            var unlockedChars = game.Characters.Values.Count(c => c is not null && c.State == UnlockHelper.State.Unlocked);
            return unlockedChars > 0 ? perChar * unlockedChars : 0;
        }

        public static (double AddMod, double MultMod) GetModifiers(ResourceModel resource, EffectHelper.EffectType type)
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
                        AddMod *= modifier.Value;
                    }
                }
            }

            return (AddMod, MultMod);
        }

    }
}
