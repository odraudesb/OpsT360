using Microsoft.Extensions.Logging;
using OpsT360.Services;
using OpsT360.ViewModels;
using OpsT360.Views;

namespace OpsT360;

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

        builder.Services.AddSingleton<IAuthState, AuthState>();
        builder.Services.AddTransient<AuthHeaderHandler>();
        builder.Services.AddHttpClient<ILoginService, LoginService>();
        builder.Services.AddHttpClient<ITransactionsService, TransactionsService>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddSingleton<LoginViewModel>();
        builder.Services.AddSingleton<SealInspectionViewModel>();
        builder.Services.AddSingleton<LoginPage>();
        builder.Services.AddSingleton<SealInspectionPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
