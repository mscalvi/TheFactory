using Microsoft.Extensions.Logging;
using ContaJunstaApp.Services;
using CommunityToolkit.Maui;

namespace ContaJunstaApp
{
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
                });
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

            // Se for salvar arquivos nativamente:
            builder.Services.AddSingleton<CommunityToolkit.Maui.Storage.IFileSaver,
                                          CommunityToolkit.Maui.Storage.FileSaver>();

            return builder.Build();
        }
    }
}
