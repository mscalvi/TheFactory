// Services/TickService.cs
using System.Diagnostics;
using System.Threading;

public interface ITickService
{
    void Start(Func<double, Task> onTick, double targetHz = 10); // 10 ticks/s padrão
    void Stop();
}

public sealed class TickService : ITickService, IDisposable
{
    private CancellationTokenSource? _cts;
    private Task? _loop;

    public void Start(Func<double, Task> onTick, double targetHz = 10)
    {
        Stop();
        _cts = new CancellationTokenSource();
        _loop = RunAsync(onTick, targetHz, _cts.Token);
    }

    public void Stop()
    {
        try { _cts?.Cancel(); } catch { }
        _loop = null;
        _cts = null;
    }

    private static async Task RunAsync(Func<double, Task> onTick, double targetHz, CancellationToken ct)
    {
        var interval = TimeSpan.FromSeconds(1.0 / Math.Max(1, targetHz));
        var timer = new PeriodicTimer(interval);

        // usamos relógio real para dt (corrige throttling de abas inativas)
        var last = DateTimeOffset.UtcNow;

        // “teto” de catch-up por tick para não pular demais de uma vez
        const double MAX_DT_PER_TICK = 0.5;    // 500 ms
        const double MAX_CATCHUP_SEC = 60.0;   // limite total de catch-up

        while (await timer.WaitForNextTickAsync(ct).ConfigureAwait(false))
        {
            var now = DateTimeOffset.UtcNow;
            var rawDt = (now - last).TotalSeconds;
            last = now;

            if (rawDt <= 0) continue;

            // se voltamos de muito tempo suspenso, dividimos em steps menores
            var remaining = Math.Min(rawDt, MAX_CATCHUP_SEC);
            while (remaining > 0)
            {
                var step = Math.Min(remaining, MAX_DT_PER_TICK);
                remaining -= step;
                await onTick(step).ConfigureAwait(false);
            }
        }
    }

    public void Dispose() => Stop();
}
