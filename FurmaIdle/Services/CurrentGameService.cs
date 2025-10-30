using FurmaIdle.Helpers;
using FurmaIdle.Models;
using FurmaIdle.Storage;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Channels;
using static FurmaIdle.Helpers.LogHelper;

namespace FurmaIdle.Services
{
    public interface ICurrentGameService
    {
        // GameModel
        GameModel CurrentGame { get; }

        // Preparation
        void Attach(GameModel game);
        void MarkReady();
        bool IsReady { get; }
        event Action? ReadyChanged;

        // Game Change
        Task Mutate(Action<GameModel> edit, bool save = true, bool ui = true);
        event Action? GameChanged;
    }

    public sealed class CurrentGameService : ICurrentGameService
    {
        private readonly IGameStore _store;

        public CurrentGameService(IGameStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public GameModel CurrentGame { get; private set; } = new();
        public bool IsReady { get; private set; }
        public event Action? GameChanged;
        public event Action? ReadyChanged;

        #region Geral
        public void Attach(GameModel game)
        {
            CurrentGame = game;
            GameChanged?.Invoke();
        }
        public async Task Mutate(Action<GameModel> edit, bool save = true, bool ui = true)
        {
            if (edit is null) return;

            // aplica mutações no estado vivo
            edit(CurrentGame);

            // notifica a UI
            if (ui)
            {
                GameChanged?.Invoke();
            }

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

        #endregion
    }
}
