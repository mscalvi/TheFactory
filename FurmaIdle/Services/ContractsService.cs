using FurmaIdle.Data;
using FurmaIdle.Helpers;
using FurmaIdle.Models;
using FurmaIdle.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using static FurmaIdle.Helpers.LogHelper;
using static FurmaIdle.Helpers.UnlockHelper;

namespace FurmaIdle.Services
{
    public interface IContractsService
    {
        void TickContracts(GameModel game, string stageId, double dtSeconds);
        int GetContractsCap(GameModel game, string stageId);
        int GetContractsUsed(GameModel game, string stageId);
        bool IsMaxContract(GameModel game, string stageId);
        IReadOnlyList<string> AvaliableContracts(GameModel game, string stageId);
        string GetChosenContractIdForLevel(GameModel game, string stageId, int level);

        bool BuyContract(GameModel game, string stageId, string contractId);
    }

    public sealed class ContractsTickSink : ITickSink, IDisposable
    {
        private readonly ITickService _ticks;
        private readonly IContractsService _contracts;

        public ContractsTickSink(ITickService ticks, IContractsService contracts)
        {
            _ticks = ticks;
            _contracts = contracts;
            _ticks.Subscribe(this);
        }

        public void OnTick(GameModel game, double dtSeconds)
        {
            foreach (var st in game.Stages.Values)
            {
                var ex = st.ActiveExpedition;
                if (ex is null) continue;

                _contracts.TickContracts(game, st.Id, dtSeconds);
            }
        }

        public void Dispose() => _ticks.Unsubscribe(this);
    }

    public sealed class ContractsService : IContractsService
    {
        private readonly IPurchaseService _purchase;
        private readonly ILocateService _locate;
        private readonly IIncomeService _income;
        public ContractsService(IPurchaseService purchase, ILocateService locate, IIncomeService income)
        {
            _purchase = purchase;
            _locate = locate;
            _income = income;
        }

        public bool BuyContract(GameModel game, string stageId, string contractId)
        {
            game.Stages.TryGetValue(stageId, out var stage);
            ContractModel contract = _locate.LocateContract(game, contractId);

            _purchase.Purchase(ItemHelper.ItemType.Contract, contract.Id, game.SelectedStageId, game);

            if (!stage.ActiveContracts.ContainsKey(contract.Id))
            {
                stage.ActiveContracts.Add(contract.Id, 1);
            }
            else
            {
                var quant = stage.ActiveContracts[contract.Id];
                stage.ActiveContracts[contract.Id] = quant + 1;
            }

            return true;
        }

