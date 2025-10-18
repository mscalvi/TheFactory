using FurmaIdle;
using FurmaIdle.Services;
using FurmaIdle.Storage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<ICreateGameService, CreateGameService>();
builder.Services.AddSingleton<ICurrentGameService, CurrentGameService>();

builder.Services.AddSingleton<ITickService, TickService>();

builder.Services.AddSingleton<IUiService, UiService>();
builder.Services.AddSingleton<ITooltipService, TooltipService>();
builder.Services.AddScoped<IDebugService, DebugService>();

builder.Services.AddSingleton<IUnlockService, UnlockService>();
builder.Services.AddSingleton<ILocateService, LocateService>();
builder.Services.AddSingleton<IIncomeService, IncomeService>();

builder.Services.AddSingleton<IGameStore, GameStore>();

await builder.Build().RunAsync();
