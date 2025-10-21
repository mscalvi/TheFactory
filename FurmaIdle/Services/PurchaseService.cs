using FurmaIdle.Data;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using static FurmaIdle.Services.PurchaseService;

namespace FurmaIdle.Services
{
    public interface IPurchaseService
    {
        Task<PurchaseResult> Purchase(ItemHelper.ItemType type, string itemId);

        bool CanAfford(ItemHelper.ItemType type, string itemId);
        IReadOnlyList<CostLine> PreviewCost(ItemHelper.ItemType kind, string itemId);
    }

    public sealed class PurchaseService : IPurchaseService
    {
        //Para ver o resultado, em outros lugares
        //var res = await _purchase.Purchase(kind, id, null);
        //if (res.Success)
        //{
        //var totalSpent = res.AppliedCosts!.Sum(c => (long)Math.Ceiling(c.Amount));
        //// ou agrupar por moeda:
        //var porMoeda = res.AppliedCosts!
        //    .GroupBy(c => (c.CurrencyGroup, c.CurrencyId))
        //    .ToDictionary(g => g.Key, g => g.Sum(c => (long)Math.Ceiling(c.Amount)));
        //}

        private readonly ICurrentGameService _game;
        private readonly IIncomeService _income;
        private readonly IUnlockService _unlock;
        private readonly ILocateService _locate;

        private readonly object _lock = new();

        public sealed record CostLine(string CurrencyGroup, string CurrencyId, double Amount);
        public sealed record PurchaseResult(bool Success, string? Reason, IReadOnlyList<CostLine>? AppliedCosts = null)
        {
            public static PurchaseResult Ok(IReadOnlyList<CostLine> applied) => new(true, null, applied);
            public static PurchaseResult Fail(string reason) => new(false, reason, null);
        }
        public PurchaseService(ICurrentGameService Game, IIncomeService Income, IUnlockService Unlock, ILocateService Locate)
        {
            _game = Game;
            _income = Income;
            _unlock = Unlock;
            _locate = Locate;
        }

        // Verify
        public async Task<PurchaseResult> Purchase(ItemHelper.ItemType type, string itemId)
        {
            var costs = PreviewCost(type, itemId);
            var m = _game.CurrentGame ?? throw new InvalidOperationException("Jogo não carregado.");
            var exp = m.ExpeditionStats ?? throw new InvalidOperationException("ExpeditionStats indisponível.");

            // 1) Validação prévia
            foreach (var c in costs)
            {
                if (!HasEnough(exp, c, out var why))
                    return PurchaseResult.Fail(why ?? "Saldo insuficiente.");
            }

            // 2) Transação atômica
            PurchaseResult result = PurchaseResult.Fail("Falha desconhecida.");

            lock (_lock)
            {
                // Observação: seu Mutate é síncrono no 'edit', mas assíncrono no Save. Vamos capturar o resultado local.
                // Revalidação leve no começo do bloco evita corrida/duplo clique.
            }

            await _game.Mutate(g =>
            {
                var expStats = g.ExpeditionStats ?? throw new InvalidOperationException("ExpeditionStats indisponível.");
                var expaStats = g.ExpansionStats ?? throw new InvalidOperationException("ExpansionStats indisponível.");
                var gameStats = g.GameStats ?? throw new InvalidOperationException("GameStats indisponível.");

                // Revalidação em-ato
                foreach (var c in costs)
                {
                    if (!HasEnough(expStats, c, out _))
                    {
                        result = PurchaseResult.Fail("Saldo insuficiente.");
                        return;
                    }
                }

                // Débito (Expedition) + acumulados (Expansion/Game)
                foreach (var c in costs)
                {
                    ApplyDebit(expStats, c);
                    ApplyStats(expaStats, gameStats, c);
                }

                // Unlock/ativação
                switch (type)
                {
                    case ItemHelper.ItemType.Specialty:
                        // Ativar Habilidade
                        result = PurchaseResult.Ok(costs);
                        return;

                    case ItemHelper.ItemType.Upgrade:
                        _unlock.UnlockUpgrade(itemId);
                        result = PurchaseResult.Ok(costs);
                        return;

                    case ItemHelper.ItemType.Tech:
                        _unlock.UnlockTech(itemId);
                        result = PurchaseResult.Ok(costs);
                        return;

                    case ItemHelper.ItemType.Expansion:
                        _unlock.UnlockExpansion(itemId);
                        result = PurchaseResult.Ok(costs);
                        return;

                    default:
                        result = PurchaseResult.Fail($"Tipo de compra não suportado: {type}");
                        return;
                }
            }, save: true);

            return result;
        }
        public bool CanAfford(ItemHelper.ItemType kind, string itemId)
        {
            var m = _game.CurrentGame ?? throw new InvalidOperationException("Jogo não carregado.");
            var s = m.ExpeditionStats ?? throw new InvalidOperationException("ExpeditionStats indisponível.");

            var costs = PreviewCost(kind, itemId);
            foreach (var c in costs)
            {
                if (!HasEnough(s, c, out var why))
                {
                    return false;
                }
            }
            return true;
        }

