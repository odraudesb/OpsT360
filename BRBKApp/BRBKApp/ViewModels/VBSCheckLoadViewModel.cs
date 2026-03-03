using Acr.UserDialogs;
using ApiModels.AppModels;
using BRBKApp.DA;
using BRBKApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace BRBKApp.ViewModels
{
    public class VBSCheckLoadViewModel : BaseViewModel, INotifyPropertyChanged
    {
        bool isRefreshing;
        const int RefreshDuration = 2;
        public bool _esActivo;
        private ImageSource _btnIcon;
        private string _txtBooking = null;
        private string _txtBarcode = null;
        private Combo _selectedNave;
        private Combo _selectedBodega;
        private Combo _selectedExportador;

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
       
        public Combo SelectedNave
        {
            get { return _selectedNave; }
            set 
            { 
                //_selectedNave = value; OnPropertyChanged();

                if (_selectedNave != value)
                {
                    _selectedNave = value;
                    OnPropertyChanged();

                    ///////////////////////////////////
                    //   llenar combo de bodegas
                    ///////////////////////////////////
                    
                    if (_selectedNave is null) { return; }
                    DatosApi datos = new DatosApi();
                    var dd1 = datos.GetListaBodegas(_selectedNave.Valor);
                    ListBodega = dd1.Result;
                    SelectedBodega = ListBodega?.FirstOrDefault();
                    OnPropertyChanged(nameof(ListBodega));
                }
            }
        }
        public List<Combo> ListNave { get; private set; }

        public Combo SelectedBodega
        {
            get { return _selectedBodega; }
            set { 
                //_selectedBodega = value; OnPropertyChanged();

                if (_selectedBodega != value)
                {
                    _selectedBodega = value;
                    OnPropertyChanged();

                    ///////////////////////////////////
                    //   llenar combo de exportador
                    ///////////////////////////////////

                    if (_selectedBodega is null || SelectedNave is null) { return; }
                    DatosApi datos = new DatosApi();
                    int? vBodega = int.Parse(_selectedBodega?.Valor);
                    var dd1 = datos.GetListaExportadoresPorNaveBodega(SelectedNave.Valor, vBodega);
                    ListExportador = dd1.Result;
                    SelectedExportador = ListExportador?.FirstOrDefault();
                    OnPropertyChanged(nameof(ListExportador));
                }
            }
        }
        public List<Combo> ListBodega { get; private set; }

        public Combo SelectedExportador
        {
            get { return _selectedExportador; }
            set { _selectedExportador = value; OnPropertyChanged(); }
        }
        public List<Combo> ListExportador { get; private set; }

        public string txtBooking
        {
            get => _txtBooking;
            set
            {
                _txtBooking = value;
                OnPropertyChanged();
            }
        }

        public string txtBarcode
        {
            get => _txtBarcode;
            set
            {
                _txtBarcode = value;
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

        public VBSCheckLoadViewModel()
        {
            Title = "Check Load";
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
            await GetAgenda(string.IsNullOrEmpty(SelectedNave?.Valor) ? null : SelectedNave?.Valor,
                            string.IsNullOrEmpty(SelectedBodega?.Valor) ? null : SelectedBodega?.Valor,
                            string.IsNullOrEmpty(SelectedExportador?.Valor) ? null : SelectedExportador?.Valor,
                            string.IsNullOrEmpty(txtBooking)? null: txtBooking, 
                            string.IsNullOrEmpty(txtBarcode) ? null : txtBarcode);
            IsRefreshing = false;
            esActivo = true; ;
        }
        async Task RefreshItemsAsync()
        {
            if (esActivo)
            {
                IsRefreshing = true;
                //await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
                //await GetAgenda(SelectedNave.Valor, SelectedBodega.Valor, SelectedExportador.Valor, txtBarcode, txtBarcode);

                ///////////////////////////////////
                //   llenar combo de naves
                ///////////////////////////////////
                DatosApi datos = new DatosApi();
                var dd = await datos.GetListaNave();
                ListNave = dd;
                SelectedNave = ListNave?.FirstOrDefault();
                OnPropertyChanged(nameof(ListNave));

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
        private async Task GetAgenda(string _idNave, string _idBodega, string _idExportador, string _booking, string _barcode)
        {
            if (string.IsNullOrEmpty(_idNave) && string.IsNullOrEmpty(_idBodega) && string.IsNullOrEmpty(_idExportador) && string.IsNullOrEmpty(_booking) && string.IsNullOrEmpty(_barcode)) { return; }
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

                    MyAgenda = await datos.GetListaMovimientosPorFiltros(_idNave, _idBodega, _idExportador, _booking, _barcode);
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

        private async void Picker_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = sender as Picker;
            if (picker.SelectedIndex == -1)
            {
                // No selection has been made
            }
            else
            {
                // Handle the selection change
                // You can access the selected item using picker.SelectedItem
                var selectedItem = picker.SelectedItem; // This will get you the selected item from the picker
            }

            ///////////////////////////////////
            //   llenar combo de ubicaciones
            ///////////////////////////////////
            DatosApi datos = new DatosApi();
            var dd = await datos.GetListaNave();
            ListNave = dd;
            SelectedNave = ListNave?.FirstOrDefault();
            OnPropertyChanged(nameof(ListNave));
        }
    }
}
