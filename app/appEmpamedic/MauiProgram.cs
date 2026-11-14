using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Toolkit.Hosting;

namespace appEmpamedic
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureMauiHandlers(handlers =>
                {
#if IOS || MACCATALYST
    				handlers.AddHandler<Microsoft.Maui.Controls.CollectionView, Microsoft.Maui.Controls.Handlers.Items2.CollectionViewHandler2>();
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
                    fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
                });

#if DEBUG
    		builder.Logging.AddDebug();
    		builder.Services.AddLogging(configure => configure.AddDebug());
#endif

            // Servicios: datos simulados o API real según ApiConfig.UsarDatosSimulados.
            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = ApiConfig.UrlBase,
                Timeout = ApiConfig.TiempoEspera
            });
            builder.Services.AddSingleton<SesionService>();
            builder.Services.AddSingleton<MockEmpamedicApi>();
            if (ApiConfig.UsarDatosSimulados)
                builder.Services.AddSingleton<IEmpamedicApi>(sp => sp.GetRequiredService<MockEmpamedicApi>());
            else
                builder.Services.AddSingleton<IEmpamedicApi, HttpEmpamedicApi>();
            builder.Services.AddSingleton<IErrorHandler, ModalErrorHandler>();

            // Páginas principales (Shell las crea y resuelve sus PageModels)
            builder.Services.AddTransient<LoginPageModel>();
            builder.Services.AddSingleton<DashboardPageModel>();
            builder.Services.AddSingleton<PacientesPageModel>();
            builder.Services.AddSingleton<ConsultasPageModel>();
            builder.Services.AddSingleton<InventarioPageModel>();

            // Páginas de detalle con ruta propia
            builder.Services.AddTransientWithShellRoute<PacienteDetallePage, PacienteDetallePageModel>("paciente");

            return builder.Build();
        }
    }
}
