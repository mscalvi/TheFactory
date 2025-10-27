namespace FurmaIdle.Services
{

    public interface IUiService
    {
        Task LoadStage(string stageId);

        string? OpenMenuId { get; }
        string? PreviousMenuId { get; }
        void SetOpenMenu(string? id);

        event Action? Changed;
        event Action? Pulse;          
        void RaisePulse();
    }
    public sealed class UiService : IUiService
    {
        private readonly ICurrentGameService _game;

        public UiService(ICurrentGameService game)
        {
            _game = game;
        }

        public async Task LoadStage(string stageId)
        {
            await _game.Mutate(g =>
            {
                if (string.IsNullOrWhiteSpace(stageId) || g is null)
                    return;

                if (!g.Stages.TryGetValue(stageId, out var stage))
                    throw new KeyNotFoundException($"Stage '{stageId}' não existe no jogo atual.");

                var before = g.SelectedStageId;
                g.SelectedStageId = stageId;

                Console.WriteLine($"[UI] LoadStage: {before} -> {stageId}");
            });
        }

        #region Menu Detect
        public string? OpenMenuId { get; private set; } = "i1";
        public string? PreviousMenuId { get; private set; } = "i1";
        public event Action? Changed;

        public void SetOpenMenu(string? id)
        {
            if (OpenMenuId == id) return;
            PreviousMenuId = OpenMenuId;
            OpenMenuId = id;
            Changed?.Invoke();
        }

        public event Action? Pulse;
        public void RaisePulse() => Pulse?.Invoke();
        #endregion
    }

    public enum UiLogKind { Info, Warn, Error, Success }
    public sealed class UiLogMessage
    {
        public DateTime Time { get; init; } = DateTime.Now;
        public string Text { get; init; } = "";
        public UiLogKind Kind { get; init; } = UiLogKind.Info;
    }
    public interface IUiLogService
    {
        event Action<UiLogMessage>? OnMessage;
        void Info(string text);
        void Warn(string text);
        void Error(string text);
        void Success(string text);
    }
    public sealed class UiLogService : IUiLogService
    {
        public event Action<UiLogMessage>? OnMessage;

        private void Emit(string text, UiLogKind kind) =>
            OnMessage?.Invoke(new UiLogMessage { Text = text, Kind = kind, Time = DateTime.Now });

        public void Info(string text) => Emit(text, UiLogKind.Info);
        public void Warn(string text) => Emit(text, UiLogKind.Warn);
        public void Error(string text) => Emit(text, UiLogKind.Error);
        public void Success(string text) => Emit(text, UiLogKind.Success);
    }
}