        // Purchase
        private static void ApplyDebit(StatsModel s, CostLine c)
        {
            var amtLong = (long)Math.Ceiling(c.Amount);

            switch (c.CurrencyGroup)
            {
                case "coin":
                    AddOrSet(s.Coins, c.CurrencyId, -amtLong);
                    AddOrSet(s.CoinsSpent, c.CurrencyId, +amtLong);
                    break;

                case "resource":
                    AddOrSet(s.Resources, c.CurrencyId, -amtLong);
                    AddOrSet(s.ResourcesSpent, c.CurrencyId, +amtLong);
                    break;

                case "knowledge":
                    AddOrSet(s.Knowledge, c.CurrencyId, -amtLong);
                    AddOrSet(s.KnowledgeSpent, c.CurrencyId, +amtLong);
                    break;

                default:
                    throw new InvalidOperationException($"Moeda desconhecida: {c.CurrencyGroup}");
            }
        }
        private static void ApplyStats(StatsModel expansion, StatsModel game, CostLine c)
        {
            var amt = (long)Math.Ceiling(c.Amount);
            switch (c.CurrencyGroup)
            {
                case "coin":
                    AddOrSet(expansion.CoinsSpent, c.CurrencyId, +amt);
                    AddOrSet(game.CoinsSpent, c.CurrencyId, +amt);
                    break;

                case "resource":
                    AddOrSet(expansion.ResourcesSpent, c.CurrencyId, +amt);
                    AddOrSet(game.ResourcesSpent, c.CurrencyId, +amt);
                    break;

                case "knowledge":
                    AddOrSet(expansion.KnowledgeSpent, c.CurrencyId, +amt);
                    AddOrSet(game.KnowledgeSpent, c.CurrencyId, +amt);
                    break;

                default:
                    throw new InvalidOperationException($"Moeda desconhecida: {c.CurrencyGroup}");
            }
        }

