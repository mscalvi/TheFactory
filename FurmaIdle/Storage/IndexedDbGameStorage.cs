using FurmaIdle.Models;
using Microsoft.JSInterop;
using System.Text.Json;
using System.Threading.Tasks;
using static FurmaIdle.Storage.GameStorage;

namespace FurmaIdle.Storage
{
    public sealed class IndexedDbGameStore : IGameStore, IAsyncDisposable
    {
        private readonly IJSRuntime _js;
        private IJSObjectReference? _module;

        // Opcional: ajuste opções de JSON se quiser
        private static readonly JsonSerializerOptions _json = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public IndexedDbGameStore(IJSRuntime js) { _js = js; }

        private async ValueTask<IJSObjectReference> Module()
        {
            _module ??= await _js.InvokeAsync<IJSObjectReference>(
                "import", "./js/idbStore.js");
            return _module;
        }

        public async Task<GameModel?> LoadAsync(string key = "main")
        {
            var m = await Module();
            var raw = await m.InvokeAsync<string?>("load", key);
            if (string.IsNullOrEmpty(raw)) return null;

            try { return JsonSerializer.Deserialize<GameModel>(raw, _json); }
            catch { return null; }
        }

        public async Task SaveAsync(GameModel model, string key = "main")
        {
            var json = JsonSerializer.Serialize(model, _json);
            var m = await Module();
            await m.InvokeAsync<bool>("save", key, json);
        }

        public async ValueTask DisposeAsync()
        {
            if (_module is not null)
                await _module.DisposeAsync();
        }
    }
}
