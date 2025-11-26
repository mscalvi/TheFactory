using FurmaIdle.Helpers;
using FurmaIdle.Models;
using static FurmaIdle.Helpers.EffectHelper;

namespace FurmaIdle.Services
{
    public interface IPurchaseService
    {
        Task Purchase(ItemHelper.ItemType type, string itemId, string stageId);
        Task Purchase(ItemHelper.ItemType type, string itemId, string stageId, int quantity);
    }

    public sealed class PurchaseService : IPurchaseService
    {
        private readonly ICurrentGameService _game;
        private readonly ILocateService _locate;
        private readonly IUiLogService _log;
        private readonly IEffectService _effect;
        private readonly IUiService _ui;
        private readonly ICostService _cost;
        private readonly ILoreService _lore;
        private readonly ITooltipService _tooltip;

        public PurchaseService(
            ICurrentGameService Game,
            IUiLogService Log,
            ILocateService Locate,
            IEffectService effect,
            IUiService ui,
            ICostService cost,
            ILoreService lore,
            ITooltipService tooltip)
        {
            _game = Game;
            _locate = Locate;
            _log = Log;
            _effect = effect;
            _ui = ui;
            _cost = cost;
            _lore = lore;
            _tooltip = tooltip;
        }

        private int contractBuy = 0;
        private bool busy = false;

        // --- API antiga: continua igual, só delega pra quantity = 1 ---
        public Task Purchase(ItemHelper.ItemType type, string itemId, string stageId)
            => Purchase(type, itemId, stageId, 1);

        // --- Nova API: suporta quantidade ---
        public async Task Purchase(ItemHelper.ItemType type, string itemId, string stageId, int quantity)
        {
            if (busy || quantity <= 0)
                return;

            busy = true;
            try
            {
                // Por enquanto, bulk só para Contrato.
                if (type == ItemHelper.ItemType.Contract && quantity > 1)
                {
                    await PurchaseContractsBulk(itemId, stageId, quantity);
                }
                else
                {
                    // Upgrades, Specialties, etc → 1x como sempre
                    await PurchaseCore(type, itemId, stageId);
                }
            }
            finally
            {
                busy = false;
            }
        }

        // Compra "unitária" (uso interno ou para tipos que não tem bulk)
        private async Task<bool> PurchaseCore(ItemHelper.ItemType type, string itemId, string stageId)
        {
            var game = _game.CurrentGame;
            var expansion = _locate.LocateExpansion(game, game.CurrentExpansionId);
            var stage = _locate.LocateStage(game, stageId);

            var cost = _cost.ComputeCost(type, itemId, stageId);

            var coinCost = new CoinModel();
            var resourceCost = new ResourceModel();
            var knowledgeCost = new KnowledgeModel();

            bool hasFunds = cost.costId[0] switch
            {
                'm' => GetOrZero(stage.ExpeditionStats.Coins, cost.costId) >= cost.costValue,
                'r' => GetOrZero(expansion.ExpansionStats.Resources, cost.costId) >= cost.costValue,
                'k' => GetOrZero(expansion.ExpansionStats.Knowledge, cost.costId) >= cost.costValue,
                _ => false
            };

            switch (cost.costId[0])
            {
                case 'm':
                    coinCost = _locate.LocateCoin(game, cost.costId);
                    break;
                case 'r':
                    resourceCost = _locate.LocateResource(game, cost.costId);
                    break;
                case 'k':
                    knowledgeCost = _locate.LocateKnowledge(game, cost.costId);
                    break;
            }

            if (!hasFunds)
                return false;

            await _game.Mutate(g =>
            {
                ApplyDebit(stage.ExpeditionStats, expansion.ExpansionStats, cost.costValue, cost.costId);

                switch (type)
                {
                    case ItemHelper.ItemType.Upgrade:
                        // var upgrade = _locate.LocateUpgrade(g, itemId);
                        break;

                    case ItemHelper.ItemType.Contract:
                        var contract = _locate.LocateContract(g, itemId);

                        contract.GameUseState = UnlockHelper.ContractState.InUse;
                        stage.ActiveContracts ??= new Dictionary<string, int>(StringComparer.Ordinal);

                        stage.ActiveContracts[contract.Id] =
                            (stage.ActiveContracts.TryGetValue(contract.Id, out var q) ? q : 0) + 1;

                        contractBuy = stage.ActiveContracts[contract.Id];

                        stage.lockedContractLevel.Add(contract.Level);
                        if (!expansion.inUseContracts.Contains(contract.Id))
                        {
                            expansion.inUseContracts.Add(contract.Id);
                            _tooltip.Clear();
                        }
                        break;

                    case ItemHelper.ItemType.Specialty:
                        // var spec = _locate.LocateSpecialty(g, itemId);
                        break;
                }

                ApplyStats(expansion.ExpansionStats, g.GameStats, cost.costValue, cost.costId);

            }, save: false, ui: false);

            await _effect.ApplyEffect(type, itemId, stageId);

            await _game.Mutate(g =>
            {
                if (g.GameStats.CharactersUnlocked == 2 && itemId.StartsWith("up"))
                {
                    _ui.NavMenuControl("FirstCharacterPurchase");
                    _lore.LoreTrigger("FirstCharacterPurchase");
                    _lore.LoreTrigger(itemId);
                }
                else if (g.GameStats.KnowledgesUnlocked == 1 && itemId.StartsWith("uk"))
                {
                    _ui.NavMenuControl("FirstKnowledgePurchase");
                    _lore.LoreTrigger("FirstKnowledgePurchase");
                    _lore.LoreTrigger(itemId);
                }
                else if (g.GameStats.TechUnlocked == 1 && itemId.StartsWith("uh"))
                {
                    _ui.NavMenuControl("FirstTechPurchase");
                    _lore.LoreTrigger("FirstTechPurchase");
                    _lore.LoreTrigger(itemId);
                }
                else if (itemId.StartsWith("c"))
                {
                    _ui.NavMenuControl(itemId, contractBuy.ToString());
                    _lore.LoreTrigger(itemId, contractBuy.ToString());
                }
                else
                {
                    _ui.NavMenuControl(itemId);
                    _lore.LoreTrigger(itemId);
                }

            }, save: true, ui: true);

            if (type == ItemHelper.ItemType.Upgrade)
            {
                var up = _locate.LocateUpgrade(game, itemId);
                if (up.MaxBuy == 1 && up.ActualBuy >= 1)
                {
                    _tooltip.Clear();
                }
            }

            return true;
        }

