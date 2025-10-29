using FurmaIdle.Helpers;
using FurmaIdle.Models;
using static FurmaIdle.Helpers.ItemHelper;

namespace FurmaIdle.Services
{
    public interface ISpecialtiesService
    {
        void TickSpecialties(GameModel game, double dtSeconds);
        (double Actual, double Total) GetSpecialtyTimer(SpecialtyModel spec);
        void ActivateSpecialtyTimer(string specialtyId, double durationSec);
    }

    public sealed class SpecialtiesTickSink : ITickSink, IDisposable
    {
        private readonly ITickService _ticks;
        private readonly ISpecialtiesService _specialties;

        public SpecialtiesTickSink(ITickService ticks, ISpecialtiesService specialties)
        {
            _ticks = ticks;
            _specialties = specialties;
            _ticks.Subscribe(this);
        }

        public void OnTick(GameModel game, double dtSeconds)
        {
            _specialties.TickSpecialties(game, dtSeconds);
        }

        public void Dispose() => _ticks.Unsubscribe(this);
    }

    public sealed class SpecialtiesService : ISpecialtiesService
    {
        private readonly ILocateService _locate;
        private readonly ICurrentGameService _game;

        public SpecialtiesService(ILocateService locate, ICurrentGameService game)
        {
            _locate = locate;
            _game = game;
        }

        private double _acc;

        // Timers de Specialties: specialtyId -> (EndsAt, TotalSec)
        private readonly Dictionary<string, (DateTimeOffset endsAt, double totalSec)> _specTimers
            = new(StringComparer.Ordinal);

        public void ActivateSpecialtyTimer(string specialtyId, double durationSec)
        {
            if (string.IsNullOrWhiteSpace(specialtyId)) return;
            var now = DateTimeOffset.UtcNow;
            var dur = Math.Max(0.001, durationSec);
            _specTimers[specialtyId] = (now.AddSeconds(dur), dur);
        }

        // Specialties
        private (double Actual, double Total) GetSpecialtyTimer(string specialtyId)
        {
            if (string.IsNullOrWhiteSpace(specialtyId)) return (0, 0);

            if (_specTimers.TryGetValue(specialtyId, out var t))
            {
                var remaining = (t.endsAt - DateTimeOffset.UtcNow).TotalSeconds;
                if (remaining <= 0)
                {
                    _specTimers.Remove(specialtyId);
                    return (0, t.totalSec);
                }
                return (remaining, t.totalSec);
            }
            return (0, 0);
        }
        public (double Actual, double Total) GetSpecialtyTimer(SpecialtyModel spec)
            => spec is null ? (0, 0) : GetSpecialtyTimer(spec.Id);

        public void TickSpecialties(GameModel game, double dtSeconds)
        {
            if (game is null || dtSeconds <= 0) return;

            _acc += dtSeconds;
            if (_acc < 1.0) return;
            var steps = (int)Math.Floor(_acc);
            _acc -= steps;

            for (int s = 0; s < steps; s++)
                DecreaseSpec(game);
        }

        private void DecreaseSpec(GameModel game)
        {
            if (_specTimers.Count == 0) return;

            var now = DateTimeOffset.UtcNow;
            // pegue uma cópia das chaves que expiraram
            var expired = _specTimers
                .Where(kvp => kvp.Value.endsAt <= now)
                .Select(kvp => kvp.Key)
                .ToList();

            if (expired.Count == 0) return;

            foreach (var specId in expired)
            {
                RemoveAllSpecModifiers(game, specId);
                _specTimers.Remove(specId);
            }
        }

        public void RemoveAllSpecModifiers(GameModel game, string specId)
        {
            void CleanList(List<ModifierModel> list, string specIdToRemove)
            {
                list.RemoveAll(m =>
                    m.ApplyerId == specIdToRemove &&
                    m.Scope == UnlockHelper.Persistence.untilTimer
                );
            }

            var spec = _locate.LocateSpecialty(game, specId);

            if(spec.TargetId == "zSpecialties")
            {
                foreach (var it in game.Specialties.Values) CleanList(it.Modifiers, specId);
            }
            if(spec.TargetId == "zContracts")
            {
                foreach (var it in game.Contracts.Values) CleanList(it.Modifiers, specId);
            }
            if (spec.TargetId == "aResources")
            {
                foreach (var it in game.Resources.Values) CleanList(it.Modifiers, specId);
            }
        }

    }

}
