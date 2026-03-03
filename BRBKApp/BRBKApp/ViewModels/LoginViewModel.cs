using ApiModels.AppModels;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using BRBKApp.DA;
using BRBKApp.Services;
using BRBKApp.Services.Interfaces;
using BRBKApp.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Acr.UserDialogs;
using Xamarin.Essentials;
using System.Threading.Tasks;
using BRBKApp.Services;

namespace BRBKApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; }
        private string username = null;
        private string password = null;
        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        public LoginViewModel()
        {
            LoginCommand = new Command(async (o) => await OnLoginClicked(o));
        }

        private async Task OnLoginClicked(object obj)
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync<PhonePermission>();
                string imeis = "";

                imeis = DependencyService.Get<IServiceImei>().GetImei();

                UserDialogs.Instance.ShowLoading("Sending...");
                var current = Connectivity.NetworkAccess;
                if (current == NetworkAccess.None)
                {
                    await App.Current.MainPage.DisplayAlert("Internet Error", "Please verify your internet connection", "OK");
                    return;
                }

                if (current == NetworkAccess.Internet)
                {
                    string IpAddress = string.Empty;
                    try
                    {
                        IpAddress = DependencyService.Get<INetServices>().ConvertHostIP();
                    }
                    catch { IpAddress = "172.0.0.1"; }
                    

                    DatosApi datos = new DatosApi();
                    User _user = await datos.Login(username.Trim(), password, imeis);
                    int grabado;

                    if (_user.response == true)
                    {
                        List<UserDB> users = new List<UserDB>();
                        users = await App.Database.GetRegistradoById(_user.UserId);

                        if (users.Count > 0)
                        {
                            grabado = await App.Database.UpdateDatos(_user.UsuarioDB);
                            if (grabado == 1)
                            {
                                App.Current.Properties["IsLoggedIn"] = true;
                                App.Current.Properties["UserId"] = _user.UserId;
                                App.Current.Properties["Username"] = _user.Username;
                                App.Current.Properties["RoleId"] = _user.RoleId;
                                App.Current.Properties["SuperUser"] = _user.Role?.SuperUser;
                                App.Current.Properties["Timers"] = _user.Timer;
                                App.Current.Properties["UserIp"] = IpAddress;
                                await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "1");
                                Application.Current.MainPage = new AppShell();
                            }
                            else
                            {
                                await App.Current.MainPage.DisplayAlert("Error", "Please try again", "Close");
                            }
                        }
                        else
                        {
                            grabado = await App.Database.RegistroDatos(_user.UsuarioDB);

                            if (grabado == 1)
                            {
                                App.Current.Properties["IsLoggedIn"] = true;
                                App.Current.Properties["UserId"] = _user.UserId;
                                App.Current.Properties["Username"] = _user.Username;
                                App.Current.Properties["RoleId"] = _user.RoleId;
                                App.Current.Properties["SuperUser"] = _user.Role?.SuperUser;
                                App.Current.Properties["Timers"] = _user.Timer;
                                App.Current.Properties["UserIp"] = IpAddress;
                                await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "1");
                                Application.Current.MainPage = new AppShell();
                            }
                            else
                            {
                                await App.Current.MainPage.DisplayAlert("Error", "Please try again", "Close");
                            }
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Error", _user.messages + " imeis:"+ imeis, "Cerrar");
                    }
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Cerrar");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

    }
}
