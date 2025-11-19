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
        private readonly IUiLogService _log;
        private readonly ILoreService _lore;

        public UiService(ICurrentGameService game, IUiLogService log, ILoreService lore)
        {
            _game = game;
            _log = log;
            _lore = lore;
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
            "tech-knowledge",
            "tech-available",
            "tech-done",
            "up-objetive",
            "up-expansion",
            "up-expedition",
            "up-permanents",
            "expedition-toggle",
            "expedition-gain",
            "expedition-status",
            "expedition-party",
            "expansion-status",
            "expansion-basechars",
            "expansion-stagechars",
            "expansion-upgrades",
            "game-status",
        };

        private readonly List<string> GamePanels = new(AllPanels);

        public void HidePanel(string id)
        {
            _ = _game.Mutate(g =>
            {
                g.Ui ??= new UiState();
                g.Ui.HiddenPanels.Add(id);
            }, save: true);
            RaiseChanged();
        }

        public void ShowPanel(string id)
        {
            _ = _game.Mutate(g =>
            {
                g.Ui ??= new UiState();
                g.Ui.HiddenPanels.Remove(id);
            }, save: true);
            RaiseChanged();
        }

        public bool IsHidden(string id)
        {
            var g = _game.CurrentGame;
            return g?.Ui?.HiddenPanels?.Contains(id) == true;
        }


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
            new() { Id = "i1",  Label = "EXPAN",   Unlocked = false },
            new() { Id = "i2",  Label = "REGIÃO",   Unlocked = false },
            new() { Id = "i3",  Label = "EXPED",   Unlocked = false },
            new() { Id = "i5",  Label = "UPGR",      Unlocked = false },
            new() { Id = "i50", Label = "PESQ",    Unlocked = false },
            new() { Id = "i97", Label = "GAME",  Unlocked = false },
            new() { Id = "i98", Label = "LOJA",     Unlocked = false },
            new() { Id = "i99", Label = "CONFIG",    Unlocked = false },
            new() { Id = "i100", Label = "TIPS",   Unlocked = false },
        };
        public IEnumerable<NavItem> VisibleNav =>
            _nav
                .Where(item => item.Unlocked)
                .OrderBy(item => item.SortKey);

        public void SyncMenusFromGame(GameModel g)
        {
            g.Ui ??= new UiState();

            // garante que sempre existe HashSet no save
            g.Ui.UnlockedMenus ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in _nav)
            {
                // Regra: se o id está no save, fica desbloqueado.
                // Se não está, fica bloqueado.
                item.Unlocked = g.Ui.UnlockedMenus.Contains(item.Id);
            }

            // fallback obrigatório: se nada estava salvo ainda (jogo novo),
            // garante pelo menos Updates aberto (i5) pra não quebrar a UI:
            if (!g.Ui.UnlockedMenus.Any())
            {
                foreach (var id in _nav.Select(n => n.Id))
                    g.Ui.UnlockedMenus.Add(id);

                foreach (var item in _nav)
                    item.Unlocked = true;
            }

            // opcional: também seta qual menu está aberto visualmente
            if (!string.IsNullOrWhiteSpace(g.Ui.OpenMenuId) &&
                _nav.Any(n => n.Id == g.Ui.OpenMenuId && n.Unlocked))
            {
                OpenMenuId = g.Ui.OpenMenuId;
            }
            else
            {
                OpenMenuId = "i5";
                g.Ui.OpenMenuId = OpenMenuId;
            }

            PreviousMenuId = OpenMenuId;
            RaiseChanged();
        }

        public void SetOpenMenu(string? id)
        {
            if (OpenMenuId == id)
                return;

            PreviousMenuId = OpenMenuId;
            OpenMenuId = id;

            if (!string.IsNullOrWhiteSpace(id))
                ClearNotificationMenu(id);

            _ = _game.Mutate(g => g.Ui.OpenMenuId = id, save: true);
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
                g.Ui.UnlockedMenus ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                g.Ui.UnlockedMenus.Remove(id);
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
                g.Ui.UnlockedMenus ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                g.Ui.UnlockedMenus.Add(id);
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

            var game = _game.CurrentGame;

            switch (itemId) 
            {
                // First Unlocks
                case "FirstCharacterPurchase":
                    // Libera Menu de Expansion
                    Console.WriteLine("[UI] Expansion Menu Liberado");

                    UnlockMenu("i1");
                    if (IsHidden("expansion-basechars"))
                    {
                        ShowPanel("expansion-basechars");
                    }
                    if (IsHidden("expansion-stagechars"))
                    {
                        ShowPanel("expansion-stagechars");
                    }
                    if (IsHidden("expansion-upgrades"))
                    {
                        ShowPanel("expansion-upgrades");
                    }
                    if (IsHidden("expansion-status"))
                    {
                        ShowPanel("expansion-status");
                    }

                    SetNotificationMenu("i1");
                    break;
                case "us01":
                    // Libera Menu de Stage e de Outside Market
                    Console.WriteLine("[UI] Stage e Market Menu Liberados");

                    UnlockMenu("i2");
                    UnlockMenu("i98");

                    SetNotificationMenu("i98");
                    SetOpenMenu("i2");
                    break;
                case "ua01":
                    // Libera Menu de Expedition
                    Console.WriteLine("[UI] Expedition Menu Liberado");

                    UnlockMenu("i3");
                    if (IsHidden("expedition-gain"))
                    {
                        ShowPanel("expedition-gain");
                    }
                    if (IsHidden("expedition-status"))
                    {
                        ShowPanel("expedition-status");
                    }
                    if (IsHidden("expedition-party"))
                    {
                        ShowPanel("expedition-party");
                    }
                    if (IsHidden("expedition-toggle"))
                    {
                        ShowPanel("expedition-toggle");
                    }

                    SetNotificationMenu("i3");
                    break;
                case "GameStart":
                    // Libera Menu de Updates e de Settings
                    Console.WriteLine("[UI] Updates e Settings Menu Liberados");

                    foreach(var panel in GamePanels)
                    {
                        HidePanel(panel);
                    }

                    UnlockMenu("i5");
                    UnlockMenu("i99");
                    UnlockMenu("i100");

                    SetOpenMenu("i100");
                    SetNotificationMenu("i5");
                    SetNotificationMenu("i99");
                    break;
                case "FirstKnowledgePurchase":
                    Console.WriteLine("[UI] Tech Menu Liberado");
                    // Libera Menu de Tech

                    if (IsHidden("tech-knowledge"))
                    {
                        ShowPanel("tech-knowledge");
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
                    Console.WriteLine("[UI] Game Stats Menu Liberado");

                    if (IsHidden("game-status"))
                    {
                        ShowPanel("game-status");
                    }

                    UnlockMenu("i97");

                    SetNotificationMenu("i97");
                    break;
                case "c011":
                    if (IsHidden("up-expansion"))
                    {
                        _lore.LoreTrigger("FirstContractPurchase");
                        ShowPanel("up-expansion");
                    }
                    if(help == "2")
                    {
                        if (IsHidden("up-permanents"))
                        {
                            _lore.LoreTrigger("SecondContractPurchase");
                            ShowPanel("up-permanents");
                        }
                    }
                    if (help == "10")
                    {
                        if (IsHidden("up-objetive"))
                        {
                            _lore.LoreTrigger("ObjetiveUnlock");
                            ShowPanel("up-objetive");
                        }
                    }
                    break;
                case "um01":
                    if (IsHidden("up-expedition"))
                    {
                        _lore.LoreTrigger("FirstCapPurchase");
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
                    Console.WriteLine("[UI] Expedition Start");

                    UnlockMenu("i5");

                    SetOpenMenu("i5");
                    break;
                case "ExpeditionEnd":
                    Console.WriteLine("[UI] Expedition End");

                    SetOpenMenu("i3");

                    LockMenu("i5");
                    break;
                case "ExpansionEnd":
                    Console.WriteLine("[UI] Expansion End");

                    SetOpenMenu("i3");

                    LockMenu("i5");
                    break;

                default: break;
            }
        }

        private bool SetNotificationMenu(string menuId)
        {
            var item = _nav.FirstOrDefault(n => string.Equals(n.Id, menuId, StringComparison.OrdinalIgnoreCase));
            if (item is null) return false;
            if (item.Notification) return false;      

            item.Notification = true;
            RaiseChanged();                          
            return true;
        }

        public bool ClearNotificationMenu(string menuId)
        {
            var item = _nav.FirstOrDefault(n => string.Equals(n.Id, menuId, StringComparison.OrdinalIgnoreCase));
            if (item is null) return false;
            if (!item.Notification) return false;

            item.Notification = false;
            RaiseChanged();
            return true;
        }
        #endregion
    }

    public enum UiLogKind { Info, Lore, Error, Unlock }
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
        void Lore(string text);
        void Error(string text);
        void Unlock(string text);
    }
    public sealed class UiLogService : IUiLogService
    {
        public event Action<UiLogMessage>? OnMessage;
        private readonly ICurrentGameService _game;

        private const int MaxLog = 200;
        public UiLogService(ICurrentGameService game) { _game = game; }

        private void Emit(string text, UiLogKind kind)
        {
            var msg = new UiLogMessage { Text = text, Kind = kind, Time = DateTime.Now };

            // dispara para a tela "ao vivo"
            OnMessage?.Invoke(msg);

            // persiste no save
            _ = _game.Mutate(g =>
            {
                g.Ui ??= new UiState();
                g.Ui.LogBuffer ??= new List<UiLogMessage>();
                g.Ui.LogBuffer.Add(msg);
                if (g.Ui.LogBuffer.Count > MaxLog)
                    g.Ui.LogBuffer.RemoveRange(0, g.Ui.LogBuffer.Count - MaxLog);
            }, save: true);
        }

        public void Info(string text) => Emit(text, UiLogKind.Info);
        public void Lore(string text) => Emit(text, UiLogKind.Lore);
        public void Error(string text) => Emit(text, UiLogKind.Error);
        public void Unlock(string text) => Emit(text, UiLogKind.Unlock);
    }
}
