using ApiModels.AppModels;
using Plugin.Geolocator;
using BRBKApp.DA;
using BRBKApp.Services;
using BRBKApp.Services.Interfaces;
using BRBKApp.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BRBKApp
{
    public partial class App : Application
    {
        static Data database;
        public static Data Database
        {
            get
            {
                if (database == null)
                {
                    database = new Data(DependencyService.Get<ILocHelper>().GetLocalFilePath("databaseBRBK.db"));
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();

            if (!Application.Current.Properties.ContainsKey("Timers"))
            {
                App.Current.Properties["Timers"] = 1;
            }

            DependencyService.Register<MockDataStore>();
            var isLoogged = Xamarin.Essentials.SecureStorage.GetAsync("isLogged").Result;
            if (isLoogged == "1")
            {
                
                MainPage = new AppShell();
            }
            else
            {
                MainPage = new LoginPage();
            }
        }

        Geolocator _geolocator;
        private async void Current_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            try
            {
                if (!CrossGeolocator.Current.IsListening)
                {
                    await App.Current.MainPage.DisplayAlert("Advertencia", "No se esta visualizando las coordenadas", "Cerrar");
                    return;
                }
                var position = CrossGeolocator.Current.GetPositionAsync();

                var ids = App.Current.Properties["UserId"];
                int id = Convert.ToInt32(ids);

                _geolocator = new Geolocator()
                {
                    UserId = id,
                    Latitude = position.Result.Latitude,
                    Longitude = position.Result.Longitude,
                    Altitude = position.Result.Altitude
                };
                DatosApi datos = new DatosApi();
            }
            catch
            {
            }

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
