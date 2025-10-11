using FurmaIdle;
using FurmaIdle.Services;
using FurmaIdle.Storage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using static FurmaIdle.Storage.GameStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<IStartService, StartService>();
builder.Services.AddSingleton<IGameService, GameService>(); 
builder.Services.AddSingleton<ITooltipService, TooltipService>();
builder.Services.AddSingleton<IUpgradeService, UpgradeService>();
builder.Services.AddSingleton<IStageService, StageService>();
builder.Services.AddSingleton<ITickService, TickService>();
builder.Services.AddSingleton<IUnlockService, UnlockService>();

builder.Services.AddSingleton<IGameStore, IndexedDbGameStore>();

await builder.Build().RunAsync();
