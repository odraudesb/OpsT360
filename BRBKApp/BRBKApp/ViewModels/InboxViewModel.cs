using Acr.UserDialogs;
using ApiModels.AppModels;
using BRBKApp.DA;
using BRBKApp.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BRBKApp.ViewModels
{
    public class InboxViewModel : BaseViewModel
    {
        bool isRefreshing;
        const int RefreshDuration = 2;
        public bool _esActivo;
        private ImageSource _btnIcon;
        private string txtMrn = null;
        public Command ConsultCommand { get; }
        public Command NavTaskCommand { get; set; }
        public ObservableCollection<Tasks> MyAgenda { get; private set; }
        public ImageSource BtnIcon
        {
            get { return _btnIcon; }
            set
            {
                _btnIcon = value;
                SetProperty(ref _btnIcon, value);
            }
        }
        public string TxtMRN
        {
            get => txtMrn;
            set
            {
                txtMrn = value;
                OnPropertyChanged();
            }
        }
        public bool esActivo
        {
            get => _esActivo;
            set
            {
                _esActivo = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand => new Command(async () => await RefreshItemsAsync());

        public InboxViewModel()
        {
            Title = "Inbox BL";
            NavTaskCommand = new Command<string>(NavTaskDetail);
            _btnIcon = ImageSource.FromFile("icon_search.png");
            OnPropertyChanged(nameof(BtnIcon));
            ConsultCommand = new Command(OnConsultClicked);
            esActivo = true;
        }

        private async void OnConsultClicked(object obj)
        {
            esActivo = false;
            IsRefreshing = true;
            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
            await GetAgenda(TxtMRN);
            IsRefreshing = false;
            esActivo = true; ;
        }
        async Task RefreshItemsAsync()
        {
            if (esActivo)
            {
                IsRefreshing = true;
                await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
                await GetAgenda(TxtMRN);
                IsRefreshing = false;
            }
        }
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }
        private async Task GetAgenda(string _mrn)
        {
            if (string.IsNullOrEmpty(_mrn)){ return; }
            try
            {
                UserDialogs.Instance.ShowLoading("Loading...");
                var current = Connectivity.NetworkAccess;
                if (current == NetworkAccess.None)
                {
                    await App.Current.MainPage.DisplayAlert("Internet Error", "Please verify your internet connection", "OK");
                    return;
                }
                if (current == NetworkAccess.Internet)
                {
                    DatosApi datos = new DatosApi();
                    var ids = App.Current.Properties["UserId"];
                    int id = Convert.ToInt32(ids);

                    var _sUser = App.Current.Properties["SuperUser"];
                    bool superUser = bool.Parse(_sUser.ToString());
                    string _lugar = string.Empty;

                    if (superUser)
                    {
                        _lugar = "BODEGA";
                    }
                    else
                    {
                        _lugar = "MUELLE";
                    }

                    MyAgenda = await datos.GetListaBL(_mrn,_lugar, id);
                    OnPropertyChanged(nameof(MyAgenda));
                }

                
            }
            catch
            {
                App.Current.Properties["IsLoggedIn"] = false;
                App.Current.Properties["UserId"] = null;
                App.Current.Properties["Timers"] = 1;
                await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "0");
                await Shell.Current.GoToAsync("//LoginPage");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }
        async private void NavTaskDetail(string obj)
        {
            await SecureStorage.SetAsync("WorkId", obj);
            await Shell.Current.GoToAsync("TaskDetails");
        }
    }
}
