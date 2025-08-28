using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Storage;
using ContaJunstaApp.Services;

namespace ContaJunstaApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .UseMauiCommunityToolkit();

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // Seus serviços
        builder.Services.AddScoped<DataService>();
        builder.Services.AddScoped<CalculationService>();
        builder.Services.AddScoped<ExportService>();

        // FileSaver nativo (Android/iOS) para salvar TXT/CSV
        builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);

        return builder.Build();
    }
}
