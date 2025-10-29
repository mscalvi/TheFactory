using FurmaIdle.Helpers;
using FurmaIdle.Models;
using static FurmaIdle.Helpers.ItemHelper;

namespace FurmaIdle.Services
{
    public interface ISpecialtiesService
    {
        void TickSpecialties(GameModel game, double dtSeconds);
        (double Actual, double Total) GetSpecialtyTimer(SpecialtyModel spec);
        void ActivateSpecialtyTimer(string specialtyId, double durationSec, string stageId);
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
        private readonly Dictionary<string, (DateTimeOffset endsAt, double totalSec, string stageId)> _specTimers
            = new(StringComparer.Ordinal);

        public void ActivateSpecialtyTimer(string specialtyId, double durationSec, string stageId)
        {
            if (string.IsNullOrWhiteSpace(specialtyId)) return;

            var now = DateTimeOffset.UtcNow;
            var dur = Math.Max(0.001, durationSec);
            _specTimers[specialtyId] = (now.AddSeconds(dur), dur, stageId);
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

        private void DecreaseSpec(GameModel game)
        {
            if (_specTimers.Count == 0) return;

            var now = DateTimeOffset.UtcNow;

            var expired = _specTimers
                .Where(kvp => kvp.Value.endsAt <= now)
                .Select(kvp => kvp.Key)
                .ToList();

            if (expired.Count == 0) return;

            foreach (var specId in expired)
            {
                _specTimers.TryGetValue(specId, out var spec);

                RemoveAllSpecModifiers(game, specId, spec.stageId);
                _specTimers.Remove(specId);
            }
        }

        private static void Scrub(List<ModifierModel> list, string specId)
        {
            list.RemoveAll(m =>
                m.Scope == UnlockHelper.Persistence.untilTimer &&
                m.ApplyerId == specId
            );
        }

        public void RemoveAllSpecModifiers(GameModel game, string specId, string stageId)
        {
            // localizar qual specialty expirou
            var spec = _locate.LocateSpecialty(game, specId);
            if (spec is null) return;

            // isso é igual ao que você faz na hora de aplicar
            string targetTypeId = spec.TargetId.Length >= 2
                ? spec.TargetId.Substring(0, 1)
                : spec.TargetId;

            switch (targetTypeId)
            {
                case "a": // All of a kind
                    if (spec.TargetId == "aContracts")
                    {
                        foreach (var c in game.Contracts.Values)
                            Scrub(c.Modifiers, specId);
                    }
                    if (spec.TargetId == "aKnowledges")
                    {
                        foreach (var k in game.Knowledges.Values)
                            Scrub(k.Modifiers, specId);
                    }
                    if (spec.TargetId == "aCoins")
                    {
                        foreach (var c in game.Coins.Values)
                            Scrub(c.Modifiers, specId);
                    }
                    if (spec.TargetId == "aResources")
                    {
                        foreach (var r in game.Resources.Values)
                            Scrub(r.Modifiers, specId);
                    }
                    if (spec.TargetId == "aClicks")
                    {
                        foreach (var cl in game.Clicks.Values)
                            Scrub(cl.Modifiers, specId);
                    }
                    if (spec.TargetId == "aCharacters")
                    {
                        foreach (var ch in game.Characters.Values)
                            Scrub(ch.Modifiers, specId);
                    }
                    if (spec.TargetId == "aUpgrades")
                    {
                        foreach (var up in game.Upgrades.Values)
                            Scrub(up.Modifiers, specId);
                    }
                    break;

                case "m": // moeda única
                    {
                        var coin = _locate.LocateCoin(game, spec.TargetId);
                        if (coin != null) Scrub(coin.Modifiers, specId);
                        break;
                    }

                case "p": // personagem único
                    {
                        var ch = _locate.LocateCharacter(game, spec.TargetId);
                        if (ch != null) Scrub(ch.Modifiers, specId);
                        break;
                    }

                case "k": // knowledge único
                    {
                        var know = _locate.LocateKnowledge(game, spec.TargetId);
                        if (know != null) Scrub(know.Modifiers, specId);
                        break;
                    }

                case "t": // tech
                    {
                        var tech = _locate.LocateTech(game, spec.TargetId);
                        if (tech != null) Scrub(tech.Modifiers, specId);
                        break;
                    }

                case "u": // upgrade
                    {
                        var upg = _locate.LocateUpgrade(game, spec.TargetId);
                        if (upg != null) Scrub(upg.Modifiers, specId);
                        break;
                    }

                case "l": // local
                    {
                        var loc = _locate.LocateLocal(game, spec.TargetId);
                        if (loc != null) Scrub(loc.Modifiers, specId);
                        break;
                    }

                case "s": // stage
                    {
                        var stg = _locate.LocateStage(game, spec.TargetId);
                        if (stg != null) Scrub(stg.Modifiers, specId);
                        break;
                    }

                case "x": // expansion
                    {
                        var expa = _locate.LocateExpansion(game, spec.TargetId);
                        if (expa != null) Scrub(expa.Modifiers, specId);
                        break;
                    }

                case "d": // expedition
                    {
                        var exped = _locate.LocateExpedition(game, spec.TargetId);
                        if (exped != null) Scrub(exped.Modifiers, specId);
                        break;
                    }

                case "o": // trait
                    {
                        var trait = _locate.LocateTrait(game, spec.TargetId);
                        if (trait != null) Scrub(trait.Modifiers, specId);
                        break;
                    }

                case "e": // specialty alvo
                    {
                        var targetSpec = _locate.LocateSpecialty(game, spec.TargetId);
                        if (targetSpec != null) Scrub(targetSpec.Modifiers, specId);
                        break;
                    }

                case "c": // contract único
                    {
                        var contract = _locate.LocateContract(game, spec.TargetId);
                        if (contract != null) Scrub(contract.Modifiers, specId);
                        break;
                    }

                case "i": // click
                    {
                        var click = _locate.LocateStageClick(game, spec.TargetId);
                        if (click != null) Scrub(click.Modifiers, specId);
                        break;
                    }

                case "r": // resource único
                    {
                        var res = _locate.LocateResource(game, spec.TargetId);
                        if (res != null) Scrub(res.Modifiers, specId);
                        break;
                    }

                case "z":
                    var stage = _locate.LocateStage(game, stageId);
                    if (spec.TargetId == "zContracts")
                    {
                        foreach (var contractId in stage.ActiveContracts)
                        {
                            var contract = _locate.LocateContract(game, contractId.Key);
                            Scrub(contract.Modifiers, specId);
                        }
                    }
                    if (spec.TargetId == "zCharacters")
                    {
                        foreach (var characterId in stage.Expedition.PartyIds)
                        {
                            var character = _locate.LocateCharacter(game, characterId);
                            Scrub(character.Modifiers, specId);
                        }
                    }
                    break;
            }
        }
    }

}
