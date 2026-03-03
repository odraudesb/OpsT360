using Acr.UserDialogs;
using Android.Bluetooth;
using ApiDatos;
using ApiModels.AppModels;
using BRBKApp.DA;
using BRBKApp.Services;
using BRBKApp.Views;
using Lextm.SharpSnmpLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer;
using static Android.Resource;

namespace BRBKApp.ViewModels
{
    public class VBSTaskDetailsViewModel : BaseViewModel
    {
        private ImageSource _btnIcon;
        public bool _esVisible;
        public string idWork;
        public string comentary;
        private string taskslabel;
        private string container;
        private string temperature;
        private string types;
        private BAN_Stowage_Plan_Aisv _selectedItem;
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

        public async void LoadItemId(BAN_Stowage_Plan_Aisv oDet, int arrastre)
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

        public VBSTaskDetailsViewModel()
        {
            Title = "Detail Of Processed Boxes";
            _btnIcon = ImageSource.FromFile("icon_add.png");
            OnPropertyChanged(nameof(BtnIcon));
            var ids = App.Current.Properties["UserId"];
            iduser = ids.ToString();
            NavTaskCommand = new Command<string>(NavTaskDetail);
            
            AddCommand = new Command<string>(OnAddTap);
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

        private async Task CargaDetalle(BAN_Stowage_Plan_Aisv oDet, int arrastre)
        {
            if (oDet != null)
            {
                _selectedItem = oDet;
                int vSaldo = (int)oDet.box; //- arrastre;
                vSaldo = vSaldo - arrastre;
                container = "AISV: " + oDet.aisv + " TRUCK: " + _selectedItem.placa; ;
                taskslabel = "Exporter: " + oDet.oStowage_Plan_Det?.oExportador?.nombre;
                temperature = "Commodity: "+ oDet.oStowage_Plan_Det?.oCargo?.nombre + "/" + oDet.oStowage_Plan_Det?.oMarca?.nombre;
                types = "Qty: " + oDet.box + "     Drag: " + arrastre.ToString() + "    Pending Qty: " + vSaldo.ToString();
                comentary = "Planned Location: " + string.Format("{0} | Bloque: {1}", oDet.oStowage_Plan_Det?.oBodega?.nombre, oDet.oStowage_Plan_Det?.oBloque?.nombre);//oDet.Ubicaciones?.nombre;
                idWork = oDet.idStowageAisv.ToString();
            }
            else
            {
                container = "BL: MRN-MSN-HSN";
                temperature = "Qty = 0";
                types = "Drag: 0";
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
                    BAN_Stowage_Plan_Aisv oDet;
                    int _arrastre;

                    var _sUser = App.Current.Properties["SuperUser"];
                    bool superUser = bool.Parse(_sUser.ToString());

                    MyAgenda = await datos.GetListaMovimientos(long.Parse(idSeleccioando), id);
                    oDet = datos.oDetAisv;
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
            DeACuerdo = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Desea imprmir la etiqueta del pallet?", "De Acuerdo", "Cancelar");
            IsBusy = true;
            if (DeACuerdo)
            {
                DatosApi datos = new DatosApi();
                BAN_Stowage_Plan_Aisv oDet;
                var oMovimiento = await datos.GetRecepcionAisvPorId(long.Parse(obj));
                oDet = oMovimiento.oStowage_Plan_Aisv;
                string ZPL = imprimir.ZPLDesignEtiqueta(oMovimiento.barcode.Trim(), oMovimiento.fechaCreacion?.ToString("dd/MM/yyyy"), oDet?.booking.Trim(), oDet?.aisv.Trim(), oDet?.oStowage_Plan_Det?.oExportador?.nombre.Trim(), oDet?.oStowage_Plan_Det?.oStowage_Plan_Cab?.nave.Trim(), oMovimiento?.oModalidad?.nombre.Trim(), oDet?.dae.Trim(), oMovimiento?.cantidad.ToString());
                imprimir.Print(ZPL);
            }
            else
            {
                DeACuerdo = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Desea anular el registro?", "De Acuerdo", "Cancelar");
                IsBusy = true;
                if (DeACuerdo)
                {
                    var ids = App.Current.Properties["Username"];
                    string userName = ids.ToString();

                    DatosApi datos = new DatosApi();
                    BAN_Stowage_Plan_Aisv oDet;
                    var oMovimiento = await datos.GetRecepcionAisvPorId(long.Parse(obj));
                    oMovimiento.usuarioModifica = userName;

                    ApiModels.AppModels.Base msg = await datos.AnularMovimientoVBS(oMovimiento).ConfigureAwait(true);
                    await App.Current.MainPage.DisplayAlert("Respuesta", msg.messages, "Close");
                    if (msg.response == true)
                    {
                        IsBusy = false;
                        await RefreshItemsAsync();
                    }

                }
            }
            IsBusy = false;
        }

        async private void OnAddTap(string obj)
        {
            try
            {
                await SecureStorage.SetAsync("WorkId", idWork);
                await Shell.Current.GoToAsync("VBSNewDetail");
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Cerrar");
            }
        }
        #region "EXAMPLE PRINT BLUETOOTH"
        //*********EXAMPLE PRINT ZEBRA BLUETOOTH******************
        public enum ConnectionType
        {
            Network,
            Bluetooth
        }
        private async void GetPrinterStatusButton_Clicked(object sender, EventArgs eventArgs)
        {
            string _MacAddressEntry =  string.Empty;
            //AvailableChannelsLabel.Text = "";
            //PrinterStatusLabel.Text = "Retrieving printer status...";
            //SetInputEnabled(false);

            StatusConnection statusConnection = null;
            Connection rawConnection = null;

            try
            {
                statusConnection = CreateStatusConnection();

                if (statusConnection == null)
                {
                    return;
                }

                if (GetSelectedConnectionType() == ConnectionType.Bluetooth)
                {
                    
                    try
                    {
                        BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

                        if (bluetoothAdapter == null || !bluetoothAdapter.IsEnabled)
                        {
                            // Bluetooth no está disponible o no está habilitado
                            return;
                        }

                        var pairedDevices = bluetoothAdapter.BondedDevices;
                        if (pairedDevices != null && pairedDevices.Count > 0)
                        {
                            foreach (var device1 in pairedDevices)
                            {
                                // Aquí puedes listar los dispositivos emparejados
                                Console.WriteLine(device1.Name + " - " + device1.Address);

                                if (device1.Name.Contains("zq230"))
                                {
                                    _MacAddressEntry = device1.Address;
                                }
                            }
                        }
                        // Over Bluetooth, the printer only broadcasts the status connection if a valid raw connection is open
                        rawConnection = DependencyService.Get<IConnectionManager>().GetBluetoothConnection(_MacAddressEntry);
                    }
                    catch (NotImplementedException)
                    {
                        throw new NotImplementedException("Bluetooth connection not supported on this platform");
                    }

                    await Task.Factory.StartNew(() => {
                        rawConnection.Open();
                    });

                    await Task.Delay(3000); // Give the printer some time to start the status connection
                }

                await Task.Factory.StartNew(() => {
                    statusConnection.Open();

                    ZebraPrinter printer = ZebraPrinterFactory.GetLinkOsPrinter(statusConnection);
                    PrinterStatus printerStatus = printer.GetCurrentStatus();

                    Device.BeginInvokeOnMainThread(() => {
                        UpdateResult(printerStatus);
                    });
                });
            }
            catch (Exception e)
            {
                //PrinterStatusLabel.Text = $"Error: {e.Message}";
                //await DisplayAlert("Error", e.Message, "OK");
                await App.Current.MainPage.DisplayAlert("Connection Error", e.Message, "OK");
                return;
            }
            finally
            {
                try
                {
                    statusConnection?.Close();
                    rawConnection?.Close();
                }
                catch (ConnectionException) { }

                SetInputEnabled(true);
            }
        }

        private async void FindAvailableChannelsButton_Clicked(object sender, EventArgs eventArgs)
        {
            //AvailableChannelsLabel.Text = "Finding available channels...";
            //PrinterStatusLabel.Text = "";
            SetInputEnabled(false);

            try
            {
                await Task.Factory.StartNew(() => {
                    string connectionChannels;
                    try
                    {
                        connectionChannels = DependencyService.Get<IConnectionManager>().BuildBluetoothConnectionChannelsString("");
                    }
                    catch (NotImplementedException)
                    {
                        throw new NotImplementedException("Bluetooth connection channels not supported on this platform");
                    }

                    Device.BeginInvokeOnMainThread(() => {
                        //AvailableChannelsLabel.Text = connectionChannels;
                    });
                });
            }
            catch (Exception e)
            {
                //AvailableChannelsLabel.Text = $"Error: {e.Message}";
                //await DisplayAlert("Error", e.Message, "OK");
            }
            finally
            {
                SetInputEnabled(true);
            }
        }

        private int GetStatusPortNumber(string portNumberString)
        {
            if (!string.IsNullOrWhiteSpace(portNumberString))
            {
                try
                {
                    return int.Parse(portNumberString);
                }
                catch (Exception)
                {
                    throw new ArgumentException("Status port number must be an integer");
                }
            }
            else
            {
                return 9200;
            }
        }

        private ConnectionType? GetSelectedConnectionType()
        {
            string connectionType = "Bluetooth";// (string)ConnectionTypePicker.SelectedItem;
            switch (connectionType)
            {
                case "Network":
                    return ConnectionType.Network;
                case "Bluetooth":
                    return ConnectionType.Bluetooth;
                default:
                    return null;
            }
        }

        private StatusConnection CreateStatusConnection()
        {
            switch (GetSelectedConnectionType())
            {
                case ConnectionType.Network:
                    return null;// new TcpStatusConnection(AddressEntry.Text, GetStatusPortNumber(StatusPortNumberEntry.Text));

                case ConnectionType.Bluetooth:
                    try
                    {
                        return DependencyService.Get<IConnectionManager>().GetBluetoothStatusConnection("");
                    }
                    catch (NotImplementedException)
                    {
                        throw new NotImplementedException("Bluetooth status connection not supported on this platform");
                    }

                default:
                    throw new ArgumentNullException("No connection type selected");
            }
        }

        private void UpdateResult(PrinterStatus printerStatus)
        {
            //StringBuilder sb = new StringBuilder();

            //if (printerStatus != null)
            //{
            //    sb.AppendLine($"Printer ready: {printerStatus.isReadyToPrint}");
            //    sb.AppendLine($"Head open: {printerStatus.isHeadOpen}");
            //    sb.AppendLine($"Paper out: {printerStatus.isPaperOut}");
            //    sb.AppendLine($"Printer paused: {printerStatus.isPaused}");
            //    sb.AppendLine($"Labels remaining in batch: {printerStatus.labelsRemainingInBatch}");
            //}

            //PrinterStatusLabel.Text = sb.ToString();
        }

        private void SetInputEnabled(bool enabled)
        {
            Device.BeginInvokeOnMainThread(() => {
                //ConnectionTypePicker.IsEnabled = enabled;
                //StatusPortNumberEntry.IsEnabled = enabled;
                //AddressEntry.IsEnabled = enabled;
                //FindAvailableChannelsButton.IsEnabled = enabled;
                //GetPrinterStatusButton.IsEnabled = enabled;
            });
        }
        //***************************
        #endregion
    }
}
