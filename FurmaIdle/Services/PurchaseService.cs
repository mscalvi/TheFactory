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
        private readonly IEffectService _effect;
        private readonly IUiService _ui;
        private readonly ICostService _cost;
        private readonly ITooltipService _tooltip;

        public PurchaseService(
            ICurrentGameService Game,
            ILocateService Locate,
            IEffectService effect,
            IUiService ui,
            ICostService cost,
            ITooltipService tooltip)
        {
            _game = Game;
            _locate = Locate;
            _effect = effect;
            _ui = ui;
            _cost = cost;
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

            string helper = "";

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
                        var upgrade = _locate.LocateUpgrade(g, itemId);
                        helper = upgrade.TargetId;
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
                if(stage.Id == "s00")
                {
                    // Contract Level Unlock Stage 0
                    if (itemId == "ub00")
                    {
                        var upgrade = _locate.LocateUpgrade(game, itemId);
                        if (upgrade.ActualBuy == 1)
                        {
                            _ui.NavMenuControl("ContractLevel0Unlock");
                        }
                        if (upgrade.ActualBuy == 2)
                        {
                            _ui.NavMenuControl("ContractLevel1Unlock");
                        }
                    }

                    // Contract Level 0 Purchase Stage 0
                    if (itemId == "c001")
                    {
                        if (contractBuy == 1)
                        {
                            _ui.NavMenuControl("FirstContract0Purchase");
                        }
                        if (contractBuy == 5)
                        {
                            _ui.NavMenuControl("5xContract0Purchase");
                        }
                    }

                    // Contract Unlock Stage 0
                    if (itemId == "uu001")
                    {
                        _ui.NavMenuControl("FirstContractUnlock");                       
                    }

                    // Contract Level 1 Purchase Stage 0
                    if (itemId == "c011")
                    {
                        if (contractBuy == 1)
                        {
                            _ui.NavMenuControl("FirstContract1Purchase");
                        }
                    }
                }

                if (stage.Id == "s01")
                {
                    if (StartsWith(itemId, "up"))
                    {
                        int chars = 0;

                        foreach (var character in game.Characters)
                        {
                            if (character.Value.State == UnlockHelper.State.Unlocked)
                            {
                                chars++;
                            }
                        }

                        if (chars == 2)
                        {
                            _ui.NavMenuControl("FirstCharacterUnlock", helper);
                        }
                    }
                    if (itemId == "ur01")
                    {
                        foreach (var character in game.Characters)
                        {
                            if (character.Value.State == UnlockHelper.State.Unlocked && character.Value.CharState == UnlockHelper.CharState.InBase)
                            {
                                helper = character.Value.Id;
                            }
                        }

                        _ui.NavMenuControl("FirstResourceUnlock", helper);
                    }
                    if (itemId == "ua011")
                    {
                        foreach (var character in game.Characters)
                        {
                            if (character.Value.State == UnlockHelper.State.Unlocked && character.Value.CharState == UnlockHelper.CharState.InBase)
                            {
                                helper = character.Value.Id;
                            }
                        }

                        _ui.NavMenuControl("FirstExpeditionUnlock", helper);
                    }
                    if (StartsWith(itemId, "uk"))
                    {
                        int knows = 0;

                        foreach (var know in game.Knowledges)
                        {
                            if (know.Value.State == UnlockHelper.State.Unlocked)
                            {
                                knows++;
                            }
                        }

                        if (knows == 1)
                        {
                            _ui.NavMenuControl("FirstKnowledgeUnlock");
                        }
                    }
                    if (itemId == "ue01")
                    {
                        _ui.NavMenuControl("FirstExpansionUnlock");
                    }
                    if (StartsWith(itemId, "uh"))
                    {
                        int techs = 0;

                        foreach (var tech in game.Techs)
                        {
                            if (tech.Value.State == UnlockHelper.State.Unlocked)
                            {
                                techs++;
                            }
                        }

                        if (techs == 1)
                        {
                            _ui.NavMenuControl("FirstTechUnlock");
                        }
                    }
                    if (StartsWith(itemId, "un01"))
                    {
                        _ui.NavMenuControl("FirstShipUnlock", helper);
                    }
                    if (itemId == "us02")
                    {
                        _ui.NavMenuControl("FirstStageUnlock");
                    }
                }

                if (StartsWith(itemId, "ul"))
                {
                    _ui.NavMenuControl("LocalUnlock", helper);
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
        static bool StartsWith(string? id, string prefix)
            => id?.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) == true;
    }
}
