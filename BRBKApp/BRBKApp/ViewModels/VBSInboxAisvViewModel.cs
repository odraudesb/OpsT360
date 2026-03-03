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
    public class VBSInboxAisvViewModel : BaseViewModel
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

        public VBSInboxAisvViewModel()
        {
            Title = "Inbox AISV";
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
        private async Task GetAgenda(string _aisv)
        {
            if (string.IsNullOrEmpty(_aisv)){ _aisv = null; }
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

                    MyAgenda = await datos.GetListaAISV("ING", _aisv, null, id);
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
            var v_data = obj.Split(',');

            string v_id = v_data[0];
            bool v_estado = bool.Parse(v_data[1]);
            string v_comentario = v_data[2];


            if (v_estado)
            {
                if (v_comentario != "0")
                {
                    bool DeACuerdo;
                    DeACuerdo = await App.Current.MainPage.DisplayAlert("Confirmar", string.Format("La carga tiene la siguiente alerta: \n{0}. \n¿Desea Continuar?", v_comentario), "De Acuerdo", "Cancelar");
                    if (DeACuerdo)
                    {
                        await SecureStorage.SetAsync("WorkId", v_id);
                        await Shell.Current.GoToAsync("VBSTaskDetails");
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    await SecureStorage.SetAsync("WorkId", v_id);
                    await Shell.Current.GoToAsync("VBSTaskDetails");
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Verificación de Estado ", "Por favor verifique el estado del IIE", "OK");
                return;
            }
            
        }
    }
}
