using FurmaIdle.Helpers;
using FurmaIdle.Models;
using System.Diagnostics.Contracts;
using System.Xml.Linq;

namespace FurmaIdle.Services
{
    public interface IContractsService
    {
        void TickContracts(GameModel game, string stageId, double dtSeconds);

        (int ContractsCap, int ContractsUsed, int ContractsLevel, int ContractsMaxLevel) GetStageInfo(GameModel game, string stageId);
        IReadOnlyList<string> AvaliableContracts(GameModel game, string stageId);
        string GetChosenContractIdForLevel(GameModel game, string stageId, int level);

        (string CoinId, double CoinsPerCycle, double SecondsPerCycle, double TotalPerCycle, double TotalPerSecond) GetContractInfo(ContractModel contract, StageModel stage);
        double GetContractProgress(GameModel game, string stageId, string contractId);

        public Dictionary<string, double> GetGameContractsPerSecond(GameModel game);
        public Dictionary<string, double> GetStageContractsPerSecond(GameModel game, string stageId);

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
        private readonly IModifierService _modifier;
        private readonly IKnowledgeService _knowledge;
        private readonly IUiService _ui;
        public ContractsService(ILocateService locate, IIncomeService income, ICurrentGameService game, IModifierService modifier, IKnowledgeService knowledge, IUiService ui)
        {
            _locate = locate;
            _income = income;
            _game = game;
            _modifier = modifier;
            _knowledge = knowledge;
            _ui = ui;
        }

        bool ExpeditionUnlocked = false;

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

                var realParameters = GetContractInfo(contract, stage);

                prog += dtSeconds / realParameters.SecondsPerCycle;

                // fecha ciclos inteiros
                var cycles = (long)Math.Floor(prog);
                if (cycles > 0)
                {
                    prog -= cycles;

                    var perCycle = realParameters.CoinsPerCycle;

                    var total = perCycle * qty * cycles;

                    _ = _income.AddAsync(ItemHelper.ItemType.Coin, realParameters.CoinId, total, ItemHelper.ItemType.Contract, contractId, stageId);
                }

