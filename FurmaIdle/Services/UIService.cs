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
        public string? OpenMenuId { get; private set; } = "i4";
        public string? PreviousMenuId { get; private set; } = "i4";
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
}
