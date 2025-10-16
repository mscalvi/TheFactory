using FurmaIdle.Helpers;
using FurmaIdle.Models;
using FurmaIdle.Storage;
using System.Threading.Channels;
using static FurmaIdle.Helpers.LogHelper;

namespace FurmaIdle.Services
{
    public interface ICurrentGameService
    {
        GameModel CurrentGame { get; }

        // Geral
        void Attach(GameModel model, bool notify = true);
        Task Mutate(Action<GameModel> edit, bool save = true);
        event Action? GameChanged;
        event Action<string, LogKind>? Logged;

        // Tick
        void Tick(double dtSeconds);

        // Click
        void Click(string stageId);

        // Stage (foco de UI)
        bool SetSelectedStage(string stageId);
        StageModel? GetSelectedStage();

        // Expedition
        int GetEffectivePartyCap(string stageId);
        bool StartExpedition(string stageId, IReadOnlyCollection<string> party);
        bool EndExpedition(string stageId);
        IReadOnlyCollection<string> GetParty();
        bool ToggleParty(string charId);

        // Knowledge
        bool KnowledgeGain(string knowledgeId, int gain);
        Dictionary<string, int> GetKnowledgePreview(string stageId);

        // Specialties
        bool ActivateSpecialty(string charId);
        (bool active, double remaining, double total) GetSpecialtyTimer(string charId);

        // Contracts
        bool StartContract(string stageId, string contractId);
        bool BuyContract(string stageId, string contractId);
        int GetContractsCap(string stageId);
        double GetContractProgress(string stageId, string contractId);

        // Resources and Coins
        void Add(string resourceId);
        public double GetStageGeneration(string stageId);
        double GetResourceCap(string resourceId);
        bool TrySpend(string resourceId, double amount);

        // Unlocks
        bool UnlockItem(string itemId, ItemHelper.ItemType itemType);

        // Expansion
        bool BuyExpansion(string expId);

    }

    public sealed class CurrentGameService : ICurrentGameService
    {
        #region Geral
        private readonly IGameStore _store;
        private readonly IModifierService _modifiers;
        private readonly IStageService _stages;
        private readonly ILogger<CurrentGameService> _log;

        public GameModel CurrentGame { get; private set; } = new();
        public event Action? GameChanged;
        public event Action<string, LogKind>? Logged;

        public CurrentGameService(IModifierService modifiers, IStageService stages, IGameStore store, ILogger<CurrentGameService> log)
        {
            _modifiers = modifiers;
            _stages = stages;
            _store = store;
            _log = log;
        }
        public void Attach(GameModel model, bool notify = true)
        {
            CurrentGame = model ?? throw new ArgumentNullException(nameof(model));
            _log.LogInformation("Attach: CurrentGame set. Stages={Count}", model.Stages?.Count);
            if (notify) GameChanged?.Invoke();
        }
        public async Task Mutate(Action<GameModel> edit, bool save = true)
        {
            if (edit is null) return;

            // aplica mutações no estado vivo
            edit(CurrentGame);

            _log.LogInformation("Mutate: after edit. Stages={Count}", CurrentGame.Stages?.Count);

            // notifica a UI
            GameChanged?.Invoke();

            // persiste no storage (IndexedDB via JS)
            if (save)
                await _store.SaveAsync(CurrentGame);
        }
        #endregion

        #region Tick
        public void Tick(double dtSeconds)
        {

        }
        #endregion

        #region Click
        public void Click(string stageId)
        {

        }
        #endregion

        #region Stage
        public bool SetSelectedStage(string stageId)
        {
            return false;

        }
        public StageModel? GetSelectedStage() 
        { 
            StageModel Stage = new StageModel();

            return Stage;
        }
        #endregion

        #region Expedition
        public IReadOnlyCollection<string> GetParty()
        {
            IReadOnlyCollection<string> Party = new List<string>();
            return Party;
        }
        public bool StartExpedition(string stageId, IReadOnlyCollection<string> party)
        {
            return false;
        }
        public bool EndExpedition(string stageId)
        {
            return false;
        }
        public int GetEffectivePartyCap(string stageId)
        {
            return 0;
        }
        public bool ToggleParty(string charId)
        {
            return false;
        }
        #endregion

        #region Knowledge
        public bool KnowledgeGain(string knowledgeId, int gain)
        {
            return false;
        }
        public Dictionary<string, int> GetKnowledgePreview(string stageId)
        {
            Dictionary<string, int> KnowledgePreview = new Dictionary<string, int>();

            return KnowledgePreview;
        }
        #endregion

        #region Specialties
        public bool ActivateSpecialty(string charId)
        {
            return false;
        }
        public (bool active, double remaining, double total) GetSpecialtyTimer(string charId)
        {
            return (false, 0, 0);
        }
        #endregion

        #region Contracts
        public bool StartContract(string stageId, string contractId)
        {
            return false;
        }
        public bool BuyContract(string stageId, string contractId)
        {
            return false;
        }
        public int GetContractsCap(string stageId)
        {
            return 0;
        }
        public double GetContractProgress(string stageId, string contractId)
        {
            return 0;
        }
        #endregion

        #region Resources and Coins
        public void Add(string resourceId)
        {

        }
        public double GetStageGeneration(string stageId)
        {
            return 0;
        }
        public double GetResourceCap(string resourceId)
        {
            return 0;
        }
        public bool TrySpend(string resourceId, double amount)
        {
            return false;
        }
        #endregion

        #region Unlocks
        public bool UnlockItem(string itemId, ItemHelper.ItemType itemType)
        {
            return false;
        }
        #endregion

        #region Expansion
        public bool BuyExpansion(string expId)
        {
            return false;
        }
        #endregion
    }
}
