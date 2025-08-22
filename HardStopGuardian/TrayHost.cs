using System;
using System.IO;
using System.Text.Json;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;                // abrir Notepad
using System.Collections.Generic;
using System.Runtime.InteropServices;   // DllImport LockWorkStation
using Microsoft.Win32;                  // SystemEvents + HKCU\Run
using WinFormsTimer = System.Windows.Forms.Timer;

namespace HardStopGuardian
{
    public partial class TrayHost : Form
    {
        // --- Ícone de bandeja e menu ---
        private NotifyIcon _tray;
        private ContextMenuStrip _menu;

        // --- Timers do fluxo principal (startup/T-10/T0) ---
        private WinFormsTimer _startupTimer;
        private WinFormsTimer _tMinus10Timer;
        private WinFormsTimer _t0Timer;

        // --- Estado do hard-stop ---
        private DateTime _nextHardStop;
        private bool _armed;             // se há hard-stop armado
        private bool _pendingOverlay;    // se deve abrir overlay após desbloquear
        private int _blockMinutes = 5;   // duração atual do bloqueio (5→30, +5 por ciclo)

        // --- Auto-start (HKCU\Run) ---
        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string RunValueName = "HardStopGuardian";

        // --- Checkups diários ---
        private List<CheckupItem> _checkups;
        private WinFormsTimer _checkTimer;
        private bool _checkDialogOpen;   // evita sobreposição de diálogos

        // --- WinAPI: bloquear estação (tela de login) ---
        [DllImport("user32.dll")]
        private static extern bool LockWorkStation();

        // =========================
        // CONSTRUTOR
        // =========================
        public TrayHost()
        {
            InitializeComponent();

            SetupTray();           // cria ícone e menu
            HideOnStart();         // app fica oculto (só bandeja)
            SetupStartupPopup();   // agenda pop-up inicial (insistente)

            // Retomar estado salvo (com confirmação "Retomar?")
            var resumed = TryResumeFromState();
            if (resumed)
                _startupTimer?.Stop(); // evita pop-up de 30s quando já retomou

            // Carrega checkups e inicia varredura
            _checkups = CheckupStore.Load();
            StartCheckScheduler();

            // Ouve desbloqueios de sessão para mostrar overlay após LockWorkStation
            SystemEvents.SessionSwitch += OnSessionSwitch;
        }

        // =========================
        // UI DE BANDEJA
        // =========================
        // Cria ícone de bandeja e menu de contexto
        private void SetupTray()
        {
            var autoItem = new ToolStripMenuItem("Iniciar com Windows")
            {
                Checked = IsAutoStartEnabled(),
                CheckOnClick = true
            };
            autoItem.CheckedChanged += (s, e) => SetAutoStart(autoItem.Checked);

            _menu = new ContextMenuStrip();
            _menu.Items.Add(autoItem);
            _menu.Items.Add(new ToolStripMenuItem("Editar Checkups", null, (s, e) => OpenCheckupsFile()));
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add("Ligar/Editar", null, (s, e) => PromptAndArm()); // define ou reprograma o hard-stop
            _menu.Items.Add("Pause", null, (s, e) => Standby());             // cancela timers e salva standby
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add("Encerrar", null, (s, e) => Application.Exit());

            _tray = new NotifyIcon
            {
                Icon = SystemIcons.Application, // troque por um .ico seu depois
                Visible = true,
                Text = "HardStop Guardian",
                ContextMenuStrip = _menu
            };
        }

        // Oculta a janela principal (o host é invisível)
        private void HideOnStart()
        {
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
            Opacity = 0;
            Hide();
        }

        // =========================
        // POP-UP INICIAL (insistente)
        // =========================
        // Agenda pop-up em ~20s após iniciar
        private void SetupStartupPopup()
        {
            _startupTimer = new WinFormsTimer { Interval = 20_000 };
            _startupTimer.Tick += (s, e) =>
            {
                _startupTimer.Stop();
                ShowStartupPopup();
            };
            _startupTimer.Start();
        }

