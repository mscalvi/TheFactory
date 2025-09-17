using GuildsLifeCounter;
using GuildsLifeCounter.Models;
using GuildsLifeCounter.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSingleton<CharacterPoolService>();
builder.Services.AddSingleton<MatchService>();

await builder.Build().RunAsync();
