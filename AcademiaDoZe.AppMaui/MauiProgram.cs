using AcademiaDoZe.AppMaui;
using AcademiaDoZe.AppMaui.Configuration;
using AcademiaDoZe.AppMaui.ViewModels;
using AcademiaDoZe.AppMaui.Views;
using Microsoft.Extensions.Logging;
namespace AcademiaDoZe.AppMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
        .UseMauiApp<App>();
        builder.ConfigureFonts(fonts =>
        {
            fonts.AddFont("CormorantGaramond-SemiBold.ttf", "GothicTitle");
            fonts.AddFont("Inter-Regular.ttf", "GothicBody");
            fonts.AddFont("Inter-SemiBold.ttf", "GothicBodyBold");

            // Adicione esta linha para registrar a fonte
            fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
        });
        // Configurar serviços da aplicação e repositórios
        ConfigurationHelper.ConfigureServices(builder.Services);
        // Registrar ViewModels
        builder.Services.AddTransient<DashboardListViewModel>();
        builder.Services.AddTransient<LogradouroListViewModel>();
        builder.Services.AddTransient<LogradouroViewModel>();
        // Registrar Views
        builder.Services.AddTransient<DashboardListPage>();
        builder.Services.AddTransient<LogradouroListPage>();
        builder.Services.AddTransient<LogradouroPage>();
        builder.Services.AddTransient<ConfigPage>();

        // Registrar ViewModels
        builder.Services.AddTransient<ColaboradorListViewModel>();
        builder.Services.AddTransient<ColaboradorViewModel>();
        // Registrar Views
        builder.Services.AddTransient<ColaboradorListPage>();
        builder.Services.AddTransient<ColaboradorPage>();

        builder.Services.AddTransient<AlunoListViewModel>();
        builder.Services.AddTransient<AlunoViewModel>();
        builder.Services.AddTransient<AlunoListPage>();
        builder.Services.AddTransient<AlunoPage>();
#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}