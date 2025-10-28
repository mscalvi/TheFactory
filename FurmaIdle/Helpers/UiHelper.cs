using Microsoft.AspNetCore.Components;
using FurmaIdle.Services;

public abstract class UiHelper : ComponentBase, IDisposable
{
    [Inject] protected IUiService UI { get; set; } = default!;
    [Inject] protected ICurrentGameService Game { get; set; } = default!;

    protected override void OnInitialized()
    {
        UI.Pulse += OnUiPulseInternal;
        Game.GameChanged += OnGameChangedInternal;
        Game.ReadyChanged += OnGameChangedInternal;
    }

    private void OnUiPulseInternal()
    {
        try { OnUiPulse(); }
        finally { _ = InvokeAsync(StateHasChanged); }
    }

    private void OnGameChangedInternal()
    {
        try { OnGameChanged(); }
        finally { _ = InvokeAsync(StateHasChanged); }
    }

    // Hooks opcionais para derivados
    protected virtual void OnUiPulse() { }
    protected virtual void OnGameChanged() { }

    public void Dispose()
    {
        UI.Pulse -= OnUiPulseInternal;
        Game.GameChanged -= OnGameChangedInternal;
        Game.ReadyChanged -= OnGameChangedInternal;
    }
}
