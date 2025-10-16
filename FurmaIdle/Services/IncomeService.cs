using System.Collections.Concurrent;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using Microsoft.Extensions.Logging;

namespace FurmaIdle.Services
{
    public interface IIncomeService
    {
        /// <summary>
        /// Soma amount (pode ter fração) para (type,itemId).
        /// Aplica apenas a parte inteira no save; fração fica em memória.
        /// </summary>
        Task<GainModel> AddAsync(ItemHelper.ItemType type, string itemId, double amount);
    }

    public sealed class IncomeService : IIncomeService
    {
        private readonly ILogger<IncomeService> _log;
        private readonly ICurrentGameService _game;

        // frações por chave (type:id) — só em memória
        private readonly ConcurrentDictionary<string, double> _fractions = new();

        public IncomeService(ILogger<IncomeService> log, ICurrentGameService game)
        {
            _log = log;
            _game = game;
        }

        public async Task<GainModel> AddAsync(ItemHelper.ItemType type, string itemId, double amount)
        {
            if (string.IsNullOrWhiteSpace(itemId))
                throw new ArgumentException("itemId inválido.", nameof(itemId));
            if (double.IsNaN(amount) || double.IsInfinity(amount))
                throw new ArgumentOutOfRangeException(nameof(amount), "amount inválido");

            var key = Key(type, itemId);

            // 1) acumula fração em memória
            var startFrac = _fractions.GetOrAdd(key, 0.0);
            var total = startFrac + amount;

            var eff = (long)Math.Floor(total);  // parte inteira efetiva (use long p/ evitar overflow cedo)
            var frac = total - eff;             // resto fracionário

            GainModel? result = null;

            // 2) aplica somente a parte inteira no save (atuais + lifetime)
            await _game.Mutate(g =>
            {
                EnsureStats(g);

                if (eff != 0)
                {
                    if (!TryApplyEffectiveToStats(g, type, itemId, eff))
                    {
                        // rollback mental: nada foi aplicado ao save
                        _log.LogWarning("[Income] Falha ao aplicar ganho: type={Type} id={Id} eff={Eff}", type, itemId, eff);
                        eff = 0;
                        frac = startFrac;
                    }
                }

                _fractions[key] = frac;

                result = new GainModel
                {
                    ItemId = itemId,
                    ItemType = type,
                    GainEffective = (int)Math.Clamp(eff, int.MinValue, int.MaxValue),
                    GainTotal = amount,
                    GainFraction = frac
                };
            }, save: eff != 0);

            return result!;
        }

        private static string Key(ItemHelper.ItemType t, string id) => $"{(int)t}:{id}";

        private static void EnsureStats(GameModel g)
        {
            g.Stats ??= new StatsModel();

            g.Stats.Coins ??= new Dictionary<string, long>(StringComparer.Ordinal);
            g.Stats.Resources ??= new Dictionary<string, long>(StringComparer.Ordinal);
            g.Stats.Knowledge ??= new Dictionary<string, long>(StringComparer.Ordinal);

            g.Stats.CoinsGain ??= new Dictionary<string, long>(StringComparer.Ordinal);
            g.Stats.ResourcesGain ??= new Dictionary<string, long>(StringComparer.Ordinal);
            g.Stats.KnowledgeGain ??= new Dictionary<string, long>(StringComparer.Ordinal);

            g.Stats.CoinsSpent ??= new Dictionary<string, long>(StringComparer.Ordinal);
            g.Stats.ResourcesSpent ??= new Dictionary<string, long>(StringComparer.Ordinal);
            g.Stats.KnowledgeSpent ??= new Dictionary<string, long>(StringComparer.Ordinal);
        }

        private static bool TryApplyEffectiveToStats(GameModel g, ItemHelper.ItemType type, string id, long eff)
        {
            // escolhe os dics corretos por tipo
            (Dictionary<string, long>? current, Dictionary<string, long>? lifetime) = type switch
            {
                ItemHelper.ItemType.Coin => (g.Stats!.Coins, g.Stats!.CoinsGain),
                ItemHelper.ItemType.Resource => (g.Stats!.Resources, g.Stats!.ResourcesGain),
                ItemHelper.ItemType.Knowledge => (g.Stats!.Knowledge, g.Stats!.KnowledgeGain),
                _ => (null, null),
            };

            if (current is null || lifetime is null) return false;

            // atuais
            current.TryGetValue(id, out var cur);
            current[id] = cur + eff;

            // lifetime (gerados)
            lifetime.TryGetValue(id, out var curGain);
            lifetime[id] = curGain + eff;

            return true;
        }
    }
}
