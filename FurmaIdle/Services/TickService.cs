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
        Task StartAsync();
        Task StopAsync();
        void Subscribe(ITickSink sink);
        void Unsubscribe(ITickSink sink);
    }

    public sealed class TickService : ITickService, IAsyncDisposable
    {
        private readonly ILogger<TickService> _log;
        private readonly ICurrentGameService _game;

        private readonly HashSet<ITickSink> _sinks = new();

        private PeriodicTimer? _timer;
        private CancellationTokenSource? _cts;

        // estado do loop
        private bool _running;
        private DateTime _lastLoopUtc; // para calcular dt entre iterações
        private double _saveAcc;       // acumula tempo p/ salvar

        // config
        private const int TickMs = 200;        // 5x por segundo
        private const double MaxDt = 0.25;     // clamp por tick (seg)
        private const double SaveEvery = 2.0;  // salva a cada ~2s

        public TickService(ILogger<TickService> log, ICurrentGameService game)
        {
            _log = log;
            _game = game;
        }

        public void Subscribe(ITickSink sink) => _sinks.Add(sink);
        public void Unsubscribe(ITickSink sink) => _sinks.Remove(sink);

        public async Task StartAsync()
        {
            if (_running) return;
            _running = true;

            _cts = new CancellationTokenSource();
            _timer = new PeriodicTimer(TimeSpan.FromMilliseconds(TickMs));
            _lastLoopUtc = DateTime.UtcNow;
            _saveAcc = 0;

            // catch-up offline antes do loop
            await CatchUpOfflineAsync();

            try
            {
                // loop assíncrono (sem bloqueios — compatível com WASM)
                while (await _timer.WaitForNextTickAsync(_cts.Token))
                {
                    var dt = ComputeDtSeconds();
                    if (dt <= 0) continue;

                    // clamp p/ estabilidade
                    if (dt > MaxDt) dt = MaxDt;

                    _saveAcc += dt;
                    var doSave = _saveAcc >= SaveEvery;
                    if (doSave) _saveAcc = 0;

                    await ProcessTickAsync(dt, doSave);
                }
            }
            catch (OperationCanceledException)
            {
                // normal no StopAsync
            }
            finally
            {
                _timer?.Dispose();
                _timer = null;
                _cts?.Dispose();
                _cts = null;
                _running = false;
            }
        }

        public async Task StopAsync()
        {
            _cts?.Cancel();
            // grava o estado atual (opcional, força persistir)
            await _game.Mutate(g => { /* noop */ }, save: true);
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
            _log.LogInformation("[Tick] Offline catch-up: {Seconds:F2}s", elapsed);
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
