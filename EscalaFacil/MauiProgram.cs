using EscalaFacil.Components.Services;
using Microsoft.Maui.Hosting;
using Microsoft.Extensions.Logging;

namespace EscalaFacil;

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
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // DI
        builder.Services.AddSingleton<IDataStore>(sp => new JsonDataStore());
        builder.Services.AddSingleton<ISchedulerService, SchedulerService>();
        builder.Services.AddSingleton<IScheduleLocator, ScheduleLocator>();


        return builder.Build();
    }
}
