using FurmaIdle.Helpers;
using FurmaIdle.Models;
using Microsoft.AspNetCore.Components.Web;
using System.Diagnostics.Contracts;

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
        private readonly IKnowledgeService _knowledge;
        public ClickService(ILocateService locate, IIncomeService income, ICurrentGameService game, IModifierService modifier, IKnowledgeService knowledge)
        {
            _locate = locate;
            _income = income;
            _game = game;
            _modifier = modifier;
            _knowledge = knowledge;
        }

        public int ClickGain { get; private set; } = 1;

        public async Task Click()
        {
            var game = _game.CurrentGame;
            var stageId = game?.SelectedStageId;
            var stage = _locate.LocateStage(game, stageId);
            var click = _locate.LocateStageClick(game, stage.Id);

            var modifier = _modifier.GetModifiers(ItemHelper.ItemType.Click, click.Id, stage.Id, EffectHelper.EffectSupertype.Gain);

            var expansion = _locate.LocateExpansion(_game.CurrentGame, _game.CurrentGame.CurrentExpansionId);

            var knowBurst = _knowledge.GetKnowledgeBurst(_game.CurrentGame, stage.CoinId, expansion);

            double ClickTotal = (click.BaseGain + modifier.AddMod) * modifier.MultMod * knowBurst;

            var gain = await _income.AddAsync(ItemHelper.ItemType.Coin, stage.CoinId, ClickTotal, ItemHelper.ItemType.Click, stage.ClickId, stage.Id);
            
            ClickGain = gain.GainEffective;
        }
    }
}
