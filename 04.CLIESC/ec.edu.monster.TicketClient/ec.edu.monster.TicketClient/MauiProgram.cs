using Microsoft.Extensions.Logging;
// 1. Agregamos el using para que reconozca la carpeta State
using ec.edu.monster.TicketClient.State;

using CompraService;
using PartidoService;
using ReporteService;

namespace ec.edu.monster.TicketClient
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

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // 2. Registramos tu CarritoState como Singleton aquí
            builder.Services.AddSingleton<CarritoState>();

            builder.Services.AddScoped<CompraServiceClient>();
            builder.Services.AddScoped<PartidoServiceClient>();
            builder.Services.AddScoped<ReporteServiceClient>();
            builder.Services.AddSingleton<ec.edu.monster.TicketClient.Servicios.TicketingService>();

            return builder.Build();
        }
    }
}