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
        private readonly IModifierService _modifier;
        public ClickService(ILocateService locate, IIncomeService income, ICurrentGameService game, IModifierService modifier)
        {
            _locate = locate;
            _income = income;
            _game = game;
            _modifier = modifier;
        }

        public int ClickGain { get; private set; } = 1;

        public async Task Click()
        {
            var game = _game.CurrentGame;
            var stageId = game?.SelectedStageId;
            var stage = _locate.LocateStage(game, stageId);
            var click = _locate.LocateStageClick(game, stage.Id);

            var modifier = _modifier.GetModifiers(ItemHelper.ItemType.Click, click.Id, stage.Id, EffectHelper.EffectSupertype.Gain);

            double ClickTotal = (click.BaseGain + modifier.AddMod) * modifier.MultMod;

            var gain = await _income.AddAsync(ItemHelper.ItemType.Coin, stage.CoinId, ClickTotal, ItemHelper.ItemType.Click, stage.ClickId, stage.Id);
            
            ClickGain = gain.GainEffective;
        }
    }
}
