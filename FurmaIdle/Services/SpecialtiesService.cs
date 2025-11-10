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

                    string targetTypeId = spec.TargetId.Length >= 2
                        ? spec.TargetId.Substring(0, 1)
                        : spec.TargetId;
                    switch (targetTypeId)
                    {
                        case "a": // All of a Kind
                            if (spec.TargetId == "aContracts")
                            {
                                foreach (var it in g.Contracts.Values)
                                    Scrub(it.Modifiers, specId);
                            }
                            if (spec.TargetId == "aKnowledges")
                            {
                                foreach (var it in g.Knowledges.Values)
                                    Scrub(it.Modifiers, specId);
                            }
                            if (spec.TargetId == "aCoins")
                            {
                                foreach (var it in g.Coins.Values)
                                    Scrub(it.Modifiers, specId);
                            }
                            if (spec.TargetId == "aResources")
                            {
                                foreach (var it in g.Resources.Values)
                                    Scrub(it.Modifiers, specId);
                            }
                            if (spec.TargetId == "aClicks")
                            {
                                foreach (var it in g.Clicks.Values)
                                    Scrub(it.Modifiers, specId);
                            }
                            if (spec.TargetId == "aCharacters")
                            {
                                foreach (var it in g.Characters.Values)
                                    Scrub(it.Modifiers, specId);
                            }
                            if (spec.TargetId == "aUpgrades")
                            {
                                foreach (var it in g.Upgrades.Values)
                                    Scrub(it.Modifiers, specId);
                            }
                            if (spec.TargetId == "aSpecialties")
                            {
                                foreach (var it in g.Specialties.Values)
                                    Scrub(it.Modifiers, specId);
                            }
                            break;
                        case "m": // Coins
                            var coins = _locate.LocateCoin(game, spec.TargetId);

                            Scrub(coins.Modifiers, specId);
                            break;
                        case "p": // Characters
                            var character = _locate.LocateCharacter(game, spec.TargetId);

                            Scrub(character.Modifiers, specId);
                            break;
                        case "k": // Knowledge
                            var knowledge = _locate.LocateKnowledge(game, spec.TargetId);

                            Scrub(knowledge.Modifiers, specId);
                            break;
                        case "t": // Techs
                            var tech = _locate.LocateTech(game, spec.TargetId);

                            Scrub(tech.Modifiers, specId);
                            break;
                        case "u": // Upgrades
                            var targetupgrade = _locate.LocateUpgrade(game, spec.TargetId);

                            Scrub(targetupgrade.Modifiers, specId);
                            break;
                        case "l": // Locals
                            var local = _locate.LocateLocal(game, spec.TargetId);

                            Scrub(local.Modifiers, specId);
                            break;
                        case "s": // Stages
                            var targetstage = _locate.LocateStage(game, spec.TargetId);

                            Scrub(targetstage.Modifiers, specId);
                            break;
                        case "x": // Expansions
                            var expansion = _locate.LocateExpansion(game, spec.TargetId);

                            Scrub(expansion.Modifiers, specId);
                            break;
                        case "d": // Expeditions
                            var expedition = _locate.LocateExpedition(game, spec.TargetId);

                            Scrub(expedition.Modifiers, specId);
                            break;
                        case "o": // Traits
                            var trait = _locate.LocateTrait(game, spec.TargetId);

                            Scrub(trait.Modifiers, specId);
                            break;
                        case "e": // Specialty
                            var speciality = _locate.LocateSpecialty(game, spec.TargetId);

                            Scrub(speciality.Modifiers, specId);
                            break;
                        case "c": // Contracts
                            var contract = _locate.LocateContract(game, spec.TargetId);

                            Scrub(contract.Modifiers, specId);
                            break;
                        case "i": // Clicks
                            var click = _locate.LocateClick(game, spec.TargetId);

                            Scrub(click.Modifiers, specId);
                            break;
                        case "r": // Resources
                            var resource = _locate.LocateResource(game, spec.TargetId);

                            Scrub(resource.Modifiers, specId);
                            break;
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
