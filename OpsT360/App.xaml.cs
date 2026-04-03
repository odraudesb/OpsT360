using Microsoft.Extensions.DependencyInjection;
using OpsT360.Views;

namespace OpsT360;

public partial class App : Application
{
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        var introPage = serviceProvider.GetRequiredService<IntroPage>();
        MainPage = new NavigationPage(introPage);
    }
}
