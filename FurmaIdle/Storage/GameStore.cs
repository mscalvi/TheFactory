using FurmaIdle.Models;
using Microsoft.JSInterop;
using System.Text.Json;
using System.Threading.Tasks;

namespace FurmaIdle.Storage
{
    public interface IGameStore
    {
        Task<GameModel?> LoadAsync(string key = "main");
        Task SaveAsync(GameModel model, string key = "main");
        Task ClearAsync(string key = "main");
    }

    public sealed class GameStore : IGameStore, IAsyncDisposable
    {
        private readonly IJSRuntime _js;
        private IJSObjectReference? _module;

        // Opções de JSON (camelCase, sem indentar)
        private static readonly JsonSerializerOptions _json = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public GameStore(IJSRuntime js) => _js = js;

        // Importa o módulo JS que fala com IndexedDB/localStorage
        private async ValueTask<IJSObjectReference> Module()
        {
            _module ??= await _js.InvokeAsync<IJSObjectReference>(
                "import", "./js/idbStore.js");
            return _module;
        }

        public async Task<GameModel?> LoadAsync(string key = "main")
        {
            IJSObjectReference m;
            try
            {
                m = await Module();
            }
            catch
            {
                // Se ainda não for possível importar (ex.: antes do 1º render),
                // trate como "sem dado" para não quebrar o bootstrap.
                return null;
            }

            string? raw;
            try
            {
                raw = await m.InvokeAsync<string?>("load", key);
            }
            catch
            {
                // Falha no JS/IDB → trate como ausente
                return null;
            }

            // Nada salvo / chave ausente / valores "falsos"
            if (string.IsNullOrWhiteSpace(raw)) return null;
            raw = raw.Trim();
            if (raw == "null" || raw == "undefined") return null;

            try
            {
                var model = JsonSerializer.Deserialize<GameModel>(raw, _json);
                if (model is null) return null;

                // [Sanity-check mínimo] trate "{}" ou objetos quebrados como "sem save"
                if (model.SchemaVersion <= 0) return null;

                return model;
            }
            catch
            {
                // JSON inválido → trate como "sem save"
                return null;
            }
        }

        public async Task SaveAsync(GameModel model, string key = "main")
        {
            var json = JsonSerializer.Serialize(model, _json);
            var m = await Module();
            await m.InvokeAsync<object>("save", key, json);
        }

        public async Task ClearAsync(string key = "main")
        {
            var m = await Module();
            await m.InvokeAsync<object>("remove", key);
        }

        public async ValueTask DisposeAsync()
        {
            if (_module is not null)
                await _module.DisposeAsync();
        }
    }
}
