using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FurmaIdle.Models;
using Microsoft.Extensions.Logging;

namespace FurmaIdle.Services
{
    // Quem quer receber ticks implementa isso
    public interface ITickSink
    {
        // dt em segundos
        void OnTick(GameModel game, double dtSeconds);
    }

    // Controla o loop de ticks
    public interface ITickService
    {
        void Start();
        Task StopAsync();
        void Subscribe(ITickSink sink);
        void Unsubscribe(ITickSink sink);
        bool IsRunning { get; }               
        event Action? RunningChanged;
    }

    public sealed class TickService : ITickService, IAsyncDisposable
    {
        private readonly ICurrentGameService _game;
        private readonly IUiService _ui;
        private readonly IOfflineService _offline;

        public TickService(ICurrentGameService game, IUiService ui, IOfflineService offline)
        {
            _game = game;
            _ui = ui;
            _offline = offline;
        }

        public bool IsRunning { get; private set; }
        public event Action? RunningChanged;

        private readonly HashSet<ITickSink> _sinks = new();

        private PeriodicTimer? _timer;
        private CancellationTokenSource? _cts;

        private Task? _loopTask;

        // estado do loop
        private DateTime _lastLoopUtc;      // para calcular dt entre iterações
        private double _saveAcc;            // acumula tempo p/ salvar
        private double _uiAcc;              // acumula tempo para atualizar UI
        private const double UiEvery = 0.2; // em segundos (2x/seg undo lag visual)

        // config
        private const int TickMs = 200;                 // 5x por segundo
        private const double MaxDt = 0.25;              // clamp por tick (seg)
        private const double SaveEvery = 60.0;          // salva a cada ~1min
        private const double MaxCatchupSeconds = 60.0;  // 1 min

        public TickService(ICurrentGameService game)
        {
            _game = game;
        }

        public void Subscribe(ITickSink sink) => _sinks.Add(sink);
        public void Unsubscribe(ITickSink sink) => _sinks.Remove(sink);

        public void Start()
        {
            if (IsRunning) return;
            IsRunning = true;
            RunningChanged?.Invoke();
            _cts = new CancellationTokenSource();
            _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(TickMs));
            _lastLoopUtc = DateTime.UtcNow;
            _saveAcc = 0;

            // Dispara o loop sem bloquear quem chamou:
            _loopTask = RunLoopAsync(_cts.Token);
        }

        private async Task RunLoopAsync(CancellationToken ct)
        {
            await CatchUpOfflineAsync();

            try
            {
                //while (await _timer!.WaitForNextTickAsync(ct))
                //{
                //    var dt = ComputeDtSeconds();
                //    if (dt <= 0) continue;
                //    if (dt > MaxDt) dt = MaxDt;

                //    _saveAcc += dt;
                //    _uiAcc += dt;

                //    var doSave = _saveAcc >= SaveEvery;
                //    if (doSave) _saveAcc = 0;

                //    var doUi = _uiAcc >= UiEvery;
                //    if (doUi) _uiAcc = 0;

                //    await ProcessTickAsync(dt, doSave, doUi);
                //}

                while (await _timer!.WaitForNextTickAsync(ct))
                {
                    var rawDt = ComputeDtSeconds();
                    if (rawDt <= 0) continue;

                    var dt = rawDt;

                    // Se o delta real foi muito maior que o que queremos simular por tick,
                    // tratamos o "excedente" como tempo offline.
                    if (dt > MaxDt)
                    {
                        var offlineSeconds = dt - MaxDt;

                        // evita chamar para micro diferenciazinhas
                        if (offlineSeconds > 60.0)
                        {
                            await _offline.OfflineIncome(offlineSeconds);
                        }

                        dt = MaxDt;
                    }

                    _saveAcc += dt;
                    _uiAcc += dt;

                    var doSave = _saveAcc >= SaveEvery;
                    if (doSave) _saveAcc = 0;

                    var doUi = _uiAcc >= UiEvery;
                    if (doUi) _uiAcc = 0;

                    await ProcessTickAsync(dt, doSave, doUi);
                }

            }
            catch (OperationCanceledException) { /* normal */ }
            finally
            {
                _timer?.Dispose();
                _timer = null;
                _cts?.Dispose();
                _cts = null;
            }
        }

        public async Task StopAsync()
        {
            _cts?.Cancel();
            if (_loopTask is not null)
            {
                try { await _loopTask; } catch { /* ignore */ }
                _loopTask = null;
            }
            await _game.Mutate(_ => { }, save: true);
            if (IsRunning)
            {
                IsRunning = false;
                RunningChanged?.Invoke();
            }
            await Task.CompletedTask;
        }

        private double ComputeDtSeconds()
        {
            var now = DateTime.UtcNow;
            var dt = (now - _lastLoopUtc).TotalSeconds;
            _lastLoopUtc = now;
            return dt;
        }

        private async Task CatchUpOfflineAsync()
        {
            var g = _game.CurrentGame;
            if (g is null) return;

            var now = DateTime.UtcNow;
            var last = g.LastTick == default ? now : g.LastTick;

            var elapsedRaw = (now - last).TotalSeconds;
            Console.WriteLine($"[Tick] Catch-up START: elapsed={elapsedRaw:F1}s, last={last:O}");
            if (elapsedRaw <= 0) return;

            var elapsed = Math.Min(elapsedRaw, MaxCatchupSeconds);
            var skipped = elapsedRaw - elapsed;
            if (skipped > 0)
            {
                Console.WriteLine($"[Tick] Offline catch-up: {skipped}s");
                await _offline.OfflineIncome(skipped);
            }

            // processa em blocos de MaxDt e faz 1 save no final
            var remaining = elapsed;
            var first = true;
            while (remaining > 0)
            {
                var step = Math.Min(MaxDt, remaining);
                remaining -= step;

                // no catch-up não salvamos a cada passo
                await ProcessTickAsync(step, save: false, ui: false);

                // ajuda a não travar em devices fracos
                if (!first && remaining > 0)
                    await Task.Yield();
                first = false;
            }

            // salva ao final do catch-up
            await _game.Mutate(_ => { /* LastTick já foi atualizado no ProcessTickAsync */ }, save: true);
            _ui.RaisePulse();
            Console.WriteLine($"[Tick] Offline catch-up: {elapsed:F1}s (total {elapsedRaw:F1}s)");
        }

        private async Task ProcessTickAsync(double dtSeconds, bool save, bool ui)
        {
            if (dtSeconds <= 0) return;

            await _game.Mutate(g =>
            {
                g.LastTick = DateTime.UtcNow;
                foreach (var s in _sinks)
                    s.OnTick(g, dtSeconds);
            }, save: save, ui: ui || save);

            if (ui || save)
            {
                _ui.RaisePulse();
            }
        }

        public ValueTask DisposeAsync()
        {
            _cts?.Cancel();
            _timer?.Dispose();
            _cts = null;
            _timer = null;
            return ValueTask.CompletedTask;
        }
    }
}
