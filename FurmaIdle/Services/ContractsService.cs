using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Diagnostics.Contracts;

namespace FurmaIdle.Services
{
    public interface IContractsService
    {
        void TickContracts(GameModel game, string stageId, double dtSeconds);
        (int ContractsCap, int ContractsUsed, int ContractsLevel, int ContractsMaxLevel) GetStageContractsInfo (GameModel game, string stageId);
        
        IReadOnlyList<string> AvaliableContracts(GameModel game, string stageId);
        string GetChosenContractIdForLevel(GameModel game, string stageId, int level);
        public Dictionary<string, double> GetTotalContractsPerSecond(GameModel game);
        public double GetStageContractsPerSecond(GameModel game, string stageId, string coinId);
        double GetContractProgress(GameModel game, string stageId, string contractId);
        Task BurstProduction(double BurstTime, string stageId, string specId);
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
                var ex = st.Expedition;
                if (ex is null) continue;

                _contracts.TickContracts(game, st.Id, dtSeconds);
            }
        }

        public void Dispose() => _ticks.Unsubscribe(this);
    }

    public sealed class ContractsService : IContractsService
    {
        private readonly ILocateService _locate;
        private readonly IIncomeService _income;
        private readonly ICurrentGameService _game;
        public ContractsService(ILocateService locate, IIncomeService income, ICurrentGameService game)
        {
            _locate = locate;
            _income = income;
            _game = game;
        }

        public (int ContractsCap, int ContractsUsed, int ContractsLevel, int ContractsMaxLevel) GetStageContractsInfo(GameModel game, string stageId)
        {
            var stage = _locate.LocateStage(game, stageId);

            int contractsCap = 0;
            int contractsUsed = 0;
            int contractsLevel = stage.StartContractLevel;
            int contractsMaxLevel = stage.MaxContractLevel;

            foreach (var characterId in stage.Expedition.PartyIds)
            {
                var character = _locate.LocateCharacter(game, characterId);;
                contractsCap += character.ContractCap;

                foreach (var modifier in character.Modifiers)
                {
                    if (modifier.Type == EffectHelper.EffectType.ContractLevelUnlock)
                    {
                        if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                        {
                            contractsLevel += (int)modifier.Value;
                        }
                        if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                        {
                            contractsLevel *= (int)modifier.Value;
                        }
                    }
                    if (modifier.Type == EffectHelper.EffectType.ContractCapUnlock)
                    {
                        if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                        {
                            contractsCap += (int)modifier.Value;
                        }
                        if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                        {
                            contractsCap *= (int)modifier.Value;
                        }
                    }
                }

            }
            foreach (var contract in stage.ActiveContracts)
            {
                contractsUsed += contract.Value;
            }
            foreach (var modifier in stage.Modifiers)
            {
                if (modifier.Type == EffectHelper.EffectType.ContractLevelUnlock)
                {
                    if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                    {
                        contractsLevel += (int)modifier.Value;
                    }
                    if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                    {
                        contractsLevel *= (int)modifier.Value;
                    }
                }
                if (modifier.Type == EffectHelper.EffectType.ContractCapUnlock)
                {
                    if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                    {
                        contractsCap += (int)modifier.Value;
                    }
                    if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                    {
                        contractsCap *= (int)modifier.Value;
                    }
                }
            }

            return (contractsCap, contractsUsed, contractsLevel, contractsMaxLevel);
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

            var stage = _locate.LocateStage(game, stageId);
            var expedition = stage.Expedition;

            if (expedition is null || expedition.ExpeditionState != UnlockHelper.ExpeditionState.Active) return;

            var act = stage.ActiveContracts;
            if (act is null || act.Count == 0) return;

            stage.ActiveContractsProgress ??= new Dictionary<string, double>(StringComparer.Ordinal);

            foreach (var (contractId, qty) in act)
            {
                if (qty <= 0) continue;

                var contract = _locate.LocateContract(game, contractId);

                // progresso visual (0..1)
                var prog = stage.ActiveContractsProgress.TryGetValue(contractId, out var p) ? p : 0.0;

                var realParameters = ContractHelper.RealProdParams(contract);

                prog += dtSeconds / realParameters.SecondsPerCycle;

                // fecha ciclos inteiros
                var cycles = (long)Math.Floor(prog);
                if (cycles > 0)
                {
                    prog -= cycles;

                    var perCycle = realParameters.CoinsPerCycle;

                    var total = perCycle * qty * cycles;

                    _ = _income.AddAsync(ItemHelper.ItemType.Coin, realParameters.CoinId, total, ItemHelper.ItemType.Contract, contractId);
                }

                if (prog < 0) prog = 0;
                if (prog > 1) prog = 1;
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

        public double GetStageContractsPerSecond(GameModel game, string stageId, string coinId)
        {
            if (game is null || string.IsNullOrWhiteSpace(stageId) || string.IsNullOrWhiteSpace(coinId))
                return 0;

            var stage = _locate.LocateStage(game, stageId);
            var act = stage?.ActiveContracts;
            if (stage is null || act is null || act.Count == 0) return 0;

            double sum = 0;
            foreach (var (cid, qty) in act)
            {
                if (qty <= 0) continue;
                if (!game.Contracts.TryGetValue(cid, out var c) || c is null) continue;

                if (string.Equals(ContractHelper.CoinIdOf(c), coinId, StringComparison.Ordinal))
                    sum += ContractHelper.RealProdPerSecond(c, stage);
            }

            if (game.Coins.TryGetValue(coinId, out var coin) && coin is not null)
            {
                var coinModifiers = GetCoinModifiers(coin, EffectHelper.EffectType.CoinGain);
                var cAdd = coinModifiers.AddMod;
                var cMult = coinModifiers.MultMod <= 0 ? 1 : coinModifiers.MultMod;
                sum = (sum + cAdd) * cMult;
            }

            return sum;
        }
        public Dictionary<string, double> GetTotalContractsPerSecond(GameModel game)
        {
            var result = new Dictionary<string, double>();
            CoinModel coin = new CoinModel();
            
            if (game is null)
                return result;

            foreach (var stageDict in game.Stages)
            {
                var stage = stageDict.Value;

                if(stage.State == UnlockHelper.State.Unlocked)
                {
                    if (stage.ActiveContracts.Count <= 0) continue;

                    double sum = 0;

                    foreach (var contractDict in stage.ActiveContracts)
                    {
                        var contract = _locate.LocateContract(game, contractDict.Key);
                        var coinId = ContractHelper.CoinIdOf(contract);
                        coin = _locate.LocateCoin(game, coinId);

                        if (coin.State == UnlockHelper.State.Unlocked)
                        {
                            sum += ContractHelper.RealProdPerSecond(contract, stage);

                            var coinModifiers = GetCoinModifiers(coin, EffectHelper.EffectType.CoinGain);
                            var cAdd = coinModifiers.AddMod;
                            var cMult = coinModifiers.MultMod <= 0 ? 1 : coinModifiers.MultMod;
                            sum = (sum + cAdd) * cMult;
                        }
                    }

                    result.Add(coin.Id, sum);
                }

                return result;
            }

            return result;
        }

        public async Task BurstProduction(double BurstTime, string stageId, string specId)
        {
            var stage = _locate.LocateStage(_game.CurrentGame, stageId);
            var perSec = GetStageContractsPerSecond(_game.CurrentGame, stageId, stage.CoinId);

            var amount = perSec * BurstTime;

            if (amount > 0)
                await _income.AddAsync(ItemHelper.ItemType.Coin, stage.CoinId, amount,
                                       ItemHelper.ItemType.Specialty, specId);
        }

        private static (double AddMod, double MultMod) GetCoinModifiers(CoinModel coin, EffectHelper.EffectType type)
        {
            double AddMod = 0;
            double MultMod = 1;

            foreach (var modifier in coin.Modifiers)
            {
                if (type == modifier.Type)
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
