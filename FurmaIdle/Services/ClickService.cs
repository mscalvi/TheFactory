using FurmaIdle.Helpers;
using FurmaIdle.Models;
using Microsoft.AspNetCore.Components.Web;

namespace FurmaIdle.Services
{
    public interface IClickService
    {
        Task Click();
        int ClickGain { get; }
    }

    public sealed class ClickService : IClickService
    {
        private readonly ILocateService _locate;
        private readonly IIncomeService _income;
        private readonly ICurrentGameService _game;
        public ClickService(ILocateService locate, IIncomeService income, ICurrentGameService game)
        {
            _locate = locate;
            _income = income;
            _game = game;
        }

        public int ClickGain { get; private set; } = 1;

        public async Task Click()
        {
            var game = _game.CurrentGame;
            var stageId = game?.SelectedStageId;
            var stage = _locate.LocateStage(game, stageId);

            var ClickTotal = GetClickTotal();

            var gain = await _income.AddAsync(ItemHelper.ItemType.Coin, stage.CoinId, ClickTotal, ItemHelper.ItemType.Click, stage.ClickId);
            
            ClickGain = gain.GainEffective;
        }

        private double GetClickTotal()
        {
            var game = _game.CurrentGame;
            var click = _locate.LocateStageClick(game, game.SelectedStageId);

            var modifier = GetModifier(click);

            var baseGain = click.BaseGain;
            var addMod = modifier.AddMod;
            var multMod = modifier.MultMod <= 0 ? 1.0 : modifier.MultMod;

            return (baseGain + addMod) * multMod;
        }

        private (double AddMod, double MultMod) GetModifier(ClickModel click)
        {
            double AddMod = 0;
            double MultMod = 1;

            foreach (var modifier in click.Modifiers)
            {
                if (modifier.Type == EffectHelper.EffectType.ClickGain)
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
