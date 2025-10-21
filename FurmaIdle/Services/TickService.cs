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
        public bool IsRunning { get; private set; }
        public event Action? RunningChanged;

        private readonly ICurrentGameService _game;

        private readonly HashSet<ITickSink> _sinks = new();

        private PeriodicTimer? _timer;
        private CancellationTokenSource? _cts;

        private Task? _loopTask;

        // estado do loop
        private DateTime _lastLoopUtc; // para calcular dt entre iterações
        private double _saveAcc;       // acumula tempo p/ salvar

        // config
        private const int TickMs = 200;        // 5x por segundo
        private const double MaxDt = 0.25;     // clamp por tick (seg)
        private const double SaveEvery = 2.0;  // salva a cada ~2s

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
                while (await _timer!.WaitForNextTickAsync(ct))
                {
                    var dt = ComputeDtSeconds();
                    if (dt <= 0) continue;
                    if (dt > MaxDt) dt = MaxDt;

                    _saveAcc += dt;
                    var doSave = _saveAcc >= SaveEvery;
                    if (doSave) _saveAcc = 0;

                    await ProcessTickAsync(dt, doSave);
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
            // calcula tempo desde o último tick salvo e “passa o filme” em passos
            var g = _game.CurrentGame;
            if (g is null) return;

            var last = g.LastTick == default ? DateTime.UtcNow : g.LastTick;
            var elapsed = (DateTime.UtcNow - last).TotalSeconds;
            Console.WriteLine($"[Tick] Catch-up START: elapsed={elapsed:F1}s, last={last:O}");
            if (elapsed <= 0) return;

            // processa em blocos de MaxDt e faz 1 save no final
            var remaining = elapsed;
            var first = true;
            while (remaining > 0)
            {
                var step = Math.Min(MaxDt, remaining);
                remaining -= step;

                // no catch-up não precisamos salvar a cada passo
                await ProcessTickAsync(step, save: false);

                // evita “pico” de CPU em devices fracos (opcional)
                if (!first && remaining > 0)
                    await Task.Yield();
                first = false;
            }

            // salva ao final do catch-up
            await _game.Mutate(_ => { /* já ajustamos LastTick no ProcessTickAsync */ }, save: true);
            Console.WriteLine($"[Tick] Offline catch-up: {elapsed}s");
        }

        private async Task ProcessTickAsync(double dtSeconds, bool save)
        {
            if (dtSeconds <= 0) return;

            await _game.Mutate(g =>
            {
                g.LastTick = DateTime.UtcNow;
                foreach (var s in _sinks)
                    s.OnTick(g, dtSeconds);
            }, save: save);
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