        // Helpers Internos
        public IReadOnlyList<CostLine> PreviewCost(ItemHelper.ItemType kind, string itemId)
        {
            if (string.IsNullOrWhiteSpace(itemId))
                throw new ArgumentException("itemId inválido.", nameof(itemId));

            switch (kind)
            {
                case ItemHelper.ItemType.Specialty:
                    {
                        var sp = _locate.LocateSpecialty(_game.CurrentGame, itemId);
                        ThrowIfNull(sp, $"Specialty '{itemId}' inexistente.");

                        var coinId = sp!.PricingId;
                        if (string.IsNullOrWhiteSpace(coinId))
                            throw new InvalidOperationException($"Specialty '{itemId}' sem PricingId.");

                        var group = InferCurrencyGroup(coinId);
                        var value = sp.Cost;
                        return new[] { new CostLine(group, coinId, value) };
                    }

                case ItemHelper.ItemType.Upgrade:
                    {
                        var up = _locate.LocateUpgrade(_game.CurrentGame, itemId);
                        ThrowIfNull(up, $"Upgrade '{itemId}' inexistente.");

                        var entry = PricingHelper.PricingCost.Get(up!.PricingId);

                        var coinId = entry.CostCoinId;
                        if (string.IsNullOrWhiteSpace(coinId))
                            throw new InvalidOperationException($"Pricing '{up.PricingId}' sem CoinId.");

                        var group = InferCurrencyGroup(coinId);
                        var value = entry.CostBase;
                        return new[] { new CostLine(group, coinId, value) };
                    }

                case ItemHelper.ItemType.Tech:
                    {
                        var tech = _locate.LocateTech(_game.CurrentGame, itemId);
                        ThrowIfNull(tech, $"Tech '{itemId}' inexistente.");

                        var entry = PricingHelper.PricingCost.Get(tech!.PricingId);

                        var coinId = entry.CostCoinId;
                        if (string.IsNullOrWhiteSpace(coinId))
                            throw new InvalidOperationException($"Pricing '{tech.PricingId}' sem CoinId.");

                        var group = InferCurrencyGroup(coinId);
                        var value = entry.CostBase;
                        return new[] { new CostLine(group, coinId, value) };
                    }

                case ItemHelper.ItemType.Contract:
                    {
                        var contract = _locate.LocateContract(_game.CurrentGame, itemId);
                        ThrowIfNull(contract, $"Contract '{itemId}' inexistente.");

                        var entry = PricingHelper.PricingCost.Get(contract!.PricingId);

                        var coinId = entry.CostCoinId;
                        if (string.IsNullOrWhiteSpace(coinId))
                            throw new InvalidOperationException($"Pricing '{contract.PricingId}' sem CoinId.");

                        var group = InferCurrencyGroup(coinId);
                        var value = entry.CostBase;
                        return new[] { new CostLine(group, coinId, value) };
                    }

                default:
                    throw new NotSupportedException($"PreviewCost não implementado para '{kind}'.");
            }
        }
        private static void ThrowIfNull<T>(T obj, string message) where T : class
        {
            if (obj is null) throw new InvalidOperationException(message);
        }

        private static bool HasEnough(StatsModel s, CostLine c, out string? reason)
        {
            reason = null;

            // Normaliza: custo sempre arredondado para cima
            var needLong = (long)Math.Ceiling(c.Amount);

            switch (c.CurrencyGroup)
            {
                case "coin":
                    {
                        var have = GetOrZero(s.Coins, c.CurrencyId);
                        if (have < needLong)
                        {
                            reason = $"Moeda '{c.CurrencyId}' insuficiente ({have}/{needLong}).";
                            return false;
                        }
                        return true;
                    }

                case "resource":
                    {
                        var have = GetOrZero(s.Resources, c.CurrencyId);
                        if (have < needLong)
                        {
                            reason = $"Recurso '{c.CurrencyId}' insuficiente ({have}/{needLong}).";
                            return false;
                        }
                        return true;
                    }

                case "knowledge":
                    {
                        var have = GetOrZero(s.Knowledge, c.CurrencyId);
                        if (have < needLong)
                        {
                            reason = $"Conhecimento '{c.CurrencyId}' insuficiente ({have}/{needLong}).";
                            return false;
                        }
                        return true;
                    }

                default:
                    reason = $"Moeda desconhecida: {c.CurrencyGroup}";
                    return false;
            }
        }
        private static string InferCurrencyGroup(string coinId)
        {
            if (string.IsNullOrWhiteSpace(coinId)) return "coin"; // fallback seguro

            // 1) prefixo (mais barato/estável se você segue convenção)
            // m01 -> coin, r10 -> resource, k02 -> knowledge
            var ch = char.ToLowerInvariant(coinId[0]);
            if (ch == 'm') return "coin";
            if (ch == 'r') return "resource";
            if (ch == 'k') return "knowledge";

            // 2) data registries (se existirem com cobertura)
            if (CoinsData.All.ContainsKey(coinId)) return "coin";
            if (ResourceData.All.ContainsKey(coinId)) return "resource";
            if (KnowledgeData.All.ContainsKey(coinId)) return "knowledge";

            // 3) fallback
            return "coin";
        }

        private static long GetOrZero(Dictionary<string, long> dict, string id)
            => dict is not null && dict.TryGetValue(id, out var v) ? v : 0L;
        private static void AddOrSet(Dictionary<string, long> dict, string id, long delta)
        {
            if (!dict.TryGetValue(id, out var v)) v = 0L;
            var nv = v + delta;
            if (nv < 0) nv = 0; // proteção mínima
            dict[id] = nv;
        }
    }
}