                if (prog < 0) prog = 0;
                if (prog > 1) prog = 1;
                stage.ActiveContractsProgress[contractId] = prog;
            }
        }

        // Parâmetros do Stage
        public (int ContractsCap, int ContractsUsed, int ContractsLevel, int ContractsMaxLevel) GetStageInfo(GameModel game, string stageId)
        {
            var stage = _locate.LocateStage(game, stageId);

            int contractsCap = 0;
            int contractsUsed = 0;
            int contractsLevel = stage.StartContractLevel;
            int contractsMaxLevel = stage.MaxContractLevel;

            foreach (var characterId in stage.Expedition.PartyIds)
            {
                int characterCap = 0;
                var character = _locate.LocateCharacter(game, characterId);
                characterCap += character.ContractCap;

                foreach (var modifier in character.Modifiers)
                {
                    if (modifier.Type == EffectHelper.EffectType.ContractCapUnlock)
                    {
                        if (modifier.Operation == EffectHelper.EffectOperation.Additive)
                        {
                            characterCap += (int)modifier.Value;
                        }
                        if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                        {
                            characterCap *= (int)modifier.Value;
                        }
                    }
                }

                contractsCap += characterCap;
            }

            foreach (var contract in stage.ActiveContracts)
            {
                contractsUsed += contract.Value;
            }

            foreach (var modifier in stage.Modifiers)
            {
                int stageCap = 0;

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
                        stageCap += (int)modifier.Value;
                    }
                    if (modifier.Operation == EffectHelper.EffectOperation.Multiplicative)
                    {
                        stageCap *= (int)modifier.Value;
                    }
                }

                contractsCap += stageCap;
            }

            if (ExpeditionUnlocked == false)
            {
                if (contractsUsed >= 30)
                {
                    _ui.NavMenuControl("UnlockExpedition");
                    ExpeditionUnlocked = true;
                }
            }
            else
            {
                if (contractsUsed < 30)
                {
                    ExpeditionUnlocked = false;
                }
            }

            return (contractsCap, contractsUsed, contractsLevel, contractsMaxLevel);
        }

        public IReadOnlyList<string> AvaliableContracts(GameModel game, string stageId)
        {
            if (game is null) return Array.Empty<string>();

            var expansion = _locate.LocateExpansion(game, game.CurrentExpansionId);
            var stage = _locate.LocateStage(game, stageId);
            var expedition = stage.Expedition;

            var liberated = new List<string>();
            foreach (var characterId in expedition.PartyIds)
            {
                var character = _locate.LocateCharacter(game, characterId);
                foreach (var contractId in character.ContractsIds)
                {
                    var contract = _locate.LocateContract(game, contractId);
                    if (contract.State == UnlockHelper.State.Unlocked)
                    {
                        if (!expansion.inUseContracts.Contains(contract.Id))
                        {
                            if(!liberated.Contains(contract.Id)) liberated.Add(contract.Id);
                        }
                    }
                }
            }

            liberated.Sort(StringComparer.Ordinal);

            return liberated;
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

        // Parametros de produção de um contrato
        public (string CoinId, double CoinsPerCycle, double SecondsPerCycle, double TotalPerCycle, double TotalPerSecond) GetContractInfo(ContractModel contract, StageModel stage)
        {
            stage.ActiveContracts.TryGetValue(contract.Id, out var qty);

            var (cps, spc) = ContractHelper.GetContractBase(contract);

            var gainMod = _modifier.GetModifiers(ItemHelper.ItemType.Contract, contract.Id, stage.Id, EffectHelper.EffectSupertype.Gain);
            var timeMod = _modifier.GetModifiers(ItemHelper.ItemType.Contract, contract.Id, stage.Id, EffectHelper.EffectSupertype.Time);

            var expansion = _locate.LocateExpansion(_game.CurrentGame, _game.CurrentGame.CurrentExpansionId);
            
            var knowBurst = _knowledge.GetKnowledgeBurst(_game.CurrentGame, contract.CoinId, expansion);

            var coinsPerCycle = (cps + gainMod.AddMod) * gainMod.MultMod * knowBurst;

            var totalPerCycle = coinsPerCycle * qty;

            var timePerCycle = (spc + timeMod.AddMod) * timeMod.MultMod;

            var totalPerSecond = (coinsPerCycle / timePerCycle) * qty;

            return (contract.CoinId, coinsPerCycle, timePerCycle, totalPerCycle, totalPerSecond);
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

        // Parâmetros Generalistas
        public Dictionary<string, double> GetStageContractsPerSecond(GameModel game, string stageId)
        {
            Dictionary<string, double> result = new Dictionary<string, double>();

            if (game is null || string.IsNullOrWhiteSpace(stageId))
                return result;

            var stage = _locate.LocateStage(game, stageId);
            var expedition = _locate.LocateExpedition(game, stage.Id);

            var activeContracts = stage?.ActiveContracts;
            if (stage is null || activeContracts is null || activeContracts.Count == 0) return result;

            foreach(var coin in game.Coins)
            {
                if (coin.Value.State == UnlockHelper.State.Unlocked)
                {
                    double sum = 0;

                    foreach (var (contractId, qty) in activeContracts)
                    {
                        if (qty <= 0) continue;
                        var contract = _locate.LocateContract(game, contractId);
                        if (contract.CoinId == coin.Key)
                        {
                            var contractGeneration = GetContractInfo(contract, stage);
                            sum += contractGeneration.TotalPerSecond;
                        }
                    }

                    if(sum >= 0)
                    {
                        result.Add(coin.Key, sum);
                    }

                    expedition.CurrentCoinPerSec = sum;
                }
            }


            return result;
        }
        public Dictionary<string, double> GetGameContractsPerSecond(GameModel game)
        {

            Dictionary<string, double> result = new Dictionary<string, double>();

            if (game is null) return result;

            foreach (var coin in game.Coins)
            {
                if (coin.Value.State == UnlockHelper.State.Unlocked)
                {
                    double sum = 0;

                    foreach (var stage in game.Stages)
                    {
                        if (stage.Value.State == UnlockHelper.State.Unlocked)
                        {
                            foreach (var (contractId, qty) in stage.Value.ActiveContracts)
                            {
                                if (qty <= 0) continue;
                                var contract = _locate.LocateContract(game, contractId);
                                if (contract.CoinId == coin.Key)
                                {
                                    var contractGeneration = GetContractInfo(contract, stage.Value);
                                    sum += contractGeneration.TotalPerSecond;
                                }
                            }
                        }
                    }

                    if (sum >= 0)
                    {
                        result.Add(coin.Key, sum);
                    }
                }
            }

            return result;
        }

        public async Task BurstProduction(double BurstTime, string stageId, string specId)
        {
            var stage = _locate.LocateStage(_game.CurrentGame, stageId);
            var stagePerSec = GetStageContractsPerSecond(_game.CurrentGame, stageId);

            foreach (var coin in stagePerSec)
            {
                var amount = coin.Value * BurstTime;

                if (amount > 0)
                    await _income.AddAsync(ItemHelper.ItemType.Coin, stage.CoinId, amount,
                                           ItemHelper.ItemType.Specialty, specId, stageId);
            }
        }
    }
}
