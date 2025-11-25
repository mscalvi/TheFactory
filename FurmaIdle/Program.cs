using FurmaIdle;
using FurmaIdle.Services;
using FurmaIdle.Storage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.ComponentModel.Design;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<ICreateGameService, CreateGameService>();
builder.Services.AddSingleton<ICurrentGameService, CurrentGameService>();
builder.Services.AddSingleton<IGameMigrationService, GameMigrationService>();

builder.Services.AddSingleton<ITickService, TickService>();
builder.Services.AddSingleton<IContractsService, ContractsService>();
builder.Services.AddSingleton<ContractsTickSink>();
builder.Services.AddSingleton<IResourcesService, ResourcesService>();
builder.Services.AddSingleton<ResourcesTickSink>();
builder.Services.AddSingleton<ISpecialtiesService, SpecialtiesService>();
builder.Services.AddSingleton<SpecialtiesTickSink>();

builder.Services.AddSingleton<IUiService, UiService>();
builder.Services.AddSingleton<ITooltipService, TooltipService>();
builder.Services.AddSingleton<IUiLogService, UiLogService>();

builder.Services.AddSingleton<IUnlockService, UnlockService>();
builder.Services.AddSingleton<ILocateService, LocateService>();
builder.Services.AddSingleton<IIncomeService, IncomeService>();
builder.Services.AddSingleton<IPurchaseService, PurchaseService>();
builder.Services.AddSingleton<IExpeditionService, ExpeditionService>();
builder.Services.AddSingleton<IEffectService, EffectService>();
builder.Services.AddSingleton<IClickService, ClickService>();
builder.Services.AddSingleton<IKnowledgeService, KnowledgeService>();
builder.Services.AddSingleton<ICostService, CostService>();
builder.Services.AddSingleton<IModifierService, ModifierService>();
builder.Services.AddSingleton<ILoreService, LoreService>();
builder.Services.AddSingleton<IOfflineService, OfflineService>();

builder.Services.AddSingleton<IGameStore, GameStore>();

var host = builder.Build();

_ = host.Services.GetRequiredService<ContractsTickSink>();
_ = host.Services.GetRequiredService<ResourcesTickSink>();
_ = host.Services.GetRequiredService<SpecialtiesTickSink>();

await host.RunAsync();
