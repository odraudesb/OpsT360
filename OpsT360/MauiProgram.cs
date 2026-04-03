using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpsT360.Services;
using OpsT360.ViewModels;
using OpsT360.Views;
#if ANDROID
using Microsoft.Maui.Handlers;
#endif

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

#if ANDROID
        EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
        {
            handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
            handler.PlatformView.SetSelectAllOnFocus(false);
        });
#endif

        builder.Services.AddSingleton<IAuthState, AuthState>();
        builder.Services.AddSingleton<IAppLanguageState, AppLanguageState>();
        builder.Services.AddTransient<AuthHeaderHandler>();

        builder.Services.AddHttpClient<ILoginService, LoginService>();

        builder.Services.AddSingleton<RoboflowValidationService>();

        builder.Services.AddHttpClient<ITransactionsService, TransactionsService>()
            .AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddSingleton<IRfidScannerService, RfidScannerService>();

        // Pages y ViewModels visuales: TRANSIENT
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<IntroPage>();

        builder.Services.AddTransient<SealInspectionViewModel>();
        builder.Services.AddTransient<SealInspectionPage>();

        builder.Services.AddTransient<TransactionsPage>();
        builder.Services.AddTransient<MainMenuPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
