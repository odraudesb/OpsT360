using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BRBKApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartupPage : ContentPage
    {
        public StartupPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            CheckLogin();
        }

        private async Task CheckLogin()
        {
            try
            {
                // should check for valid login instead
                await Task.Delay(2000);
                var isLoogged = Xamarin.Essentials.SecureStorage.GetAsync("isLogged").Result;
                if (App.Current.Properties.ContainsKey("UserId") == true)
                {
                    var ids = App.Current.Properties["UserId"];
                    if (int.Parse(ids.ToString()) == 0)
                    {
                        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                    }
                    else
                    {
                        await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
            }


        }
    }
}