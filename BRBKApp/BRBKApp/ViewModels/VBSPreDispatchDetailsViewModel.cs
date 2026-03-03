using Acr.UserDialogs;
using ApiModels.AppModels;
using BRBKApp.DA;
using BRBKApp.Views;
using Newtonsoft.Json;
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
    public class VBSPreDispatchDetailsViewModel : BaseViewModel
    {
        private ImageSource _btnIcon;
        public bool _esVisible;
        public string idWork;
        public string comentary;
        private string taskslabel;
        private string container;
        private string temperature;
        private string types;
        private BAN_Stowage_OrdenDespacho _selectedItem;
        public string iduser;
        private string itemId;
        bool isRefreshing;
        const int RefreshDuration = 2;

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

        public async void LoadItemId(BAN_Stowage_OrdenDespacho oEntidad, int arrastre)
        {
            try
            {
                //GetNovedades();
                await CargaDetalle(oEntidad, arrastre);
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

        public VBSPreDispatchDetailsViewModel()
        {
            Title = "Positions Rack";
            NavTaskCommand = new Command<string>(NavTaskDetail);
            _btnIcon = ImageSource.FromFile("icon_add.png");
            OnPropertyChanged(nameof(BtnIcon));
            var ids = App.Current.Properties["UserId"];
            iduser = ids.ToString();
            ItemId = SecureStorage.GetAsync("WorkId").Result;

            BAN_Stowage_OrdenDespacho oEntidad = JsonConvert.DeserializeObject<BAN_Stowage_OrdenDespacho>(ItemId.ToString());


            GetAgenda(oEntidad);

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
            BAN_Stowage_OrdenDespacho oEntidad = JsonConvert.DeserializeObject<BAN_Stowage_OrdenDespacho>(ItemId.ToString());
            await GetAgenda(oEntidad);
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

        private async Task CargaDetalle(BAN_Stowage_OrdenDespacho oDet, int arrastre)
        {
            if (oDet != null)
            {
                DatosApi datos = new DatosApi();
                var oDetalle = await datos.GetOrdenDespacho(long.Parse(oDet.idOrdenDespacho.ToString()));
                oDet = oDetalle;
                _selectedItem = oDetalle;
                //int vSaldo = (int)oDet.box; //- arrastre;
                //vSaldo = vSaldo - arrastre;

                if (oDetalle?.idOrdenDespacho is null)
                {
                    container = "CARGA EN PRE DESPACHO COMPLETA";
                    temperature = "Pending Pallets: 0";
                    types = "Pending Boxes: 0";
                    taskslabel = "Consignee: XXXXXXXXXXX XXXXXXXXX XXXXXXXXXXX";
                    comentary = "";
                    idWork = "0";

                    OnPropertyChanged(nameof(Container));
                    OnPropertyChanged(nameof(Temperature));
                    OnPropertyChanged(nameof(Types));
                    OnPropertyChanged(nameof(TasksLabel));
                    OnPropertyChanged(nameof(Comentary));

                    await Shell.Current.GoToAsync("////VBSPreDispatchInboxPage");
                    return;
                }
                else
                {
                    container = "Order Number : " + oDet.idOrdenDespacho?.ToString("D8");
                    comentary = "Exporter: " + oDet.oExportador?.nombre;
                    taskslabel = "Location: " + oDet.oBloque?.oBodega?.nombre;
                    temperature = "Block: " + oDet.oBloque?.nombre;
                    types = "Pending Pallets: [" + oDet.pendiente + "]    Processed Pallets: [" + oDet.arrastre + "]";

                    idWork = ItemId;
                }
            }
            else
            {
                container = "CARGA EN PRE DESPACHO COMPLETA";
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
        private async Task GetAgenda(BAN_Stowage_OrdenDespacho oSeleccionado)
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
                    
                    int _arrastre;

                    var _sUser = App.Current.Properties["SuperUser"];
                    bool superUser = bool.Parse(_sUser.ToString());                  

                    MyAgenda = await datos.GetListaDetOrdenesAgrupadaXFila(oSeleccionado, long.Parse(oSeleccionado.idOrdenDespacho.ToString()));
                    
                    _arrastre = datos.arrastre;

                    

                    OnPropertyChanged(nameof(MyAgenda));

                    LoadItemId(oSeleccionado, _arrastre);

                    //await RefreshItemsAsync();
                    //if (MyAgenda?.Count == 0)
                    //{
                    //    await Shell.Current.GoToAsync("////VBSPreDispatchInboxPage");
                    //}
                    //IsBusy = false;
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
            await Shell.Current.GoToAsync("VBSPreDispatchProcessPage");
        }
    }
}
