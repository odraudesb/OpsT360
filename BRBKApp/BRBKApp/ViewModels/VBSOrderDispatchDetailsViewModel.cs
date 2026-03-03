using Acr.UserDialogs;
using ApiModels.AppModels;
using BRBKApp.DA;
using BRBKApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
            
namespace BRBKApp.ViewModels
{
    public class VBSOrderDispatchDetailsViewModel : BaseViewModel
    {
        private ImageSource _btnIcon;
        public bool _esVisible;
        public string idWork;
        public string comentary;
        private string taskslabel;
        private string container;
        private string temperature;
        private string types;
        private BAN_Stowage_Movimiento _selectedItem;
        public string iduser;
        private string itemId;
        bool isRefreshing;
        const int RefreshDuration = 2;
        public int pendiente;

        public ImageSource BtnIcon
        {
            get { return _btnIcon; }
            set
            {
                _btnIcon = value;
                SetProperty(ref _btnIcon, value);
            }
        }
        public string Comentary
        {
            get => comentary;
            set => SetProperty(ref comentary, value);
        }
        public string TasksLabel{
            get => taskslabel;
            set => SetProperty(ref taskslabel, value);
        }
        public string Container
        {
            get => container;
            set => SetProperty(ref container, value);
        }
        public string Temperature
        {
            get => temperature;
            set => SetProperty(ref temperature, value);
        }
        public string Types
        {
            get => types;
            set => SetProperty(ref types, value);
        }
        
        public List<TasksBD> Listregistrado { get; private set; }

        public Command AddCommand { get; }

