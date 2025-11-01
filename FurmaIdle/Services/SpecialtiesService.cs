using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Security.AccessControl;
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
        private readonly IUiLogService _log;

        public SpecialtiesService(ILocateService locate, ICurrentGameService game, IUiLogService log)
        {
            _locate = locate;
            _game = game;
            _log = log;
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

        private async void DecreaseSpec(GameModel game)
        {
            if (_specTimers.Count == 0) return;

            var now = DateTimeOffset.UtcNow;
            var expired = _specTimers
                .Where(kvp => kvp.Value.endsAt <= now)
                .Select(kvp => kvp.Key)
                .ToList();

            if (expired.Count == 0) return;

            await _game.Mutate(g =>
            {
                foreach (var specId in expired)
                {
                    var spec = _locate.LocateSpecialty(g, specId);
                    if (spec is null) continue;

                    if (spec.TargetId == "aSpecialties")
                    {
                        foreach (var it in g.Specialties.Values)
                            Scrub(it.Modifiers, specId);
                    }
                    else if (spec.TargetId == "aContracts")
                    {
                        foreach (var it in g.Contracts.Values)
                            Scrub(it.Modifiers, specId);
                    }
                    else if (spec.TargetId == "aResources")
                    {
                        foreach (var it in g.Resources.Values)
                            Scrub(it.Modifiers, specId);
                    }

                    _specTimers.Remove(specId);
                }

            }, save: true);
        }

        private static void Scrub(List<ModifierModel> list, string specId)
        {
            list.RemoveAll(m =>
                m.Scope == UnlockHelper.Persistence.untilTimer &&
                string.Equals(m.ApplyerId, specId, StringComparison.Ordinal));
        }
    }
}
