using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using Microsoft.Maui;
using OpsT360.Services;

namespace OpsT360;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize
                         | ConfigChanges.Orientation
                         | ConfigChanges.UiMode
                         | ConfigChanges.ScreenLayout
                         | ConfigChanges.SmallestScreenSize
                         | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    private const string LogTag = "OpsT360.RFID";

    protected override void OnStart()
    {
        base.OnStart();

        try
        {
            var msg = RfidScannerService.PlatformEnsureReaderOpened();
            Log.Debug(LogTag, $"[MainActivity.OnStart] {msg}");
        }
        catch (System.Exception ex)
        {
            Log.Error(LogTag, $"[MainActivity.OnStart] Error abriendo RFID: {ex}");
        }
    }

    protected override void OnStop()
    {
        try
        {
            RfidScannerService.PlatformCloseReader();
            Log.Debug(LogTag, "[MainActivity.OnStop] RFID cerrado");
        }
        catch (System.Exception ex)
        {
            Log.Error(LogTag, $"[MainActivity.OnStop] Error cerrando RFID: {ex}");
        }

        base.OnStop();
    }

    protected override void OnDestroy()
    {
        try
        {
            RfidScannerService.PlatformCloseReader();
            Log.Debug(LogTag, "[MainActivity.OnDestroy] RFID cerrado");
        }
        catch (System.Exception ex)
        {
            Log.Error(LogTag, $"[MainActivity.OnDestroy] Error cerrando RFID: {ex}");
        }

        base.OnDestroy();
    }
}