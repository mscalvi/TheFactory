using FurmaIdle.Models;
using System.IO;
using System.Threading.Tasks;
using static FurmaIdle.Services.UiService;

namespace FurmaIdle.Services
{
    public enum NotificationKind
    {
        None = 0,
        Info = 1,        // Laranja
        NewItem = 2,     // Azul
        Affordable = 3   // Roxo
    }

    public interface IUiService
    {
        string? OpenMenuId { get; }
        string? OpenTabId { get; }
        bool IsBusy { get; }
        string? BusyMessage { get; }
        IEnumerable<NavItem> VisibleMenu {  get; }
        IEnumerable<TabItem> VisibleTabs(string? menuId);
        string PanelClass(string classId);

        Task LoadStage(string stageId);
        void SetOpenMenu(string? id);
        void SetOpenTab(string? id);
        void NavMenuControl(string controlItem, string? helper = "");
        void SyncMenusFromGame(GameModel g);
        void RaisePulse();

        bool SetNotificationTab(string tabId, NotificationKind kind = NotificationKind.Info);
        bool ClearNotificationTab(string tabId);

        void SetBusy(string? message);
        void ClearBusy();

        event Action? Changed;
        event Action? Pulse;
        event Action? BusyChanged;

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

        #region Ui Control
        public string? OpenMenuId { get; private set; } = "i1";
        public string? OpenTabId { get; private set; } = "game-tips";
        public string? PreviousMenuId { get; private set; } = "i1";
        public sealed class NavItem
        {
            public required string Id { get; init; } = "";
            public required string Label { get; init; } = "";
            public bool Unlocked { get; set; } = false;
            public bool Notification { get; set; } = false;
            public NotificationKind NotificationKind { get; set; } = NotificationKind.None;
            public int SortKey =>
                int.TryParse(Id.AsSpan(1), out var n) ? n : int.MaxValue;
        }
        public sealed class TabItem
        {
            public required string Id { get; init; } = "";
            public required string ParentMenuId { get; init; } = "";
            public required string Label { get; init; } = "";
            public bool Unlocked { get; set; } = false;
            public bool Notification { get; set; } = false;
            public NotificationKind NotificationKind { get; set; } = NotificationKind.None;
            public int SortKey { get; init; } = 0;
        }


        private readonly HashSet<string> _hidden = new(StringComparer.Ordinal);
        private readonly List<string> GamePanels = new(AllPanels);
        public bool IsBusy { get; private set; }
        public string? BusyMessage { get; private set; }

        public event Action? BusyChanged;
        public event Action? Changed;
        public event Action? Pulse;

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
        public void RaisePulse()
        {
            Pulse?.Invoke();
        }
        private void RaiseChanged()
        {
            Changed?.Invoke();
        }
        public void SetBusy(string? message)
        {
            IsBusy = true;
            BusyMessage = message;
            BusyChanged?.Invoke();
        }
        public void ClearBusy()
        {
            IsBusy = false;
            BusyMessage = null;
            BusyChanged?.Invoke();
        }
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
                OpenMenuId = "i1";
                g.Ui.OpenMenuId = OpenMenuId;
            }

            PreviousMenuId = OpenMenuId;

            g.Ui.UnlockedTabs ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            g.Ui.TabsWithNotification ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (g.Ui.UnlockedTabs.Count == 0)
            {
                // comportamento já implícito em IsTabUnlocked: se nada foi salvo ainda,
                // considera tudo liberado (ou aqui você pode liberar só as iniciais, se preferir)
                foreach (var tab in _tabs)
                {
                    tab.Unlocked = true;
                    g.Ui.UnlockedTabs.Add(tab.Id);
                }
            }
            else
            {
                foreach (var tab in _tabs)
                {
                    tab.Unlocked = g.Ui.UnlockedTabs.Contains(tab.Id);
                }
            }

            // aplica flags de notificação salvas
            foreach (var tab in _tabs)
            {
                if (g.Ui.TabsWithNotification.Contains(tab.Id))
                {
                    tab.Notification = true;
                    // ao carregar do save antigo, assume Info por padrão
                    tab.NotificationKind = NotificationKind.Info;
                }
                else
                {
                    tab.Notification = false;
                    tab.NotificationKind = NotificationKind.None;
                }
            }



