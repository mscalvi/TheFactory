using System;
using System.Net.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using InsanaRPG;
using InsanaRPG.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");

// HttpClient padrão do WASM (base = /)
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

// DI do seu serviço de criação de personagem
builder.Services.AddScoped<CharacterCreationService>();

await builder.Build().RunAsync();
