using static FurmaIdle.Services.UiService;

namespace FurmaIdle.Services
{

    public interface IUiService
    {
        Task LoadStage(string stageId);

        string? OpenMenuId { get; }
        string? PreviousMenuId { get; }
        IEnumerable<NavItem> VisibleNav {  get; }
        void SetOpenMenu(string? id);
        void NavMenuControl(string itemId);
        bool SetNotificationMenu(string menuId);

        void LockMenu(string id);
        void UnlockMenu(string id);

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

            RaisePulse();
        }

        public event Action? Changed;
        public event Action? Pulse;

        public void RaisePulse()
        {
            Pulse?.Invoke();
        }

        private void RaiseChanged()
        {
            Changed?.Invoke();
        }


        #region Menu
        public sealed class NavItem
        {
            public required string Id { get; init; } = "";
            public required string Label { get; init; } = "";
            public bool Unlocked { get; set; } = false;
            public bool Notification { get; set; } = false;
            public int SortKey =>
                int.TryParse(Id.AsSpan(1), out var n) ? n : int.MaxValue;
        }

        private readonly List<NavItem> _nav = new()
        {
            new() { Id = "i1",  Label = "Expan",   Unlocked = false },
            new() { Id = "i2",  Label = "Stage",   Unlocked = true },
            new() { Id = "i3",  Label = "Exped",   Unlocked = false },
            new() { Id = "i4",  Label = "Party",   Unlocked = false },
            new() { Id = "i5",  Label = "Up",      Unlocked = false },
            new() { Id = "i50", Label = "Tech",    Unlocked = false },
            new() { Id = "i97", Label = "Achiev",  Unlocked = false },
            new() { Id = "i98", Label = "Out",     Unlocked = false },
            new() { Id = "i99", Label = "Sett",    Unlocked = false },
        };

        public IEnumerable<NavItem> VisibleNav =>
            _nav
                .Where(item => item.Unlocked)
                .OrderBy(item => item.SortKey);

        public string? OpenMenuId { get; private set; } = "i2";
        public string? PreviousMenuId { get; private set; } = "i2";

        public void SetOpenMenu(string? id)
        {
            if (OpenMenuId == id)
                return;

            PreviousMenuId = OpenMenuId;
            OpenMenuId = id;

            RaiseChanged();
        }

        public void LockMenu(string id)
        {
            var item = _nav.FirstOrDefault(n => string.Equals(n.Id, id, StringComparison.OrdinalIgnoreCase));
            if (item is null) return;
            if (!item.Unlocked) return;

            item.Unlocked = false;
            RaiseChanged();
        }

        public void UnlockMenu(string id)
        {
            var item = _nav.FirstOrDefault(n => string.Equals(n.Id, id, StringComparison.OrdinalIgnoreCase));
            if (item is null) return;
            if (item.Unlocked) return;

            item.Unlocked = true;
            RaiseChanged();
        }

        public void NavMenuControl(string itemId)
        {
            switch (itemId) 
            {
                case "ul00":
                    SetNotificationMenu("i5");
                    UnlockMenu("i5");
                    break;
                default: 
                    break; 
            }
        }

        public bool SetNotificationMenu(string menuId)
        {
            return true;
        }
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
