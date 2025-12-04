using FurmaIdle.Models;
using System.IO;
using System.Threading.Tasks;
using static FurmaIdle.Services.UiService;

namespace FurmaIdle.Services
{
    public interface IUiService
    {
        string? OpenMenuId { get; }
        bool IsBusy { get; }
        string? BusyMessage { get; }
        IEnumerable<NavItem> VisibleNav {  get; }

        Task LoadStage(string stageId);
        void SetOpenMenu(string? id);
        void NavMenuControl(string controlItem,string? helper = "");
        void SyncMenusFromGame(GameModel g);
        string PanelClass(string classId);
        void RaisePulse();
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

        public string? OpenMenuId { get; private set; } = "i3";
        public string? PreviousMenuId { get; private set; } = "i3";
        public sealed class NavItem
        {
            public required string Id { get; init; } = "";
            public required string Label { get; init; } = "";
            public bool Unlocked { get; set; } = true;
            public bool Notification { get; set; } = false;
            public int SortKey =>
                int.TryParse(Id.AsSpan(1), out var n) ? n : int.MaxValue;
        }

        public IEnumerable<NavItem> VisibleNav =>
            _nav
                .Where(item => item.Unlocked)
                .OrderBy(item => item.SortKey);
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
                OpenMenuId = "i3";
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
        public bool IsPanelHidden(string id)
        {
            var g = _game.CurrentGame;
            return g?.Ui?.HiddenPanels?.Contains(id) == true;
        }
        public string PanelClass(string id)
        {
            var cls = "menu-panel";
            if (IsPanelHidden(id)) cls += " is-hidden";
            return cls;
        }
        #endregion

        #region Nav Tabs
        private readonly List<NavItem> _nav = new()
        {
            new() { Id = "i1",  Label = "GUILD",   Unlocked = true },
            new() { Id = "i2",  Label = "WORLD",   Unlocked = true },
            new() { Id = "i3",  Label = "GAME",   Unlocked = true },
            new() { Id = "i98",  Label = "STORE",      Unlocked = true },
            new() { Id = "i99",  Label = "SETTING",      Unlocked = true },
        };
        #endregion

        #region Menu Sub Panels
        private static readonly string[] AllPanels = {
            "expedition-toggle",
            "expedition-party",
            "expedition-status",
            "expedition-gain",
            "info-chars",
            "expansion-info",
            "expansion-history",
            "expansion-objetive",
            "hall-permanents",
            "hall-expansion",
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
                    foreach (var panel in GamePanels)
                    {
                        // HidePanel(panel);
                    }

                    // Menu: Settings
                    // Menu: Game > Tips
                    // Menu: Game > Info

                    _lore.LoreTrigger(controlItem);

                    // Animation: piscar imagem central e o marcador de moedas.
                    // Tips: Introdução
                    // Tips: Conceito do Jogo
                    // Tips: Clicks
                    break;
                case "FirstClick":
                    _lore.LoreTrigger(controlItem);

                    // Tips: Moedas
                    break;
                case "10thClick":                    
                    // Menus: Guild > Guild Hall

                    _lore.LoreTrigger(controlItem);

                    // Animation: piscar o upgrade menu e o expedition panel
                    // Tips: Melhorias
                    break;
                case "20thClick":
                    // Menus: Guild > Gerência
                    
                    _lore.LoreTrigger(controlItem);

                    // Animation: piscar painel de expansion upgrades
                    // Tips: Permanência
                    break;
                case "ContractLevel0Unlock":
                    _lore.LoreTrigger(controlItem);

                    // Animation: piscar o Contract Menu e o Nível 0
                    // Tips: Contract Level
                    break;
                case "FirstContract0Purchase":
                    _lore.LoreTrigger(controlItem);

                    // Tips: Contratos
                    break;
                case "5xContract0Purchase":
                    _lore.LoreTrigger(controlItem);

                    // Tips: Contract Cap
                    break;
                case "ContractLevel1Unlock":
                    _lore.LoreTrigger(controlItem);
                    break;
                case "FirstContractUnlock":
                    _lore.LoreTrigger(controlItem);
                    break;
                case "FirstContract1Purchase":
                    // Menu: Guild > Expansão

                    _lore.LoreTrigger(controlItem);
                    break;
                case "14Contract1Purchase":
                    _lore.LoreTrigger(controlItem);
                    break;
                #endregion

                #region Stage 1
                case "Stage1Start":
                    // Menu: Game > Archievments

                    _lore.LoreTrigger(controlItem, helper);
                    break;
                case "FirstCharacterUnlock":
                    // Menu: Guild > Guild Info

                    _lore.LoreTrigger(controlItem, helper);

                    // Animation: piscar o Expansion Menu e (se tiver aberto) os Resources
                    // Tips: Characters
                    // Tips: Traits
                    break;
                case "FirstResourceUnlock":
                    _lore.LoreTrigger(controlItem, helper);

                    // Animation: piscar os Recursos e as Specialties
                    // Tips: Resources
                    // Tips: Specialties
                    break;
                case "FirstExpeditionUnlock":
                    // Menu: Mundo > Expedição

                    _lore.LoreTrigger(controlItem, helper);

                    // Animation: piscar ExpeditonMenu
                    // Tips: Expedition
                    break;
                case "FirstKnowledgeUnlock":
                    // Menu: Mundo > Laboratório

                    _lore.LoreTrigger(controlItem, helper);
                    break;
                case "FirstExpansionUnlock":
                    _lore.LoreTrigger(controlItem, helper);

                    // Tips: Expansion
                    break;
                case "FirstTechUnlock":
                    _lore.LoreTrigger(controlItem, helper);

                    // Tips: Techs
                    break;
                case "FirstShipUnlock":
                    // Menu: Mundo > Doca

                    _lore.LoreTrigger(controlItem, helper);
                    break;
                case "FirstStageUnlock":
                    // Menu: Mundo > Mapa

                    _lore.LoreTrigger(controlItem, helper);

                    // Tips: Stages
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