        // Mostra pop-up “Ligar Hard-Stop?” insistente
        private void ShowStartupPopup()
        {
            using var dlg = new CountdownPromptForm(
                "HardStop Guardian",
                "Ligar Hard-Stop?",
                "Sim", "Não",
                5,
                insistent: true
            );
            var r = dlg.ShowDialog();
            if (r == DialogResult.OK) PromptAndArm();
        }

        // =========================
        // ARMAR / EDITAR HARD-STOP
        // =========================
        // Confirmação + seletor de hora (pré-preenche se já armado)
        private void PromptAndArm()
        {
            using (var confirm = new CountdownPromptForm(
                "HardStop Guardian",
                _armed ? $"Reagendar de {_nextHardStop:HH:mm} para…" : "Definir um hard-stop para hoje:",
                "Continuar", "Cancelar",
                3,
                insistent: true
            ))
            {
                if (confirm.ShowDialog() != DialogResult.OK) return;
            }

            using var dlg = new TimePromptForm { StartPosition = FormStartPosition.CenterScreen };
            if (_armed) dlg.SetInitialTime(_nextHardStop); // método simples no TimePromptForm
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Arm(dlg.SelectedTime); // agenda T-10/T0, marca _armed e salva
            }
        }

        // Calcula horário, exibe balão, agenda timers e salva estado
        private void Arm(DateTime chosenTime)
        {
            _nextHardStop = NormalizeToNextOccurrence(chosenTime);

            _tray.BalloonTipTitle = "Hard-Stop armado";
            _tray.BalloonTipText = $"T0: {_nextHardStop:HH:mm}  •  Aviso: {(_nextHardStop - TimeSpan.FromMinutes(10)):HH:mm}";
            _tray.ShowBalloonTip(4000);

            ScheduleTimers();

            _armed = true;
            SaveState();
        }

        // Converte um DateTime escolhido para a próxima ocorrência (hoje ou amanhã)
        private static DateTime NormalizeToNextOccurrence(DateTime t)
        {
            var now = DateTime.Now;
            var todayAt = new DateTime(now.Year, now.Month, now.Day, t.Hour, t.Minute, 0);
            return (todayAt <= now) ? todayAt.AddDays(1) : todayAt;
        }

        // =========================
        // AGENDAMENTO T-10 E T0
        // =========================
        // Cria timers para o aviso T-10 e a execução T0
        private void ScheduleTimers()
        {
            var now = DateTime.Now;
            var tMinus10 = _nextHardStop - TimeSpan.FromMinutes(10);

            int ms10 = (int)Math.Max((tMinus10 - now).TotalMilliseconds, 1000);
            int ms0 = (int)Math.Max((_nextHardStop - now).TotalMilliseconds, 1000);

            _tMinus10Timer?.Stop();
            _t0Timer?.Stop();

            _tMinus10Timer = new WinFormsTimer { Interval = ms10 };
            _t0Timer = new WinFormsTimer { Interval = ms0 };

            _tMinus10Timer.Tick += (s, e) => { _tMinus10Timer.Stop(); ShowTMinus10Toast(); };
            _t0Timer.Tick += (s, e) => { _t0Timer.Stop(); ExecuteHardStop(); };

            _tMinus10Timer.Start();
            _t0Timer.Start();
        }

        // Balão “faltam 10 min”
        private void ShowTMinus10Toast()
        {
            _tray.BalloonTipTitle = "Faltam 10 min";
            _tray.BalloonTipText = "Salve seu progresso.";
            _tray.ShowBalloonTip(5000);
        }

        // Em T0: bloqueia a sessão; ao desbloquear abrirá overlay
        private void ExecuteHardStop()
        {
            _pendingOverlay = true;
            LockWorkStation();
        }

