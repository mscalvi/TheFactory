using FurmaIdle.Models;
using System.Threading.Tasks;
using static FurmaIdle.Services.UiService;

namespace FurmaIdle.Services
{

    public interface IUiService
    {
        Task LoadStage(string stageId);

        string? OpenMenuId { get; }
        IEnumerable<NavItem> VisibleNav {  get; }
        void SetOpenMenu(string? id);
        void NavMenuControl(string itemId, string? help = "");
        void SyncMenusFromGame(GameModel g);
        string PanelClass(string classId);

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
        #region Menu Panels
        private readonly HashSet<string> _hidden = new(StringComparer.Ordinal);
        private static readonly string[] AllPanels = {
            "knowledge",
            "tech-available",
            "tech-done",
            "up-objetive",
            "up-expansion",
            "up-expedition",
            "up-permanents",
        };

        private readonly List<string> GamePanels = new(AllPanels);

        public void HidePanel(string id) => _hidden.Add(id);
        public void ShowPanel(string id) => _hidden.Remove(id);
        public bool IsHidden(string id) => _hidden.Contains(id);

        public string PanelClass(string id)
        {
            var cls = "menu-panel";
            if (IsHidden(id)) cls += " is-hidden";
            return cls;
        }
        #endregion

        public string? OpenMenuId { get; private set; } = "i5";
        public string? PreviousMenuId { get; private set; } = "i5";
        public sealed class NavItem
        {
            public required string Id { get; init; } = "";
            public required string Label { get; init; } = "";
            public bool Unlocked { get; set; } = true;
            public bool Notification { get; set; } = false;
            public int SortKey =>
                int.TryParse(Id.AsSpan(1), out var n) ? n : int.MaxValue;
        }
        private readonly List<NavItem> _nav = new()
        {
            new() { Id = "i1",  Label = "Expan",   Unlocked = false },
            new() { Id = "i2",  Label = "Stage",   Unlocked = false },
            new() { Id = "i3",  Label = "Exped",   Unlocked = false },
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
            // garante pelo menos Updates aberto (i5) pra não quebrar a UI:
            if (!g.UnlockedMenus.Any())
            {
                foreach (var id in _nav.Select(n => n.Id))
                    g.UnlockedMenus.Add(id);

                foreach (var item in _nav)
                    item.Unlocked = true;
            }

            // opcional: também seta qual menu está aberto visualmente
            OpenMenuId = g.UnlockedMenus.Contains(OpenMenuId ?? "")
                ? OpenMenuId
                : "i5";

            PreviousMenuId = OpenMenuId;
            RaiseChanged();
        }

        public void SetOpenMenu(string? id)
        {
            if (OpenMenuId == id)
                return;

            PreviousMenuId = OpenMenuId;
            OpenMenuId = id;

            RaiseChanged();
        }
        private void LockMenu(string id)
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
        private void UnlockMenu(string id)
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
        private bool IsMenuUnlocked(string id)
        {
            var item = _nav.FirstOrDefault(n => string.Equals(n.Id, id, StringComparison.OrdinalIgnoreCase));
            return item is not null && item.Unlocked;
        }

        public void NavMenuControl(string itemId, string? help = "")
        {
            string itemType1 = itemId.Substring(0, 1);
            string itemType2 = itemId.Substring(0, 2);

            switch (itemId) 
            {
                // First Unlocks
                case "FirstCharacterPurchase":
                    // Libera Menu de Expansion
                    Console.Write("[UI] Expansion Liberado");

                    UnlockMenu("i1");

                    SetNotificationMenu("i1");
                    break;
                case "us00":
                    // Libera Menu de Stage e de Outside Market
                    Console.Write("[UI] Stage e Market Liberados");

                    UnlockMenu("i2");
                    UnlockMenu("i98");

                    SetOpenMenu("i2");
                    break;
                case "ua01":
                    // Libera Menu de Expedition
                    Console.Write("[UI] Expedition Liberado");

                    UnlockMenu("i3");

                    SetNotificationMenu("i3");
                    break;
                case "GameStart":
                    // Libera Menu de Updates e de Settings
                    Console.Write("[UI] Updates e Settings Liberados");

                    foreach(var panel in GamePanels)
                    {
                        HidePanel(panel);
                    }

                    UnlockMenu("i5");
                    UnlockMenu("i99");

                    SetOpenMenu("i5");
                    SetNotificationMenu("i99");
                    break;
                case "FirstKnowledgePurchase":
                    Console.Write("[UI] Tech Liberado");
                    // Libera Menu de Tech

                    if (IsHidden("knowledge"))
                    {
                        ShowPanel("knowledge");
                    }

                    if (IsHidden("tech-available"))
                    {
                        ShowPanel("tech-available");
                    }

                    UnlockMenu("i50");

                    SetNotificationMenu("i50");
                    break;
                case "ue01":
                    // Libera Menu de Archievments
                    Console.Write("[UI] Game Stats Liberados");

                    UnlockMenu("i97");

                    SetNotificationMenu("i97");
                    break;
                case "c011":
                    if (IsHidden("up-expansion"))
                    {
                        ShowPanel("up-expansion");
                    }
                    if(help == "2")
                    {
                        if (IsHidden("up-permanents"))
                        {
                            ShowPanel("up-permanents");
                        }
                    }
                    break;
                case "um01":
                    if (IsHidden("up-expedition"))
                    {
                        ShowPanel("up-expedition");
                    }
                    break;
                case "FirstTechPurchase":

                    if (IsHidden("tech-done"))
                    {
                        ShowPanel("tech-done");
                    }
                    break;

                // Gerais
                case "ExpeditionStart":
                    Console.Write("[UI] Expedition Start");

                    UnlockMenu("i5");

                    SetOpenMenu("i5");
                    break;
                case "ExpeditionEnd":
                    Console.Write("[UI] Expedition End");

                    SetOpenMenu("i3");

                    LockMenu("i5");
                    break;
                case "ExpansionEnd":
                    Console.Write("[UI] Expansion End");

                    SetOpenMenu("i3");

                    LockMenu("i5");
                    break;

                default: break;
            }

            LoreTrigger(itemId);
        }

        private bool SetNotificationMenu(string menuId)
        {
            return true;
        }
        #endregion

        #region Lore
        private void LoreTrigger(string itemId)
        {
            string itemType1 = itemId.Substring(0, 1);
            string itemType2 = itemId.Substring(0, 2);

            switch (itemId)
            {
                case "GameStart":
                    break;
                case "ExpeditionStart":
                    break;
                case "ExpeditionEnd":
                    break;
                case "ExpansionEnd":
                    break;
                case "CharacterPurchase":
                    break;
                default: break;
            }
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