        public string ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;
               //LoadItemId(value);
            }
        }

        public bool esVisible
        {
            get => _esVisible;
            set
            {
                _esVisible = value;
                OnPropertyChanged();
            }
        }

        public async void LoadItemId(BAN_Stowage_Movimiento oDet, int arrastre)
        {
            try
            {
                //GetNovedades();
                await CargaDetalle(oDet, arrastre);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public string _temperature;
        public string TemperatureEntry
        {
            get => _temperature;
            set
            {
                _temperature = value;
                OnPropertyChanged();
            }
        }
        public string _ventilation;
        public string VentilationEntry
        {
            get => _ventilation;
            set
            {
                _ventilation = value;
                OnPropertyChanged();
            }
        }

        //CancellationTokenSource cts;
        public Command NavTaskCommand { get; set; }
        public ObservableCollection<Tasks> MyAgenda { get; private set; }
        public ICommand RefreshCommand => new Command(async () => await RefreshItemsAsync());

        public VBSOrderDispatchDetailsViewModel()
        {
            Title = "Detail of Orders Dispatch";
            _btnIcon = ImageSource.FromFile("icon_add.png");
            OnPropertyChanged(nameof(BtnIcon));
            var ids = App.Current.Properties["UserId"];
            iduser = ids.ToString();
            AddCommand = new Command<string>(OnAddTap);
            NavTaskCommand = new Command<string>(NavTaskDetail);
            ItemId = SecureStorage.GetAsync("WorkId").Result;

            GetAgenda(ItemId);

            //aplica permisos
            var _sUser = App.Current.Properties["SuperUser"];
            bool superUser = bool.Parse(_sUser.ToString());

            if (!superUser)
            {
                esVisible = true;
            }
            else
            {
                esVisible = true;
            }
        }

        async Task RefreshItemsAsync()
        {
            IsRefreshing = true;
            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
            await GetAgenda(ItemId);
            IsRefreshing = false;
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

        private async Task CargaDetalle(BAN_Stowage_Movimiento oDet, int arrastre)
        {
            if (oDet != null)
            {
                _selectedItem = oDet;
                //int vSaldo = (int)oDet.box; //- arrastre;
                //vSaldo = vSaldo - arrastre;
                container = "Exporter: " + oDet.oExportador?.nombre;
                taskslabel = "Location: " + oDet.oBloque?.oBodega?.nombre;
                temperature = "Block: " + oDet.oBloque?.nombre;
                types = string.Format("Total Pallets: [{0}]   Drag: [{1}]   Pending: [{2}]", oDet.palets.ToString(), arrastre, oDet.palets - arrastre);
                comentary = "Booking: " + oDet.booking;
                idWork = ItemId;
                pendiente = oDet.palets - arrastre;
            }
            else
            {
                await Shell.Current.GoToAsync("////VBSOrderDispatchInboxPage");
                return;
                container = "CARGA DEL EXPORTADOR COMPLETA";
                temperature = "Pending Pallets: 0";
                types = "Pending Boxes: 0";
                taskslabel = "Consignee: XXXXXXXXXXX XXXXXXXXX XXXXXXXXXXX";
                comentary = "";
                idWork = "0";
            }
            OnPropertyChanged(nameof(Container));
            OnPropertyChanged(nameof(Temperature));
            OnPropertyChanged(nameof(Types));
            OnPropertyChanged(nameof(TasksLabel));
            OnPropertyChanged(nameof(Comentary));

        }
        private async Task GetAgenda(string idSeleccioando)
        {
            try
            {
                
                var current = Connectivity.NetworkAccess;
                if (current == NetworkAccess.None)
                {
                    await App.Current.MainPage.DisplayAlert("Internet Error", "Please verify your internet connection", "OK");
                    return;
                }
                if (current == NetworkAccess.Internet)
                {
                    UserDialogs.Instance.ShowLoading("Loading...");
                    DatosApi datos = new DatosApi();
                    var ids = App.Current.Properties["UserId"];
                    int id = Convert.ToInt32(ids);
                    BAN_Stowage_Movimiento oDet;
                    int _arrastre;

                    var _sUser = App.Current.Properties["SuperUser"];
                    bool superUser = bool.Parse(_sUser.ToString());

                    
                    var v_data = ItemId.Split(',');
                    string v_idNave = v_data[0];
                    int v_idExportador = int.Parse(v_data[1]);
                    int v_idBloque = int.Parse(v_data[2]);
                    string v_booking = v_data[3];

                    MyAgenda = await datos.GetListaOrdenes(v_idNave, v_idExportador, v_idBloque, v_booking);
                    oDet = datos.oDetMovimiento;
                    _arrastre = datos.arrastre;

                    OnPropertyChanged(nameof(MyAgenda));

                    LoadItemId(oDet, _arrastre);
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
            PrintService imprimir = new PrintService();
            bool DeACuerdo;
            DeACuerdo = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Desea anular la orden de despacho?", "De Acuerdo", "Cancelar");
            IsBusy = true;
            if (DeACuerdo)
            {
                var ids = App.Current.Properties["Username"];
                string userName = ids.ToString();

                DatosApi datos = new DatosApi();
                var oEntidad = await datos.GetOrdenDespacho(long.Parse(obj));
                oEntidad.estado = "ANU";
                oEntidad.usuarioModifica = userName;

                ApiModels.AppModels.Base msg = await datos.AnularOrdenDespacho(oEntidad).ConfigureAwait(true);
                await App.Current.MainPage.DisplayAlert("Respuesta", msg.messages, "Close");
                if (msg.response == true)
                {
                    IsBusy = false;
                    await RefreshItemsAsync();
                }

            }
            IsBusy = false;
        }
        async private void OnAddTap(string obj)
        {
            try
            {
                if (idWork != "0")
                {
                    if (pendiente > 0)
                    {
                        await SecureStorage.SetAsync("WorkId", idWork);
                        await Shell.Current.GoToAsync("VBSOrderDispatchNewOrder");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Information", "Transaction Complete", "OK");
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Information", "Transaction Complete", "OK");
                }
                //await Shell.Current.GoToAsync("VBSOrderDispatchNewPage");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Cerrar");
            }
        }
    }
}
