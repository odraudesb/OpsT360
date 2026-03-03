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
    public class VBSOrderDispatchInboxViewModel : BaseViewModel
    {
        bool isRefreshing;
        const int RefreshDuration = 2;
        public bool _esActivo;
        private ImageSource _btnIcon;
        private Combo _selectedNave;
        private Combo _selectedExportador;
        private Combo _selectedBodega;

        public Command ConsultCommand { get; }
        public Command CancelCommand { get; }
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
                    //   llenar combo de exportador
                    ///////////////////////////////////

                    if (SelectedNave is null) { return; }
                    DatosApi datos = new DatosApi();
                    var dd1 = datos.GetListaExportadoresPorNave(SelectedNave.Valor);
                    ListExportador = dd1.Result;
                    OnPropertyChanged(nameof(ListExportador));
                    SelectedExportador = ListExportador?.FirstOrDefault();
                    SelectedExportador = null;

                    ///////////////////////////////////
                    //   llenar combo de bloques
                    ///////////////////////////////////

                    if (_selectedNave is null) { return; }
                    var dd2 = datos.GetListaBloquess(_selectedNave.Valor);
                    ListBodega = dd2.Result;
                    OnPropertyChanged(nameof(ListBodega));
                    SelectedBodega = ListBodega?.FirstOrDefault();
                    SelectedBodega = null;
                }
            }
        }
        public List<Combo> ListNave { get; private set; }

        public Combo SelectedBodega
        {
            get { return _selectedBodega; }
            set { _selectedBodega = value; OnPropertyChanged();}
        }
        public List<Combo> ListBodega { get; private set; }

        public Combo SelectedExportador
        {
            get { return _selectedExportador; }
            set { _selectedExportador = value; OnPropertyChanged(); }
        }
        public List<Combo> ListExportador { get; private set; }

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

        public VBSOrderDispatchInboxViewModel()
        {
            Title = "Order Dispatch Inbox";
            NavTaskCommand = new Command<string>(NavTaskDetail);
            _btnIcon = ImageSource.FromFile("icon_search.png");
            OnPropertyChanged(nameof(BtnIcon));
            ConsultCommand = new Command(OnConsultClicked);
            CancelCommand = new Command(OnCancelClicked);
            esActivo = true;
        }

        private async void OnConsultClicked(object obj)
        {
            esActivo = false;
            IsRefreshing = true;
            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
            await GetAgenda(string.IsNullOrEmpty(SelectedNave?.Valor) ? null : SelectedNave?.Valor,
                            string.IsNullOrEmpty(SelectedExportador?.Valor) ? null : SelectedExportador?.Valor,
                            string.IsNullOrEmpty(SelectedBodega?.Valor) ? null : SelectedBodega?.Valor);
            IsRefreshing = false;
            esActivo = true; ;
        }

        private  void OnCancelClicked()
        {
            //await Shell.Current.GoToAsync("////VBSInboxAisvPage/VBSTaskDetails");
            SelectedNave = null;
            MyAgenda = null;

            ListExportador = null;
            SelectedExportador = null;
            OnPropertyChanged(nameof(ListExportador));

            ListBodega = null;
            SelectedExportador = null;
            OnPropertyChanged(nameof(ListBodega));

            OnPropertyChanged(nameof(MyAgenda));
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
                OnPropertyChanged(nameof(ListNave));
                SelectedNave = ListNave?.FirstOrDefault();

                OnCancelClicked();

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
        private async Task GetAgenda(string _idNave, string _idExportador, string _idBloque)
        {
            if (string.IsNullOrEmpty(_idNave) && string.IsNullOrEmpty(_idBloque) && string.IsNullOrEmpty(_idExportador)) { return; }
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

                    MyAgenda = await datos.GetListaMovimientosParaDespacho(_idNave, _idExportador, _idBloque );
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
            string v_idBooking = v_data[3];

            DatosApi datos = new DatosApi();
            var oEntidad = await datos.GetBAN_Stowage_Plan_AisvXBooking(v_idBooking);

            if (oEntidad.response)
            {
                if (oEntidad.daeAutorizada)
                {
                    await SecureStorage.SetAsync("WorkId", obj);
                    await Shell.Current.GoToAsync("VBSOrderDispatchDetailsPage");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("DAE Alert", "Please verify the DAE Authorization", "OK");
                    return;
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Booking Error", "Please verify Booking", "OK");
                return;
            }
            
        }
    }
}
