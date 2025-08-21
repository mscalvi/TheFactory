using System;
using System.IO;
using System.Text.Json;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using WinFormsTimer = System.Windows.Forms.Timer;

namespace HardStopGuardian
{
    public partial class TrayHost : Form
    {
        private NotifyIcon _tray;
        private ContextMenuStrip _menu;

        private WinFormsTimer _startupTimer;
        private WinFormsTimer _tMinus10Timer;
        private WinFormsTimer _t0Timer;
        
        private DateTime _nextHardStop;
        
        private bool _armed;
        private bool _pendingOverlay;
        private int _blockMinutes = 5; // cresce +5 até 30

        [DllImport("user32.dll")]
        private static extern bool LockWorkStation();

        private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string RunValueName = "HardStopGuardian";

        public TrayHost()
        {
            InitializeComponent();

            SetupTray();
            HideOnStart();

            SetupStartupPopup();

            var resumed = TryResumeFromState();
            if (resumed)
                _startupTimer?.Stop(); // evita pop-up de 30s ao retomar estado já armado


            SystemEvents.SessionSwitch += OnSessionSwitch;
        }

        private void SetupTray()
        {
            _menu = new ContextMenuStrip();

            var autoItem = new ToolStripMenuItem("Iniciar com Windows")
            {
                Checked = IsAutoStartEnabled(),
                CheckOnClick = true
            };
            autoItem.CheckedChanged += (s, e) => SetAutoStart(autoItem.Checked);

            _menu.Items.Add(autoItem);
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add("Ligar", null, (s, e) => PromptAndArm());
            _menu.Items.Add("Definir hora", null, (s, e) => PromptAndArm());
            _menu.Items.Add("Standby", null, (s, e) => Standby());
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add("Sair", null, (s, e) => Application.Exit());

            _tray = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Visible = true,
                Text = "HardStop Guardian",
                ContextMenuStrip = _menu
            };
        }


        private void HideOnStart()
        {
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
            Opacity = 0;
            Hide();
        }

        private void SetupStartupPopup()
        {
            _startupTimer = new WinFormsTimer { Interval = 30_000 };
            _startupTimer.Tick += (s, e) =>
            {
                _startupTimer.Stop();
                ShowStartupPopup();
            };
            _startupTimer.Start();
        }

        private void ShowStartupPopup()
        {
            var r = MessageBox.Show("Ligar Hard-Stop?", "HardStop Guardian",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (r == DialogResult.Yes) PromptAndArm();
        }

        private void PromptAndArm()
        {
            using var dlg = new TimePromptForm();
            dlg.StartPosition = FormStartPosition.CenterScreen;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                Arm(dlg.SelectedTime);
            }
        }

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

        private static DateTime NormalizeToNextOccurrence(DateTime t)
        {
            var now = DateTime.Now;
            var todayAt = new DateTime(now.Year, now.Month, now.Day, t.Hour, t.Minute, 0);
            return (todayAt <= now) ? todayAt.AddDays(1) : todayAt;
        }

        private void ScheduleTimers()
        {
            var now = DateTime.Now;
            var tMinus10 = _nextHardStop - TimeSpan.FromMinutes(10);

            int ms10 = (int)Math.Max((tMinus10 - now).TotalMilliseconds, 1000);
            int ms0 = (int)Math.Max((_nextHardStop - now).TotalMilliseconds, 1000);

            // (re)cria timers a cada armada, simples e sem ambiguidade
            _tMinus10Timer?.Stop();
            _t0Timer?.Stop();

            _tMinus10Timer = new WinFormsTimer { Interval = ms10 };
            _t0Timer = new WinFormsTimer { Interval = ms0 };

            _tMinus10Timer.Tick += (s, e) => { _tMinus10Timer.Stop(); ShowTMinus10Toast(); };
            _t0Timer.Tick += (s, e) => { _t0Timer.Stop(); ExecuteHardStop(); };

            _tMinus10Timer.Start();
            _t0Timer.Start();
        }

        private void ShowTMinus10Toast()
        {
            _tray.BalloonTipTitle = "Faltam 10 min";
            _tray.BalloonTipText = "Salve seu progresso.";
            _tray.ShowBalloonTip(5000);
        }

        private void ExecuteHardStop()
        {
            _pendingOverlay = true;
            LockWorkStation(); // bloqueia agora; ao desbloquear, mostra overlay
        }

        private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionUnlock && _pendingOverlay)
            {
                _pendingOverlay = false;
                if (InvokeRequired)
                    BeginInvoke(new Action(ShowOverlay));
                else
                    ShowOverlay();
            }
        }

        private void ShowOverlay()
        {
            var overlay = new OverlayForm(_blockMinutes);
            overlay.FormClosed += (s, e) => AfterBlock();
            overlay.Show();
        }

        private void AfterBlock()
        {
            var r = MessageBox.Show("Manter ativado?", "HardStop Guardian", MessageBoxButtons.YesNo);
            if (r == DialogResult.Yes)
            {
                _blockMinutes = Math.Min(_blockMinutes + 5, 30);
                _nextHardStop = DateTime.Now.AddMinutes(5);
                _armed = true;
                ScheduleTimers();
                SaveState();
            }
            else
            {
                _tMinus10Timer?.Stop();
                _t0Timer?.Stop();
                _armed = false;
                SaveState();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            SaveState();
            Microsoft.Win32.SystemEvents.SessionSwitch -= OnSessionSwitch;
            _tray?.Dispose();
            base.OnFormClosed(e);
        }

        private void SaveState()
        {
            Persistence.Save(new StateModel
            {
                Armed = _armed,
                NextHardStop = _nextHardStop,
                BlockMinutes = _blockMinutes
            });
        }

        private bool TryResumeFromState()
        {
            var st = Persistence.Load();
            if (st == null) return false;

            _blockMinutes = Math.Clamp(st.BlockMinutes, 5, 30);

            if (st.Armed)
            {
                // Normaliza para hoje (ou amanhã) no mesmo horário salvo
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

                    return true;    // mantém seu _startupTimer?.Stop();
                }
                else
                {
                    _armed = false;
                    SaveState();    // persiste que NÃO vai retomar
                    return false;   // mantém pop-up de 30s ativo
                }
            }

            return false;
        }

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

        private static bool IsAutoStartEnabled()
        {
            using var rk = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
            var val = rk?.GetValue(RunValueName) as string;
            return !string.IsNullOrEmpty(val);
        }

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

    }
}
