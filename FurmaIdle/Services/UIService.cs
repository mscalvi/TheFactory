using FurmaIdle.Models;
using System.Threading.Tasks;
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
        void SyncMenusFromGame(GameModel g);

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
            new() { Id = "i1",  Label = "Expan",   Unlocked = true },
            new() { Id = "i2",  Label = "Stage",   Unlocked = true },
            new() { Id = "i3",  Label = "Exped",   Unlocked = true },
            new() { Id = "i5",  Label = "Up",      Unlocked = true },
            new() { Id = "i50", Label = "Tech",    Unlocked = true },
            new() { Id = "i97", Label = "Achiev",  Unlocked = true },
            new() { Id = "i98", Label = "Out",     Unlocked = true },
            new() { Id = "i99", Label = "Sett",    Unlocked = true },
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

            _ = _game.Mutate(g =>
            {
                g.UnlockedMenus ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                g.UnlockedMenus.Remove(id);
            }, save: true);

            RaiseChanged();
        }

        public void UnlockMenu(string id)
        {
            var item = _nav.FirstOrDefault(n => string.Equals(n.Id, id, StringComparison.OrdinalIgnoreCase));
            if (item is null) return;
            if (item.Unlocked) return;

            item.Unlocked = true;

            _ = _game.Mutate(g =>
            {
                g.UnlockedMenus ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                g.UnlockedMenus.Add(id);
            }, save: true);

            RaiseChanged();
        }

        public void NavMenuControl(string itemId)
        {

        }

        public bool SetNotificationMenu(string menuId)
        {
            return true;
        }

        public void SyncMenusFromGame(GameModel g)
        {
            // garante que sempre existe HashSet no save
            g.UnlockedMenus ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in _nav)
            {
                // Regra: se o id está no save, fica desbloqueado.
                // Se não está, fica bloqueado.
                item.Unlocked = g.UnlockedMenus.Contains(item.Id);
            }

            // fallback obrigatório: se nada estava salvo ainda (jogo novo),
            // garante pelo menos Stage aberto (i2) pra não quebrar a UI:
            if (!g.UnlockedMenus.Any())
            {
                var fallback = _nav.FirstOrDefault(x => x.Id == "i2");
                if (fallback is not null)
                {
                    fallback.Unlocked = true;
                    g.UnlockedMenus.Add("i2");
                }
            }

            // opcional: também seta qual menu está aberto visualmente
            OpenMenuId = g.UnlockedMenus.Contains(OpenMenuId ?? "")
                ? OpenMenuId
                : "i2";

            PreviousMenuId = OpenMenuId;
            RaiseChanged();
        }

        private bool IsMenuUnlocked(string id)
        {
            var item = _nav.FirstOrDefault(n => string.Equals(n.Id, id, StringComparison.OrdinalIgnoreCase));
            return item is not null && item.Unlocked;
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