        public bool IsMaxContract(GameModel game, string stageId)
        {
            // 0) Stage válido
            var stage = _locate.LocateStage(game, stageId);
            if (stage is null)
            {
                return true;
            }

            // 1) Expedição ativa
            var ex = stage.ActiveExpedition;
            if (ex is null || ex.ExpeditionState != UnlockHelper.ExpeditionState.Active)
            {
                return true;
            }

            // 2) Party válida?
            var party = ex.PartyIds ?? new List<string>();
            if (party.Count == 0)
            {
                return true;
            }

            // 3) Cap total
            int contractsMax = 0;
            foreach (var characterId in party.Distinct(StringComparer.Ordinal))
            {
                if (!game.Characters.TryGetValue(characterId, out var character) || character is null)
                {
                    continue;
                }

                var cap = Math.Max(0, character.ContractCap);
                contractsMax += cap;
            }

            if (contractsMax <= 0)
            {
                return true;
            }

            // 4) Usados = soma das QUANTIDADES no dicionário ActiveContracts
            int contractsTotal = 0;
            if (stage.ActiveContracts is not null)
            {
                foreach (var kv in stage.ActiveContracts)
                {
                    var contractId = kv.Key;
                    var qty = Math.Max(0, kv.Value);
                    contractsTotal += qty;
                }
            }

            Console.WriteLine($"[Contracts] Usados: {contractsTotal} / Cap: {contractsMax}");
            return contractsTotal >= contractsMax;
        }
        public int GetContractsCap(GameModel game, string stageId)
        {
            int contractsMax = 0;

            var stage = _locate.LocateStage(game, stageId);

            if (stage.ActiveExpedition.PartyIds.Count == 0) return 0;

            foreach (var characterId in stage.ActiveExpedition.PartyIds)
            {
                game.Characters.TryGetValue(characterId, out var character);
                contractsMax += character.ContractCap;
            }

            return contractsMax;
        }
        public int GetContractsUsed(GameModel game, string stageId)
        {
            int contractsTotal = 0;

            var stage = _locate.LocateStage(game, stageId);

            foreach (var contract in stage.ActiveContracts)
            {
                contractsTotal += contract.Value;
            }

            return contractsTotal;
        }
        public IReadOnlyList<string> AvaliableContracts(GameModel game, string stageId)
        {
            if (game is null) return Array.Empty<string>();

            // 1) Colete todos os IDs permitidos a partir dos personagens desbloqueados
            var allowed = new HashSet<string>(StringComparer.Ordinal);
            foreach (var ch in game.Characters.Values)
            {
                if (ch is null) continue;
                if (ch.State != UnlockHelper.State.Unlocked) continue;
                if (ch.ContractsIds is null) continue;

                foreach (var cid in ch.ContractsIds)
                {
                    if (!string.IsNullOrWhiteSpace(cid))
                        allowed.Add(cid);
                }
            }

            if (allowed.Count == 0 || game.Contracts is null)
                return Array.Empty<string>();

            // 2) Filtre apenas contratos desbloqueados que estejam nesse conjunto
            var result = new List<string>(allowed.Count);
            foreach (var cid in allowed)
            {
                if (game.Contracts.TryGetValue(cid, out var c) &&
                    c is not null &&
                    c.State == UnlockHelper.State.Unlocked)
                {
                    result.Add(cid);
                }
            }

            // (Opcional) ordenar
            result.Sort(StringComparer.Ordinal);
            return result;
        }
        public string GetChosenContractIdForLevel(GameModel game, string stageId, int level)
        {
            var stage = _locate.LocateStage(game, stageId);

            if (stage?.ActiveContracts is null) return null;

            foreach (var kv in stage.ActiveContracts)
            {
                if (kv.Value <= 0) continue;
                var cm = _locate.LocateContract(game, kv.Key);
                if (cm is not null && cm.Level == level)
                    return kv.Key;
            }
            return null;
        }


        public void TickContracts(GameModel game, string stageId, double dtSeconds)
        {
            if (game is null || string.IsNullOrWhiteSpace(stageId) || dtSeconds <= 0) return;
            if (!game.Stages.TryGetValue(stageId, out var stage) || stage is null) return;
            if (stage.ActiveContracts is null || stage.ActiveContracts.Count == 0) return;

            stage.ActiveContractsProgress ??= new Dictionary<string, double>();

            foreach (var kv in stage.ActiveContracts)
            {
                var contractId = kv.Key;
                var qty = kv.Value;
                if (qty <= 0) continue;

                game.Contracts.TryGetValue(contractId, out var contract);
                var contractParams = ContractHelper.ProdParams(contract);

                var perSecond = contractParams.CoinsPerCycle / contractParams.SecondsPerCycle;
                if (perSecond < 0) perSecond = 0;

                // ===== 1) PROGRESSO VISUAL =====
                var prog = stage.ActiveContractsProgress.TryGetValue(contractId, out var p) ? p : 0.0;
                prog += dtSeconds / contractParams.SecondsPerCycle;

                while (prog >= 1.0)
                {
                    prog -= 1.0;

                    // recompensa por ciclo (por unidade) = perSecond * cycleSeconds
                    var rewardPerUnit = perSecond * contractParams.SecondsPerCycle;
                    var totalReward = rewardPerUnit * qty;

                    _income.AddAsync(ItemHelper.ItemType.Coin, contractParams.CoinId, totalReward);
                }

                stage.ActiveContractsProgress[contractId] = prog;
            }
        }

        public double GetContractProgress(GameModel game, string stageId, string contractId)
        {
            if (game is null || string.IsNullOrWhiteSpace(stageId) || string.IsNullOrWhiteSpace(contractId))
                return 0;

            if (!game.Stages.TryGetValue(stageId, out var stage) || stage is null)
                return 0;

            stage.ActiveContractsProgress ??= new Dictionary<string, double>();
            if (!stage.ActiveContractsProgress.TryGetValue(contractId, out var prog))
                return 0;

            if (prog < 0) prog = 0;
            if (prog > 1) prog = 1;
            return prog;
        }
    }
}
