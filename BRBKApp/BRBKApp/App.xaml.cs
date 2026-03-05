using BRBKApp.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BRBKApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new OpsT360RootPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