        // =========================
        // OVERLAY PÓS-DESBLOQUEIO
        // =========================
        // Ao desbloquear, se havia _pendingOverlay, mostra overlay
        private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionUnlock && _pendingOverlay)
            {
                _pendingOverlay = false;
                if (InvokeRequired) BeginInvoke(new Action(ShowOverlay));
                else ShowOverlay();
            }
        }

        // Exibe overlay com contador (_blockMinutes) e, ao fechar, decide próximo passo
        private void ShowOverlay()
        {
            var overlay = new OverlayForm(_blockMinutes);
            overlay.FormClosed += (s, e) => AfterBlock();
            overlay.Show();
        }

        // Após overlay: “Manter?” (insistente). Sim → +5 min (até 30) e rearmar; Não → standby
        private void AfterBlock()
        {
            using var dlg = new CountdownPromptForm(
                "HardStop Guardian",
                "Manter ativado?",
                "Sim", "Não",
                10,
                insistent: true
            );
            var r = dlg.ShowDialog();

            if (r == DialogResult.OK) // Sim
            {
                _blockMinutes = Math.Min(_blockMinutes + 5, 30);
                _nextHardStop = DateTime.Now.AddMinutes(5);
                _armed = true;
                ScheduleTimers();
                SaveState();
            }
            else // Não
            {
                _tMinus10Timer?.Stop();
                _t0Timer?.Stop();
                _armed = false;
                SaveState();
            }
        }

        // =========================
        // CICLO DE VIDA / LIMPEZA
        // =========================
        // Ao fechar: salva estado, desinscreve eventos, libera recursos
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            SaveState();
            SystemEvents.SessionSwitch -= OnSessionSwitch;

            _startupTimer?.Stop();
            _tMinus10Timer?.Stop();
            _t0Timer?.Stop();
            _checkTimer?.Stop();

            _tray?.Dispose();
            base.OnFormClosed(e);
        }

        // =========================
        // PERSISTÊNCIA DE ESTADO
        // =========================
        // Salva estado mínimo em %AppData%\HardStopGuardian\state.json
        private void SaveState()
        {
            Persistence.Save(new StateModel
            {
                Armed = _armed,
                NextHardStop = _nextHardStop,
                BlockMinutes = _blockMinutes
            });
        }

        // Tenta retomar estado salvo; pergunta “Retomar?”; agenda se aceitar
        private bool TryResumeFromState()
        {
            var st = Persistence.Load();
            if (st == null) return false;

            _blockMinutes = Math.Clamp(st.BlockMinutes, 5, 30);

            if (st.Armed)
            {
                var t0 = NormalizeToNextOccurrence(st.NextHardStop);
                var ans = MessageBox.Show(
                    $"Retomar hard-stop para hoje às {t0:HH:mm}?",
                    "Retomar?",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (ans == DialogResult.Yes)
                {
                    _armed = true;
                    _nextHardStop = t0;
                    ScheduleTimers();

                    _tray.BalloonTipTitle = "Hard-Stop retomado";
                    _tray.BalloonTipText = $"T0: {_nextHardStop:HH:mm}";
                    _tray.ShowBalloonTip(3000);
                    return true;
                }
                else
                {
                    _armed = false;
                    SaveState(); // grava decisão de não retomar
                    return false;
                }
            }
            return false;
        }

        // Coloca em standby (cancela timers e salva)
        private void Standby()
        {
            _tMinus10Timer?.Stop();
            _t0Timer?.Stop();
            _armed = false;
            SaveState();

            _tray.BalloonTipTitle = "Standby";
            _tray.BalloonTipText = "Hard-Stop desativado.";
            _tray.ShowBalloonTip(3000);
        }

        // =========================
        // AUTO-START (opcional)
        // =========================
        // Checa se há valor em HKCU\...\Run
        private static bool IsAutoStartEnabled()
        {
            using var rk = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
            var val = rk?.GetValue(RunValueName) as string;
            return !string.IsNullOrEmpty(val);
        }

        // Liga/desliga auto-start para o caminho atual do EXE
        private static void SetAutoStart(bool enable)
        {
            var exe = Application.ExecutablePath;
            using var rk = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true)
                         ?? Registry.CurrentUser.CreateSubKey(RunKeyPath);
            if (enable)
                rk.SetValue(RunValueName, $"\"{exe}\"");  // aspas para caminhos com espaço
            else
                rk.DeleteValue(RunValueName, throwOnMissingValue: false);
        }

        // =========================
        // CHECKUPS DIÁRIOS
        // =========================
        // Inicia o varredor (a cada 15s) para disparar checkups
        private void StartCheckScheduler()
        {
            _checkTimer = new WinFormsTimer { Interval = 15_000 };
            _checkTimer.Tick += (s, e) => PollCheckups();
            _checkTimer.Start();
        }

        // Verifica se algum checkup venceu (janela de 60s) e dispara 1 por vez
        private void PollCheckups()
        {
            if (_checkDialogOpen) return;

            var now = DateTime.Now;
            foreach (var it in _checkups)
            {
                if (!it.Enabled) continue;

                var due = new DateTime(now.Year, now.Month, now.Day, it.Time.Hours, it.Time.Minutes, 0);

                // já feito hoje?
                if (it.LastDoneDate.HasValue && it.LastDoneDate.Value.Date == now.Date) continue;

                // janela de disparo de ~60s
                if (now >= due && (now - due).TotalSeconds <= 60)
                {
                    _checkDialogOpen = true;
                    BeginInvoke(new Action(() => FireCheckup(it)));
                    return;
                }
            }
        }

        // Inicia o ciclo do checkup (Sim/Não)
        private void FireCheckup(CheckupItem it)
        {
            _checkDialogOpen = true;
            ShowCheckupPrompt(it);
        }

        // Pop-up incisiva "Sim/Não"; se "Não", bloqueia 1 min e repete até "Sim"
        private void ShowCheckupPrompt(CheckupItem it)
        {
            try
            {
                using var dlg = new CountdownPromptForm(
                    "Checklist",
                    $"{it.Label} agora?",
                    "Sim", "Não",
                    0,
                    insistent: true
                );
                var r = dlg.ShowDialog();

                if (r == DialogResult.OK) // Sim
                {
                    it.LastDoneDate = DateTime.Now.Date;
                    CheckupStore.Save(_checkups);
                    _tray.BalloonTipTitle = "Anotado";
                    _tray.BalloonTipText = $"{it.Label} marcado como feito.";
                    _tray.ShowBalloonTip(2000);
                    _checkDialogOpen = false;
                }
                else // Não → bloqueia 1 min e repete
                {
                    BlockAndRepeatCheckup(it);
                }
            }
            catch
            {
                _checkDialogOpen = false; // não travar em caso de erro
            }
        }

        // Faz overlay de 1 minuto; ao terminar, volta a perguntar o mesmo checkup
        private void BlockAndRepeatCheckup(CheckupItem it)
        {
            var overlay = new OverlayForm(1);
            overlay.FormClosed += (s, e) => ShowCheckupPrompt(it);
            overlay.Show();
        }

        // Abre checkups.json no Notepad; ao fechar, recarrega
        private void OpenCheckupsFile()
        {
            try
            {
                _ = CheckupStore.Load(); // garante existência do arquivo

                var psi = new ProcessStartInfo("notepad.exe", CheckupStore.PathForUser)
                {
                    UseShellExecute = false
                };
                var p = Process.Start(psi);
                if (p != null)
                {
                    p.EnableRaisingEvents = true;
                    p.Exited += (s, e) =>
                    {
                        if (IsHandleCreated)
                            BeginInvoke(new Action(ReloadCheckups));
                    };
                }
            }
            catch
            {
                // silencioso
            }
        }

        // Recarrega lista de checkups do disco e mostra balão de confirmação
        private void ReloadCheckups()
        {
            try
            {
                var newList = CheckupStore.Load();
                _checkups = newList;
                _tray.BalloonTipTitle = "Checkups recarregados";
                _tray.BalloonTipText = $"{_checkups.Count} item(ns) carregado(s).";
                _tray.ShowBalloonTip(2000);
            }
            catch (Exception ex)
            {
                _tray.BalloonTipTitle = "Erro ao recarregar checkups";
                _tray.BalloonTipText = ex.Message;
                _tray.ShowBalloonTip(4000);
            }
        }
    }
}
