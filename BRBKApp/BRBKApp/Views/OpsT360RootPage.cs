using Xamarin.Forms;

namespace BRBKApp.Views
{
    public class OpsT360RootPage : TabbedPage
    {
        public OpsT360RootPage()
        {
            Title = "OpsT360";
            Children.Add(new NavigationPage(new OpsAuthPage()) { Title = "Acceso" });
            Children.Add(new NavigationPage(new OpsSealOperationsPage()) { Title = "Sellos" });
        }
    }
}
