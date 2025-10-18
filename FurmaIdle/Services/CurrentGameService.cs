using FurmaIdle.Helpers;
using FurmaIdle.Models;
using FurmaIdle.Storage;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Channels;
using static FurmaIdle.Helpers.LogHelper;

namespace FurmaIdle.Services
{
    public interface ICurrentGameService
    {
        GameModel CurrentGame { get; }
        bool IsReady { get; }
        event Action? GameChanged;
        event Action? ReadyChanged;
        void Attach(GameModel game);
        Task Mutate(Action<GameModel> edit, bool save = true);
        event Action<string, LogKind>? Logged;
        void MarkReady();
        string InstanceId { get; }
    }

    public sealed class CurrentGameService : ICurrentGameService
    {

        private readonly IGameStore _store;
        private readonly IModifierService _modifiers;
        private readonly IStageService _stages;

        public CurrentGameService(IGameStore store, IModifierService modifiers, IStageService stages)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _modifiers = modifiers;
            _stages = stages;
        }

        public GameModel CurrentGame { get; private set; } = new();
        public bool IsReady { get; private set; }
        public event Action? GameChanged;
        public event Action? ReadyChanged;
        public event Action<string, LogKind>? Logged;

        #region Geral
        public void Attach(GameModel game)
        {
            CurrentGame = game;
            GameChanged?.Invoke();
        }
        public async Task Mutate(Action<GameModel> edit, bool save = true)
        {
            if (edit is null) return;

            // aplica mutações no estado vivo
            edit(CurrentGame);
            // notifica a UI
            GameChanged?.Invoke();

            // persiste no storage (IndexedDB via JS)
            if (save)
            {
                await _store.SaveAsync(CurrentGame);
            }
        }
        public void MarkReady()
        {
            if (IsReady) return;
            IsReady = true;
            ReadyChanged?.Invoke();
        }

        public string InstanceId { get; } = Guid.NewGuid().ToString("N").Substring(0, 8);
        #endregion
    }
}
