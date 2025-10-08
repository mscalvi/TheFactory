using FurmaIdle.Data;
using FurmaIdle.Models;
using FurmaIdle.Enums;
using System;
using static FurmaIdle.Models.CharacterModel;

namespace FurmaIdle.Services
{
    public interface IGameService
    {
        GameModel Current { get; }
        void Attach(GameModel model);
        ResourceModel? Get(string id);
        void Add(string id, double amount = 1);
        void Click(string stageId);
        bool UnlockCharacter(string charId);
        bool SendToStage(string charId, string stageId);
        bool ReturnToBase(string charId);

        event Action? Changed;
    }

    public sealed class GameService : IGameService
    {
        private readonly IUpgradeService _effects;

        public GameService(IUpgradeService effects)
        {
            _effects = effects ?? throw new ArgumentNullException(nameof(effects));
        }

        public GameModel Current { get; private set; } = new();
        public event Action? Changed;

        public void Attach(GameModel model)
        {
            Current = model ?? throw new ArgumentNullException(nameof(model));
            Changed?.Invoke();
        }

        // ---------- Resources ----------
        public ResourceModel? Get(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || Current.Resources is null) return null;
            return Current.Resources.TryGetValue(id, out var r) ? r : null;
        }

        private ResourceModel EnsureResource(string id)
        {
            if (Current.Resources is null) Current.Resources = new();
            if (!Current.Resources.TryGetValue(id, out var r))
            {
                r = new ResourceModel { Id = id, Actual = 0, Total = 0 };
                Current.Resources[id] = r;
            }
            return r;
        }

        public void Add(string id, double amount = 1)
        {
            if (string.IsNullOrWhiteSpace(id) || amount == 0) return;

            var r = EnsureResource(id);
            r.Actual += amount;
            if (amount > 0) r.Total += amount;

            Changed?.Invoke();
        }

        // ---------- Clicks ----------
        public void Click(string stageId)
        {
            if (string.IsNullOrWhiteSpace(stageId) || Current.Clicks is null) return;
            if (!Current.Clicks.TryGetValue(stageId, out var click)) return;

            var gain = click.BaseGain * click.Modifier;
            if (!(gain > 0) || double.IsNaN(gain) || double.IsInfinity(gain)) return;

            var resId = StageData.GetResourceId(stageId);
            Add(resId, gain);
            click.TotalGain += gain;

            Changed?.Invoke();
        }

        // ---------- Characters ----------
        public bool UnlockCharacter(string charId)
        {
            if (!Current.Characters.TryGetValue(charId, out var character)) return false;
            if (character.CharState == CharStateEnum.CharState.Locked)
            {
                character.CharState = CharStateEnum.CharState.InBase;
                character.CharDestId = null;
                Changed?.Invoke();
                return true;
            }
            return false;
        }

        public bool SendToStage(string charId, string stageId)
        {
            if (!Current.Characters.TryGetValue(charId, out var character)) return false;
            if (character.CharState == CharStateEnum.CharState.Locked) return false;

            character.CharState = CharStateEnum.CharState.OnStage;
            character.CharDestId = stageId;
            Changed?.Invoke();
            return true;
        }

        public bool ReturnToBase(string charId)
        {
            if (!Current.Characters.TryGetValue(charId, out var character)) return false;
            if (character.CharState == CharStateEnum.CharState.Locked) return false;

            character.CharState = CharStateEnum.CharState.InBase;
            character.CharDestId = null;
            Changed?.Invoke();
            return true;
        }

        // ---------- Upgrades / Effects ----------
        public double ComputeContractTick(string contractId, double baseGainPerTick)
        {
            var mult = _effects.GainMult(contractId);
            return baseGainPerTick * mult;
        }

        public double ComputeContractTime(string contractId, double baseSeconds)
        {
            var mult = _effects.TimeMult(contractId);
            return baseSeconds * mult;
        }
    }
}
