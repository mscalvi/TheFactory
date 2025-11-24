using FurmaIdle.Models;

namespace FurmaIdle.Services
{
    public interface IOfflineService
    {
        Task OfflineIncome (double time);
    }
    public sealed class OfflineService : IOfflineService
    {
        private readonly ICurrentGameService _game;
        private readonly IIncomeService _income;
        private readonly IModifierService _modifiers;
        private readonly IUiLogService _log;
        private readonly ILocateService _locate;

        public OfflineService(ICurrentGameService game, IIncomeService income, IModifierService modifier, IUiLogService log, ILocateService locate)
        {
            _game = game;
            _income = income;
            _modifiers = modifier;
            _log = log;
            _locate = locate;
        }

        public async Task OfflineIncome (double time)
        {
            Dictionary<string, double> coinsToGenerate = new Dictionary<string, double> ();
            Dictionary<string, double> resourceToGenerate = new Dictionary<string, double>();

            foreach (var stage in _game.CurrentGame.Stages.Values)
            {
                if (stage.State != Helpers.UnlockHelper.State.Unlocked) continue;

                string coinId = stage.CoinId;

                var modifiers = _modifiers.GetModifiers(Helpers.ItemHelper.ItemType.Coin, coinId, stage.Id, Helpers.EffectHelper.EffectSupertype.Offline);

                var coinsGenerated = (stage.Expedition.CurrentCoinPerSec + modifiers.AddMod ) * modifiers.MultMod * time;

                coinsToGenerate.Add(coinId, coinsGenerated);

                foreach (var coin in coinsToGenerate)
                {
                    if (coin.Value > 1)
                    {
                        var coinInfo = _locate.LocateCoin(_game.CurrentGame, coin.Key);
                        _log.Info($"Ganhamos {coin.Value.ToString("N0")} {coinInfo.Name} enquanto estavamos distantes.");
                        await _income.AddAsync(Helpers.ItemHelper.ItemType.Coin, coin.Key, coin.Value, Helpers.ItemHelper.ItemType.Offline, stage.Id, stage.Id);
                    }
                }

                coinsToGenerate.Clear();
            }
        }
    }
}
