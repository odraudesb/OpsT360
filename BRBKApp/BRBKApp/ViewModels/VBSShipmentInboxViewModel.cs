using Acr.UserDialogs;
using ApiModels.AppModels;
using BRBKApp.DA;
using BRBKApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BRBKApp.ViewModels
{
    public class VBSShipmentInboxViewModel : BaseViewModel
    {
        bool isRefreshing;
        const int RefreshDuration = 2;
        public bool _esActivo;
        public string iduser;
        public string idWork;
        private ImageSource _btnIcon;
        private ImageSource _btnIcon1;
        private Combo _selectedNave;
        private Combo _selectedExportador;
        public ICommand RefreshCommand => new Command(async () => await RefreshItemsAsync());

        #region "Propiedades"
        public Command AddCommand { get; }
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
        public ImageSource BtnIcon1
        {
            get { return _btnIcon1; }
            set
            {
                _btnIcon1 = value;
                SetProperty(ref _btnIcon1, value);
            }
        }
        public List<Combo> ListNave { get; private set; }
        public List<Combo> ListExportador { get; private set; }
        public Combo SelectedNave
        {
            get { return _selectedNave; }
            set
            {
                //_selectedBodega = value; OnPropertyChanged();

                if (_selectedNave != value)
                {
                    _selectedNave = value;
                    OnPropertyChanged();
                }
            }
        }
        public Combo SelectedExportador
        {
            get { return _selectedExportador; }
            set
            {
                //_selectedBodega = value; OnPropertyChanged();

                if (_selectedExportador != value)
                {
                    _selectedExportador = value;
                    OnPropertyChanged();
                }
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
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }
        #endregion
        
        #region "Load"
        public VBSShipmentInboxViewModel()
        {
            Title = "Inbox Shiping";
            NavTaskCommand = new Command<string>(NavTaskDetail);
            _btnIcon = ImageSource.FromFile("icon_search.png");
            _btnIcon1 = ImageSource.FromFile("icon_add.png");
            OnPropertyChanged(nameof(BtnIcon));

            var ids = App.Current.Properties["UserId"];
            iduser = ids.ToString();
            AddCommand = new Command<string>(OnAddTap);
            ConsultCommand = new Command(OnConsultClicked);
            esActivo = true;
        }
        #endregion

        #region "Metodos"
        private async Task GetAgenda(string _idNave, string _idExportador)
        {
            if (string.IsNullOrEmpty(_idNave)) { return; }
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
                    MyAgenda = await datos.GetListaOrdenesEmbarque(_idNave, _idExportador, id);
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
        #endregion

        #region "Eventos Controles"
        private async void OnConsultClicked(object obj)
        {
            esActivo = false;
            IsRefreshing = true;
            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
            string vCodExpo = SelectedExportador?.Valor;
            if (SelectedExportador?.Valor == "0")
            {
                vCodExpo = null;
            }
            await GetAgenda(SelectedNave?.Valor, vCodExpo);
            IsRefreshing = false;
            esActivo = true; ;
        }
        async Task RefreshItemsAsync()
        {
            if (esActivo)
            {
                IsRefreshing = true;
                await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));

                ///////////////////////////////////
                //   llenar combo de naves y export
                ///////////////////////////////////
                DatosApi datos = new DatosApi();
                var dd = await datos.GetListaNavesVBS();
                ListNave = dd;

                var lstExpo = await datos.GetListaExportadoresVBS();
                ListExportador = lstExpo;

                OnPropertyChanged(nameof(ListNave));
                OnPropertyChanged(nameof(ListExportador));

                SelectedNave = ListNave?.FirstOrDefault();
                SelectedExportador = ListExportador?.FirstOrDefault();
                string vCodExpo = SelectedExportador?.Valor;
                if (SelectedExportador?.Valor == "0")
                {
                    vCodExpo = null;
                }

                await GetAgenda(SelectedNave?.Valor, vCodExpo);
                IsRefreshing = false;
            }
        }
        async private void OnAddTap(string obj)
        {
            try
            {
                string vCodExpo = SelectedExportador?.Valor;
                if (SelectedExportador?.Valor == "0")
                {
                    vCodExpo = null;
                }
                idWork = string.Format("{0}-{1}", SelectedNave?.Valor, vCodExpo);
                await SecureStorage.SetAsync("WorkId", idWork);
                await Shell.Current.GoToAsync("VBSShipmentInboxNewPage");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Cerrar");
            }
        }
        async private void NavTaskDetail(string obj)
        {
            await SecureStorage.SetAsync("WorkId", obj);
            await Shell.Current.GoToAsync("VBSShipmentDetailsPage");
        }
        #endregion
    }
}