            // garante que menus reflitam notificações das tabs
            foreach (var menu in _nav)
            {
                var notifiedTabs = _tabs
                    .Where(t => t.ParentMenuId == menu.Id && t.Notification)
                    .ToList();

                if (!notifiedTabs.Any())
                {
                    menu.Notification = false;
                    menu.NotificationKind = NotificationKind.None;
                }
                else
                {
                    menu.Notification = true;
                    // pega o tipo "mais forte" entre as tabs
                    menu.NotificationKind = notifiedTabs
                        .Select(t => t.NotificationKind)
                        .DefaultIfEmpty(NotificationKind.Info)
                        .Max();
                }
            }


            RaiseChanged();
        }

        public IEnumerable<NavItem> VisibleMenu =>
            _nav
                .Where(item => item.Unlocked)
                .OrderBy(item => item.SortKey);
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
        private bool SetNotificationMenu(string menuId, NotificationKind kind)
        {
            var item = _nav.FirstOrDefault(n => string.Equals(n.Id, menuId, StringComparison.OrdinalIgnoreCase));
            if (item is null) return false;
            if (OpenMenuId == menuId) return false;

            item.Notification = true;
            item.NotificationKind = kind;
            RaiseChanged();
            return true;
        }
        public bool ClearNotificationMenu(string menuId)
        {
            var item = _nav.FirstOrDefault(n => string.Equals(n.Id, menuId, StringComparison.OrdinalIgnoreCase));
            if (item is null) return false;

            var anyTabNotified = _tabs.Any(t => t.ParentMenuId == menuId && t.Notification);
            if (anyTabNotified) return false;

            if (!item.Notification) return false;

            item.Notification = false;
            item.NotificationKind = NotificationKind.None;

            RaiseChanged();
            return true;
        }


        public IEnumerable<TabItem> VisibleTabs(string? menuId)
        {
            if (string.IsNullOrEmpty(menuId))
                return Enumerable.Empty<TabItem>();

            return _tabs
                .Where(t => t.ParentMenuId == menuId && t.Unlocked)
                .OrderBy(t => t.SortKey);
        }
        public bool IsTabUnlocked(string tabId)
        {
            var g = _game.CurrentGame;
            g.Ui ??= new UiState();
            g.Ui.UnlockedTabs ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (g.Ui.UnlockedTabs.Count == 0)
                return true;

            return g.Ui.UnlockedTabs.Contains(tabId);
        }
        public void SetOpenTab(string? id)
        {
            if (OpenTabId == id)
                return;

            OpenTabId = id;

            if (!string.IsNullOrWhiteSpace(id))
            {
                // 1) limpa a notificação da TAB
                ClearNotificationTab(id);

                // 2) encontra o menu pai e atualiza a notificação do menu
                var parent = _tabs.FirstOrDefault(t => t.Id == id)?.ParentMenuId;
                if (!string.IsNullOrWhiteSpace(parent))
                {
                    UpdateMenuNotificationFromTabs(parent);
                }
            }

            _ = _game.Mutate(g =>
            {
                g.Ui ??= new UiState();
                g.Ui.OpenTabId = id;
            }, save: true);

            RaiseChanged();
        }
        public void UnlockTab(string tabId)
        {
            var tab = _tabs.FirstOrDefault(t => string.Equals(t.Id, tabId, StringComparison.OrdinalIgnoreCase));
            if (tab is null) return;
            if (tab.Unlocked) return;

            tab.Unlocked = true;

            _ = _game.Mutate(g =>
            {
                g.Ui ??= new UiState();
                g.Ui.UnlockedTabs ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                g.Ui.UnlockedTabs.Add(tabId);
            }, save: true);

            RaiseChanged();
        }
        public void LockTab(string tabId)
        {
            var tab = _tabs.FirstOrDefault(t => string.Equals(t.Id, tabId, StringComparison.OrdinalIgnoreCase));
            if (tab is null) return;
            if (!tab.Unlocked) return;

            tab.Unlocked = false;

            _ = _game.Mutate(g =>
            {
                g.Ui ??= new UiState();
                g.Ui.UnlockedTabs ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                g.Ui.UnlockedTabs.Remove(tabId);
            }, save: true);

            RaiseChanged();
        }
        public bool SetNotificationTab(string tabId, NotificationKind kind = NotificationKind.Info)
        {
            var tab = _tabs.FirstOrDefault(t => string.Equals(t.Id, tabId, StringComparison.OrdinalIgnoreCase));
            if (tab is null) return false;
            if (OpenTabId == tabId) return false;

            if (tab.Notification && tab.NotificationKind == kind)
                return false;

            tab.Notification = true;
            tab.NotificationKind = kind;

            SetNotificationMenu(tab.ParentMenuId, kind);

            _ = _game.Mutate(g =>
            {
                g.Ui ??= new UiState();
                g.Ui.TabsWithNotification ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                g.Ui.TabsWithNotification.Add(tabId);
            }, save: true);

            RaiseChanged();
            return true;
        }
        public bool ClearNotificationTab(string tabId)
        {
            var tab = _tabs.FirstOrDefault(t => string.Equals(t.Id, tabId, StringComparison.OrdinalIgnoreCase));
            if (tab is null) return false;
            if (!tab.Notification) return false;

            tab.Notification = false;
            tab.NotificationKind = NotificationKind.None;

            _ = _game.Mutate(g =>
            {
                if (g.Ui?.TabsWithNotification is null) return;
                g.Ui.TabsWithNotification.Remove(tabId);
            }, save: true);

            UpdateMenuNotificationFromTabs(tab.ParentMenuId);

            RaiseChanged();
            return true;
        }
        private void UpdateMenuNotificationFromTabs(string parentMenuId)
        {
            var menu = _nav.FirstOrDefault(n => n.Id == parentMenuId);
            if (menu is null) return;

            var notifiedTabs = _tabs
                .Where(t => t.ParentMenuId == parentMenuId && t.Notification)
                .ToList();

            if (!notifiedTabs.Any())
            {
                menu.Notification = false;
                menu.NotificationKind = NotificationKind.None;
            }
            else
            {
                menu.Notification = true;
                menu.NotificationKind = notifiedTabs
                    .Select(t => t.NotificationKind)
                    .DefaultIfEmpty(NotificationKind.Info)
                    .Max();
            }
        }

        public void UnlockPanel(string id)
        {
            _ = _game.Mutate(g =>
            {
                g.Ui ??= new UiState();
                g.Ui.HiddenPanels.Remove(id);
            }, save: true);
            RaiseChanged();
        }
        public void LockPanel(string id)
        {
            _ = _game.Mutate(g =>
            {
                g.Ui ??= new UiState();
                g.Ui.HiddenPanels.Add(id);
            }, save: true);
            RaiseChanged();
        }
        public bool IsPanelUnlocked(string id)
        {
            var g = _game.CurrentGame;
            return g?.Ui?.HiddenPanels?.Contains(id) == true;
        }
        public string PanelClass(string id)
        {
            var cls = "menu-panel";
            if (IsPanelUnlocked(id)) cls += " is-hidden";
            return cls;
        }
        #endregion

        #region Nav Tabs
        private readonly List<NavItem> _nav = new()
        {
            new() { Id = "i1", Label = "GUILD", Unlocked = false },
            new() { Id = "i2", Label = "STAGE", Unlocked = false },
            new() { Id = "i3", Label = "WORLD", Unlocked = false },
            new() { Id = "i4", Label = "GAME", Unlocked = false },
            new() { Id = "i98", Label = "STORE", Unlocked = false },
            new() { Id = "i99", Label = "SETTING", Unlocked = false },
        };

        private readonly List<TabItem> _tabs = new()
        {
            // GUILD (i1)
            new() { Id = "guild-hall",      ParentMenuId = "i1", Label = "Gerência", SortKey = 10 },
            new() { Id = "guild-contracts", ParentMenuId = "i1", Label = "Contratos", SortKey = 20 },
            new() { Id = "guild-lab",       ParentMenuId = "i1", Label = "Laboratório", SortKey = 30 },
            new() { Id = "guild-members",   ParentMenuId = "i1", Label = "Membros", SortKey = 40 },

            // STAGE (i2)
            new() { Id = "stage-expedition", ParentMenuId = "i2", Label = "Expedição", SortKey = 10 },
            new() { Id = "stage-locals",     ParentMenuId = "i2", Label = "Lugares",   SortKey = 20 },
            new() { Id = "stage-dock",       ParentMenuId = "i2", Label = "Doca",      SortKey = 30 },

            // WORLD (i3)
            new() { Id = "world-expansion", ParentMenuId = "i3", Label = "Expansão",   SortKey = 10 },
            new() { Id = "world-influence", ParentMenuId = "i3", Label = "Influência", SortKey = 20 },
            new() { Id = "world-ships",     ParentMenuId = "i3", Label = "Navios",     SortKey = 30 },
            new() { Id = "world-map",       ParentMenuId = "i3", Label = "Mapa",       SortKey = 40 },

            // GAME (i4)
            new() { Id = "game-info",  ParentMenuId = "i4", Label = "Informações", SortKey = 10 },
            new() { Id = "game-arch",  ParentMenuId = "i4", Label = "Archievments", SortKey = 20 },
            new() { Id = "game-tips",  ParentMenuId = "i4", Label = "Manual", SortKey = 30 },
        };

        private static readonly string[] AllPanels = {
            "guild-contracts-fast",
            "guild-contracts-long",
        };
        #endregion

        #region Menu Control
        public void NavMenuControl(string controlItem, string? helper = "")
        {
            var game = _game.CurrentGame;

            switch (controlItem)
            {
                #region Stage 0
                case "GameCreation":
                    foreach(var panel in AllPanels)
                    {
                        LockPanel(panel);
                    }

                    // Menu: Settings
                    UnlockMenu("i99");

                    // Menu: Game > Tips
                    UnlockMenu("i4");
                    UnlockTab("game-tips");

                    SetOpenMenu("i4");
                    SetOpenTab("game-tips");

                    _lore.LoreTrigger(controlItem);
                    // Tips: Introdução
                    // Tips: Conceito do Jogo
                    // Tips: Clicks

                    // Animation: piscar imagem central e o marcador de moedas.
                    break;
                case "FirstClick":
                    _lore.LoreTrigger(controlItem);

                    // Tips: Moedas
                    break;
                case "10thClick":
                    // Menu: Guild > Contracts
                    UnlockMenu("i1");
                    UnlockTab("guild-contracts");
                    UnlockPanel("guild-contracts-fast");

                    SetOpenMenu("i1");
                    SetOpenTab("guild-contracts");

                    _lore.LoreTrigger(controlItem);

                    // Tips: Melhorias
                    SetNotificationTab("game-tips");

                    // Animation: piscar o navmenu e o contractsmenu
                    break;
                case "20thClick":
                    UnlockPanel("guild-contracts-long");

                    _lore.LoreTrigger(controlItem);

                    // Tips: Permanência
                    SetNotificationTab("game-tips");

                    // Animation: piscar guild-contracts no navmenu
                    break;
                case "ContractLevel0Unlock":
                    _lore.LoreTrigger(controlItem);

                    // Tips: Contract Level
                    SetNotificationTab("game-tips");

                    // Animation: piscar o Contract Menu e o Nível 0
                    break;
                case "FirstContract0Purchase":
                    _lore.LoreTrigger(controlItem);

                    // Tips: Contratos
                    SetNotificationTab("game-tips");
                    break;
                case "5xContract0Purchase":
                    _lore.LoreTrigger(controlItem);

                    // Tips: Contract Cap
                    SetNotificationTab("game-tips");

                    break;
                case "ContractLevel1Unlock":
                    // Menus: Guild > Hall
                    UnlockTab("guild-hall");
                    SetNotificationTab("guild-hall");

                    _lore.LoreTrigger(controlItem);

                    // Tips: Permanência
                    SetNotificationTab("game-tips");
                    break;
                case "FirstContractUnlock":
                    _lore.LoreTrigger(controlItem);
                    break;
                case "FirstContract1Purchase":
                    _lore.LoreTrigger(controlItem);
                    break;
                case "14Contract1Purchase":
                    _lore.LoreTrigger(controlItem);
                    break;
                #endregion

                #region Stage 1
                case "Stage1Start":
                    SetOpenTab("game-tips");
                    SetNotificationTab("guild-hall");
                    SetNotificationTab("guild-contracts");

                    // Menu: Game > Archievments TODO

                    // Menu: Game > Info
                    UnlockTab("game-info");
                    SetNotificationTab("game-info");

                    _lore.LoreTrigger(controlItem, helper);
                    break;
                case "FirstCharacterUnlock":
                    // Menu: Guild > Members
                    UnlockTab("guild-members");
                    SetNotificationTab("guild-members");

                    _lore.LoreTrigger(controlItem, helper);

                    // Tips: Characters
                    // Tips: Traits
                    SetNotificationTab("game-tips");

                    // Animation: piscar o Expansion Menu e (se tiver aberto) os Resources
                    break;
                case "FirstExpeditionUnlock":
                    // Menu: Stage > Expedition
                    UnlockMenu("i2");
                    UnlockTab("stage-expedition");
                    SetNotificationTab("stage-expedition");

                    _lore.LoreTrigger(controlItem, helper);

                    // Tips: Expedition
                    SetNotificationTab("game-tips");

                    // Animation: piscar ExpeditonMenu
                    break;
                case "FirstExpeditionComplete":
                    // Menu: Guild > Lab
                    UnlockTab("guild-lab");
                    SetNotificationTab("guild-lab");

                    _lore.LoreTrigger(controlItem, helper);

                    // Tips: Knowledge
                    SetNotificationTab("game-tips");
                    break;
                case "FirstKnowledgeUnlock":
                    // Menu: Stage > Locals
                    UnlockTab("stage-locals");
                    SetNotificationTab("stage-locals");
                    _lore.LoreTrigger(controlItem, helper);

                    // Tips: Techs
                    SetNotificationTab("game-tips");
                    break;
                case "FirstTechUnlock":
                    // Menu: World > Expansion
                    UnlockMenu("i3");
                    UnlockTab("world-expansion");
                    SetNotificationTab("world-expansion");

                    _lore.LoreTrigger(controlItem, helper);

                    // Tips: TechUpgrades
                    SetNotificationTab("game-tips");
                    break;
                case "FirstResourceUnlock":
                    _lore.LoreTrigger(controlItem, helper);

                    // Tips: Resources
                    // Tips: Specialties
                    SetNotificationTab("game-tips");

                    // Animation: piscar os Recursos e as Specialties
                    break;
                case "GuildTip":
                    // Tips: Guild
                    SetNotificationTab("game-tips");
                    break;
                case "BaseTip":
                    // Tips: Base
                    SetNotificationTab("game-tips");
                    break;
                case "FirstExpansionUnlock":
                    _lore.LoreTrigger(controlItem, helper);

                    // Tips: Expansion
                    SetNotificationTab("game-tips");
                    break;
                case "FirstShipUnlock":
                    // Menu: World > Dock
                    UnlockTab("stage-dock");
                    SetNotificationTab("stage-dock");

                    _lore.LoreTrigger(controlItem, helper);
                    break;
                case "FirstStageUnlock":
                    // Menu: World > Map
                    UnlockTab("world-map");
                    SetNotificationTab("world-map");

                    _lore.LoreTrigger(controlItem, helper);

                    // Tips: Stages
                    SetNotificationTab("game-tips");
                    break;
                #endregion

                #region Gerais
                case "LocalUnlock":
                    _lore.LoreTrigger(controlItem, helper);

                    // tips: Locals
                    break;
                case "CharacterUnlock":
                    _lore.LoreTrigger(controlItem, helper);

                    break;
                case "SpecialtyUsed":
                    _lore.LoreTrigger(controlItem, helper);

                    break;
                case "ExpeditionStart":
                    break;
                case "ExpeditionEnd":
                    break;
                case "ExpansionEnd":
                    break;
                #endregion
                default: break;
            }
        }
        #endregion
    }

    public interface IUiLogService
    {
        event Action<UiLogMessage>? OnMessage;
        void Info(string text);
        void Lore(string text);
        void Error(string text);

        // Characters
        void Ferri(string text);
        void Maik(string text);
        void Claimi(string text);
        void Alan(string text);
        void Jaime (string text);
        void Yg(string text);
    }
    public enum UiLogKind { Error, Info, Lore, Ferri, Maik, Claimi, Alan, Jaime, Yg }
    public sealed class UiLogMessage
    {
        public DateTime Time { get; init; } = DateTime.Now;
        public string Text { get; init; } = "";
        public UiLogKind Kind { get; init; } = UiLogKind.Info;
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
        public void Ferri(string text) => Emit(text, UiLogKind.Ferri);
        public void Maik(string text) => Emit(text, UiLogKind.Maik);
        public void Claimi(string text) => Emit(text, UiLogKind.Claimi);
        public void Alan(string text) => Emit(text, UiLogKind.Alan);
        public void Jaime(string text) => Emit(text, UiLogKind.Jaime);
        public void Yg(string text) => Emit(text, UiLogKind.Yg);
    }
}