        // Bulk específico para contratos (x10 / MAX)
        private async Task PurchaseContractsBulk(string contractId, string stageId, int quantity)
        {
            var game = _game.CurrentGame;
            var expansion = _locate.LocateExpansion(game, game.CurrentExpansionId);
            var stage = _locate.LocateStage(game, stageId);
            var contract = _locate.LocateContract(game, contractId);

            // Garantir dicionário
            stage.ActiveContracts ??= new Dictionary<string, int>(StringComparer.Ordinal);
            stage.ActiveContracts.TryGetValue(contractId, out var currentQty);

            if (quantity <= 0)
                return;

            // Custo total da quantidade solicitada
            var (totalCost, costId) = _cost.ComputeCost(
                ItemHelper.ItemType.Contract,
                contractId,
                stageId,
                quantity
            );

            if (totalCost <= 0 || string.IsNullOrWhiteSpace(costId))
                return;

            // Quanto temos disponível na "fonte" certa
            long have = costId[0] switch
            {
                'm' => GetOrZero(stage.ExpeditionStats.Coins, costId),
                'r' => GetOrZero(expansion.ExpansionStats.Resources, costId),
                'k' => GetOrZero(expansion.ExpansionStats.Knowledge, costId),
                _ => 0L
            };

            if (have < totalCost)
                return;

            await _game.Mutate(g =>
            {
                // 1) Debita tudo de uma vez
                ApplyDebit(stage.ExpeditionStats, expansion.ExpansionStats, totalCost, costId);

                // 2) Atualiza quantidade do contrato
                contract.GameUseState = UnlockHelper.ContractState.InUse;

                var oldQty = currentQty;
                var newQty = oldQty + quantity;
                stage.ActiveContracts[contract.Id] = newQty;
                contractBuy = newQty;

                // 3) Marca nível/uso na expansão
                stage.lockedContractLevel.Add(contract.Level);
                if (!expansion.inUseContracts.Contains(contract.Id))
                {
                    expansion.inUseContracts.Add(contract.Id);
                }

                // 4) Stats (gasto total)
                ApplyStats(expansion.ExpansionStats, g.GameStats, totalCost, costId);

            }, save: false, ui: false);

            // Aplica efeitos baseada no estado final
            await _effect.ApplyEffect(ItemHelper.ItemType.Contract, contractId, stageId);

            // UI / Lore uma vez só
            await _game.Mutate(g =>
            {
                _ui.NavMenuControl(contractId, contractBuy.ToString());
                _lore.LoreTrigger(contractId, contractBuy.ToString());
            }, save: true, ui: true);
        }

        // -------- helpers de débito / stats / leitura --------

        private static void ApplyDebit(StatsModel expeditionStats, StatsModel expansionStats, long cost, string costId)
        {
            char costGroup = costId?[0] ?? '\0';

            switch (costGroup)
            {
                case 'm':
                    AddOrSet(expeditionStats.Coins, costId, -cost);
                    AddOrSet(expansionStats.Coins, costId, -cost);
                    break;

                case 'r':
                    AddOrSet(expansionStats.Resources, costId, -cost);
                    break;

                case 'k':
                    AddOrSet(expansionStats.Knowledge, costId, -cost);
                    break;
            }
        }

        private static void ApplyStats(StatsModel expansion, StatsModel game, long cost, string costId)
        {
            char costGroup = costId?[0] ?? '\0';

            switch (costGroup)
            {
                case 'm':
                    AddOrSet(expansion.CoinsSpent, costId, +cost);
                    AddOrSet(game.CoinsSpent, costId, +cost);
                    break;

                case 'r':
                    AddOrSet(expansion.ResourcesSpent, costId, +cost);
                    AddOrSet(game.ResourcesSpent, costId, +cost);
                    break;

                case 'k':
                    AddOrSet(expansion.KnowledgeSpent, costId, +cost);
                    AddOrSet(game.KnowledgeSpent, costId, +cost);
                    break;
            }
        }

        private static void AddOrSet(Dictionary<string, long> dict, string id, long delta)
        {
            if (!dict.TryGetValue(id, out var v)) v = 0L;
            var nv = v + delta;
            if (nv < 0) nv = 0;
            dict[id] = nv;
        }

        private static long GetOrZero(Dictionary<string, long> dict, string id)
            => dict is not null && dict.TryGetValue(id, out var v) ? v : 0L;
    }
}
