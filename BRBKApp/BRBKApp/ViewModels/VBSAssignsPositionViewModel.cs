using ApiModels.AppModels;
using BRBKApp.DA;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using BRBKApp.Views;
using System.Linq;
using System.IO;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Essentials;
using Acr.UserDialogs;
using System.Threading.Tasks;

namespace BRBKApp.ViewModels
{
    public class VBSAssignsPositionViewModel : BaseViewModel
    {
        #region Declaraciones
        bool isRefreshing;
        public bool _esActivo;
        const int RefreshDuration = 2;
        public string idWork;
        public string AdidWork;
        public string _barcodeEntry;
        private BAN_Stowage_Plan_Aisv _selectedItem;
        private BAN_Stowage_Plan_Aisv _selectedItemAd;
        private ImageSource _btnIcon;
        private string container;
        private string temperature;
        private string types;
        private string taskslabel;
        public string iduser;
        public string comentary;
        private string txtNumMovimientoBarcode = null;

        private string Adcontainer;
        private string Adtemperature;
        private string Adtypes;
        private string Adtaskslabel;
        public string Adcomentary;
        private string txtAdNumMovimientoBarcode = null;

        private Combo _selectedNov;

        public Command ConsultCommand { get; }
        public Command AdConsultCommand { get; }
        public ImageSource BtnIcon
        {
            get { return _btnIcon; }
            set
            {
                _btnIcon = value;
                SetProperty(ref _btnIcon, value);
            }
        }
        public string TxtNumMovimientoBarcode
        {
            get => txtNumMovimientoBarcode;
            set
            {
                txtNumMovimientoBarcode = value;
                OnPropertyChanged();
            }
        }
        public string TxtAdNumMovimientoBarcode
        {
            get => txtAdNumMovimientoBarcode;
            set
            {
                txtAdNumMovimientoBarcode = value;
                OnPropertyChanged();
            }
        }
        public BAN_Stowage_Movimiento MyAgenda { get; private set; }
        public BAN_Stowage_Movimiento AdMyAgenda { get; private set; }
        public Command ReportAlertCommand { get; }
        public Command SaveChangesCommand { get; }
        public Command CancelCommand { get; }
        public Command TapCommand { get; }
        public List<TasksBD> Listregistrado { get; private set; }

        public string BarcodeEntry
        {
            get => _barcodeEntry;
            set
            {
                _barcodeEntry = value;
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

        public string Container
        {
            get => container;
            set => SetProperty(ref container, value);
        }

        public string AdContainer
        {
            get => Adcontainer;
            set => SetProperty(ref Adcontainer, value);
        }
        public string Comentary
        {
            get => comentary;
            set => SetProperty(ref comentary, value);
        }
        public string AdComentary
        {
            get => Adcomentary;
            set => SetProperty(ref Adcomentary, value);
        }
        public string Temperature
        {
            get => temperature;
            set => SetProperty(ref temperature, value);
        }
        public string AdTemperature
        {
            get => Adtemperature;
            set => SetProperty(ref Adtemperature, value);
        }
        public string Types
        {
            get => types;
            set => SetProperty(ref types, value);
        }

        public string AdTypes
        {
            get => Adtypes;
            set => SetProperty(ref Adtypes, value);
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
        public string TasksLabel
        {
            get => taskslabel;
            set => SetProperty(ref taskslabel, value);
        }

        public string AdTasksLabel
        {
            get => Adtaskslabel;
            set => SetProperty(ref Adtaskslabel, value);
        }

        public Combo SelectedNov
        {
            get { return _selectedNov; }
            set { _selectedNov = value; OnPropertyChanged(); }
        }
        public List<Combo> ListProfundidad { get; private set; }
      
        #endregion
        public VBSAssignsPositionViewModel()
        {
            Title = "Assigns Location";
             _btnIcon = ImageSource.FromFile("icon_search.png");

            OnPropertyChanged(nameof(BtnIcon));

            ConsultCommand = new Command(OnConsultClicked);
            AdConsultCommand = new Command(OnAdConsultClicked);
            TapCommand = new Command(OnTapped);
            SaveChangesCommand = new Command(async (o) => await OnSaveClicked(o));
            CancelCommand = new Command(OnCancelClicked);
            esActivo = true;
        }
        public async void OnConsultClicked(object obj)
        {
            if (TxtNumMovimientoBarcode?.Length == 13)
            { 
                esActivo = false;
                IsRefreshing = true;
                await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
                var vBarcode = TxtNumMovimientoBarcode;
                CleanPages();
                await GetAgenda(vBarcode);
                IsRefreshing = false;
                esActivo = true;
            }
        }
        public async void OnAdConsultClicked(object obj)
        {
            if (TxtAdNumMovimientoBarcode?.Length == 13)
            {
                esActivo = false;
                IsRefreshing = true;
                await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
                var vBarcode = TxtAdNumMovimientoBarcode;
                //CleanPages();
                await GetAdAgenda(vBarcode);
                IsRefreshing = false;
                esActivo = true;
            }
        }
        private async Task GetAgenda(string _barcode)
        {
            if (string.IsNullOrEmpty(_barcode)) { return; }
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
                    MyAgenda = await datos.GetRecepcionAisvPorBarcode(_barcode);
                    OnPropertyChanged(nameof(MyAgenda));
                    if (MyAgenda.idStowageAisv > 0)
                    {
                        LoadItemId(MyAgenda);
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Respuesta", MyAgenda.messages, "OK");
                    }
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

        private async Task GetAdAgenda(string _barcode)
        {
            if (string.IsNullOrEmpty(_barcode)) { return; }
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
                    AdMyAgenda = await datos.GetRecepcionAisvPorBarcode(_barcode);
                    OnPropertyChanged(nameof(AdMyAgenda));
                    if (AdMyAgenda.idStowageAisv > 0)
                    {
                        AdLoadItemId(AdMyAgenda);
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Respuesta", MyAgenda.messages, "OK");
                    }
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
        public async void LoadItemId(BAN_Stowage_Movimiento oRecepcion)
        {
            try
            {
                await CargaDetalle(oRecepcion);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async void AdLoadItemId(BAN_Stowage_Movimiento oRecepcion)
        {
            try
            {
                await AdCargaDetalle(oRecepcion);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task CargaDetalle(BAN_Stowage_Movimiento oRecepcion)
        {
            if (oRecepcion.idStowageAisv > 0)
            {
                if (oRecepcion.estado == "NUE" || oRecepcion.estado == "CON")
                {
                    ///////////////////////////////////
                    //   llenar combo de ubicaciones
                    ///////////////////////////////////
                    /*DatosApi datos = new DatosApi();
                    var dd = await datos.GetListaProfundidad();
                    ListProfundidad = dd;
                    SelectedNov = ListProfundidad?.FirstOrDefault();
                    OnPropertyChanged(nameof(ListProfundidad));*/

                    ///////////////////////////////////
                    //   carga datos en pantalla
                    ///////////////////////////////////
                    //TxtNumMovimientoBarcode = oRecepcion.barcode;
                    _selectedItem = oRecepcion.oStowage_Plan_Aisv;
                    container = "***** " + oRecepcion.oEstado?.nombre + " *****" + "\nAISV: " + _selectedItem.aisv + " \nQR: " + oRecepcion.barcode ;
                    taskslabel = "Cliente: " + _selectedItem.oStowage_Plan_Det?.oExportador?.nombre + "\nCargo/Marca: " + _selectedItem.oStowage_Plan_Det?.oCargo?.nombre + "/" + _selectedItem.oStowage_Plan_Det?.oMarca?.nombre; ;
                    temperature = "Referencia: " + _selectedItem.oStowage_Plan_Det?.oStowage_Plan_Cab?.nave;
                    types = "Booking: " + _selectedItem?.booking;
                    comentary = "Bodega: " + _selectedItem.oStowage_Plan_Det?.oBodega.nombre + " Bloque: " + _selectedItem.oStowage_Plan_Det?.oBloque.nombre + "\nModalidad: " + oRecepcion?.oModalidad?.nombre + "\nCantidad: " + oRecepcion?.cantidad;
                    idWork = oRecepcion.idMovimiento.ToString();
                    //BarcodeEntry = oRecepcion.cantidad.ToString();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("information", string.Format("Load is {0}",oRecepcion.oEstado.nombre), "Close");
                }
            }
            else
            {
                container = "BL: MRN-MSN-HSN";
                temperature = "Cantidad = 0";
                //types = "Arrastre: 0";
                taskslabel = "Cliente: XXXXXXXXXXX XXXXXXXXX XXXXXXXXXXX";
                comentary = "";
                idWork = "0";
            }
            OnPropertyChanged(nameof(Container));
            OnPropertyChanged(nameof(Temperature));
            OnPropertyChanged(nameof(Types));
            OnPropertyChanged(nameof(TasksLabel));
            OnPropertyChanged(nameof(Comentary));
            OnPropertyChanged(nameof(TxtNumMovimientoBarcode));
            OnPropertyChanged(nameof(BarcodeEntry));
        }

        private async Task AdCargaDetalle(BAN_Stowage_Movimiento oRecepcion)
        {
            if (oRecepcion.idStowageAisv > 0)
            {
                if (oRecepcion.estado == "NUE" || oRecepcion.estado == "CON")
                {
                   
                    ///////////////////////////////////
                    //   carga datos en pantalla
                    ///////////////////////////////////
                    //TxtNumMovimientoBarcode = oRecepcion.barcode;
                    _selectedItemAd = oRecepcion.oStowage_Plan_Aisv;
                    Adcontainer = "***** " + oRecepcion.oEstado?.nombre + " *****" + "\nAISV: " + _selectedItemAd.aisv + " \nQR: " + oRecepcion.barcode;
                    Adtaskslabel = "Cliente: " + _selectedItemAd.oStowage_Plan_Det?.oExportador?.nombre + "\nCargo/Marca: " + _selectedItemAd.oStowage_Plan_Det?.oCargo.nombre + "/" + _selectedItemAd.oStowage_Plan_Det?.oMarca.nombre; ;
                    Adtemperature = "Referencia: " + _selectedItemAd.oStowage_Plan_Det?.oStowage_Plan_Cab?.nave;
                    Adtypes = "Booking: " + _selectedItemAd?.booking;
                    Adcomentary = "Bodega: " + _selectedItemAd.oStowage_Plan_Det?.oBodega.nombre + " Bloque: " + _selectedItemAd.oStowage_Plan_Det?.oBloque.nombre + "\nModalidad: " + oRecepcion?.oModalidad?.nombre + "\nCantidad: " + oRecepcion?.cantidad;
                    AdidWork = oRecepcion.idMovimiento.ToString();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("information", string.Format("Load is {0}", oRecepcion.oEstado.nombre), "Close");
                }
            }
            else
            {
                Adcontainer = "BL: MRN-MSN-HSN";
                Adtemperature = "Cantidad = 0";
                Adtypes = "Arrastre: 0";
                Adtaskslabel = "Cliente: XXXXXXXXXXX XXXXXXXXX XXXXXXXXXXX";
                Adcomentary = "";
                AdidWork = "0";
            }
            OnPropertyChanged(nameof(AdContainer));
            OnPropertyChanged(nameof(AdTemperature));
            OnPropertyChanged(nameof(AdTypes));
            OnPropertyChanged(nameof(AdTasksLabel));
            OnPropertyChanged(nameof(AdComentary));
            OnPropertyChanged(nameof(TxtAdNumMovimientoBarcode));
            OnPropertyChanged(nameof(BarcodeEntry));
        }
        public void CleanPages()
        {
            IsBusy = false;
            MyAgenda = null;
            _selectedItem = null;
            container = string.Empty;
            temperature = string.Empty;
            types = string.Empty;
            taskslabel = string.Empty;
            comentary = string.Empty;
            idWork = "0";

            Container = null;
            Temperature = "";
            Types = "";
            TasksLabel = "";
            Comentary = "";
            TxtNumMovimientoBarcode = "";
            BarcodeEntry = "";


            AdMyAgenda = null;
            _selectedItemAd = null;
            Adcontainer = string.Empty;
            Adtemperature = string.Empty;
            Adtypes = string.Empty;
            Adtaskslabel = string.Empty;
            Adcomentary = string.Empty;
            AdidWork = "0";

            AdContainer = null;
            AdTemperature = "";
            AdTypes = "";
            TasksLabel = "";
            AdComentary = "";
            TxtAdNumMovimientoBarcode = "";

            OnPropertyChanged(nameof(Container));
            OnPropertyChanged(nameof(Temperature));
            OnPropertyChanged(nameof(Types));
            OnPropertyChanged(nameof(TasksLabel));
            OnPropertyChanged(nameof(Comentary));
            OnPropertyChanged(nameof(TxtNumMovimientoBarcode));
            OnPropertyChanged(nameof(BarcodeEntry));

            OnPropertyChanged(nameof(AdContainer));
            OnPropertyChanged(nameof(AdTemperature));
            OnPropertyChanged(nameof(AdTypes));
            OnPropertyChanged(nameof(AdTasksLabel));
            OnPropertyChanged(nameof(AdComentary));
            OnPropertyChanged(nameof(TxtAdNumMovimientoBarcode));
        }
        private void OnCancelClicked(object obj)
        {
            CleanPages();
        }

        private async Task OnSaveClicked(object obj)
        {
            try
            {
                bool DeACuerdo;
                bool flags = false;
                DeACuerdo = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Está seguro de asignar la ubicación?", "De Acuerdo", "Cancelar");

                var ids = App.Current.Properties["Username"];
                string userName = ids.ToString();

                if (DeACuerdo)
                {
                    if (string.IsNullOrEmpty(SelectedNov?.Valor))
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please enter Depth", "Close");
                        flags = true;
                    }

                    if (string.IsNullOrEmpty(BarcodeEntry))
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please enter Barcode", "Close");
                        flags = true;
                    }

                    if (flags == false)
                    {
                        var current = Connectivity.NetworkAccess;
                        if (current == NetworkAccess.None)
                        {
                            await App.Current.MainPage.DisplayAlert("Internet Error", "Please verify your internet connection", "OK");
                            return;
                        }
                        if (current == NetworkAccess.Internet)
                        {
                            //SE ASIGNA UNA UBICACION A 2 PALLET
                            if (MyAgenda != null && AdMyAgenda != null)
                            {
                                DeACuerdo = await App.Current.MainPage.DisplayAlert("Confirmar", "Se ha detectado Pallet consolidado para una misma ubicación. ¿Está seguro de asignar la ubicación?", "De Acuerdo", "Cancelar");
                                if (DeACuerdo)
                                { 
                                    if (MyAgenda.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idExportador != AdMyAgenda.oStowage_Plan_Aisv?.oStowage_Plan_Det?.idExportador)
                                    {
                                        await App.Current.MainPage.DisplayAlert("Error", "La carga debe pertenecer al mismo exportador", "Close");
                                        return;
                                    }
                                    int vTotal = MyAgenda.cantidad + AdMyAgenda.cantidad;

                                    if (vTotal > 48)
                                    {
                                        await App.Current.MainPage.DisplayAlert("Error", "La cantidad de cajas consolidadas no debe ser mayor a 48", "Close");
                                        return;
                                    }


                                    UserDialogs.Instance.ShowLoading("Sending...");
                                    DatosApi datos = new DatosApi();

                                    string _barcode = BarcodeEntry.ToString() + "-" + SelectedNov.Valor;

                                    var oUbicacion = await datos.GetUbicacionPorBarcode(_barcode).ConfigureAwait(true);

                                    if (oUbicacion != null)
                                    {
                                        if (oUbicacion.disponible)
                                        {
                                            BAN_Stowage_Movimiento oEntidad = new BAN_Stowage_Movimiento();
                                            oEntidad.idStowageAisv = long.Parse(MyAgenda.idStowageAisv.ToString());
                                            oEntidad.idMovimiento = long.Parse(idWork.ToString());
                                            oEntidad.tipo = "ING";
                                            oEntidad.idUbicacion = oUbicacion.id;
                                            oEntidad.usuarioModifica = userName;
                                            oEntidad.isMix = true;
                                            oEntidad.referencia = AdMyAgenda.barcode;
                                            ApiModels.AppModels.Base msg = await datos.ActualizarMovimientoVBS(oEntidad).ConfigureAwait(true);
                                            if (msg.response == true)
                                            {
                                                oEntidad.idStowageAisv = long.Parse(AdMyAgenda.idStowageAisv.ToString());
                                                oEntidad.idMovimiento = long.Parse(AdidWork.ToString());
                                                oEntidad.tipo = "ING";
                                                oEntidad.idUbicacion = oUbicacion.id;
                                                oEntidad.usuarioModifica = userName;
                                                oEntidad.isMix = true;
                                                oEntidad.referencia = MyAgenda.barcode;
                                                ApiModels.AppModels.Base msg1 = await datos.ActualizarMovimientoVBS(oEntidad).ConfigureAwait(true);
                                                await App.Current.MainPage.DisplayAlert("Respuesta", msg.messages + " \n" + msg1.messages, "Close");
                                                if (msg1.response == true)
                                                {
                                                    CleanPages();
                                                }
                                            }
                                        }
                                        else
                                        {
                                            await App.Current.MainPage.DisplayAlert("Location Error", "Ubicación no esta disponible, por favor verifique el Slot, ", "OK");
                                            return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (MyAgenda != null)
                                {
                                    UserDialogs.Instance.ShowLoading("Sending...");
                                    DatosApi datos = new DatosApi();

                                    string _barcode = BarcodeEntry.ToString() + "-" + SelectedNov.Valor;

                                    var oUbicacion = await datos.GetUbicacionPorBarcode(_barcode).ConfigureAwait(true);

                                    if (oUbicacion != null)
                                    {
                                        if (oUbicacion.disponible)
                                        {
                                            BAN_Stowage_Movimiento oEntidad = new BAN_Stowage_Movimiento();
                                            oEntidad.idStowageAisv = long.Parse(MyAgenda.idStowageAisv.ToString());
                                            oEntidad.idMovimiento = long.Parse(idWork.ToString());
                                            oEntidad.tipo = "ING";
                                            oEntidad.idUbicacion = oUbicacion.id;
                                            //oEntidad.estado = "CON";
                                            oEntidad.usuarioModifica = userName;

                                            ApiModels.AppModels.Base msg = await datos.ActualizarMovimientoVBS(oEntidad).ConfigureAwait(true);
                                            await App.Current.MainPage.DisplayAlert("Respuesta", msg.messages, "Close");
                                            if (msg.response == true)
                                            {
                                                CleanPages();
                                            }
                                        }
                                        else
                                        {
                                            await App.Current.MainPage.DisplayAlert("Location Error", "Ubicación no esta disponible, por favor verifique el Slot, ", "OK");
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    if (TxtNumMovimientoBarcode == "")
                                    {
                                        await App.Current.MainPage.DisplayAlert("Respuesta", "Ingrese el barcode de la carga, luego presione el botón de consulta", "Close");
                                    }
                                    else
                                    {
                                        OnConsultClicked(null);
                                        await App.Current.MainPage.DisplayAlert("Respuesta", "La información de la recepción se ha refrescado, Intente nuevamente por favor", "Close");
                                    }
                                }
                            }
                        }
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
        private async void OnTapped(object s)
        {

            string action = await App.Current.MainPage.DisplayActionSheet("¿What do you want to do?", "Cancel", null,
                "Take Photo", "Delete Photo");


            if (action == "Take Photo")
            {
                Camara(1, s);
            }

            //if (action == "Subir Foto")
            //{
            //    Camara(2, s);
            //}
            /*if (action == "Delete Photo")
            {
                string ss = s.ToString();
                switch (ss)
                {
                    case "0":
                        _imageSource = ImageSource.FromFile("icon.png");
                        ArrayFoto = null;
                        OnPropertyChanged(nameof(ImageSource));
                        break;
                    case "1":
                        _imageSource1 = ImageSource.FromFile("icon.png");
                        ArrayFoto1 = null;
                        OnPropertyChanged(nameof(ImageSource1));
                        break;
                    case "2":
                        _imageSource2 = ImageSource.FromFile("icon.png");
                        ArrayFoto2 = null;
                        OnPropertyChanged(nameof(ImageSource2));
                        break;
                    case "3":
                        _imageSource3 = ImageSource.FromFile("icon.png");
                        ArrayFoto3 = null;
                        OnPropertyChanged(nameof(ImageSource3));
                        break;
                    default:
                        break;
                }
            }*/
        }
        private async void Camara(int Accion, object imageControl)
        {
            try
            {
                MediaFile file = null;
                await CrossMedia.Current.Initialize();
                if (!CrossMedia.Current.IsCameraAvailable
                    || !CrossMedia.Current.IsTakePhotoSupported
                    || !CrossMedia.Current.IsPickPhotoSupported
                    )
                {
                    await App.Current.MainPage.DisplayAlert("Camara no habilitada", "Revise su dispositivo", "Cerrar");
                    return;
                }
                if (Accion == 1) //TOMA FOTO
                {
                    file = await CrossMedia.Current.TakePhotoAsync(
                         new StoreCameraMediaOptions
                         {
                             SaveToAlbum = true,
                             PhotoSize = PhotoSize.Small
                             //CustomPhotoSize = 70
                         });
                }
                if (Accion == 2) //SUBE FOTO
                {
                    PickMediaOptions c = new PickMediaOptions();
                    c.PhotoSize = PhotoSize.Small;
                    file = await CrossMedia.Current.PickPhotoAsync(c);
                }
                /*if (Accion != 3 && file != null)
                {
                    ArrayFotoX = ReadImage(file.GetStream());
                    _imageSourceX = ImageSource.FromStream(() =>
                    {
                        var stream = file.GetStream();
                        ArrayFotoX = ReadImage(file.GetStream());

                        file.Dispose();
                        return stream;
                    }
                   );

                    switch (imageControl)
                    {
                        case "0":
                            _imageSource = _imageSourceX;
                            ArrayFoto = ArrayFotoX;
                            OnPropertyChanged(nameof(ImageSource));
                            break;
                        case "1":
                            _imageSource1 = _imageSourceX;
                            ArrayFoto1 = ArrayFotoX;
                            OnPropertyChanged(nameof(ImageSource1));
                            break;
                        case "2":
                            _imageSource2 = _imageSourceX;
                            ArrayFoto2 = ArrayFotoX;
                            OnPropertyChanged(nameof(ImageSource2));
                            break;
                        case "3":
                            _imageSource3 = _imageSourceX;
                            ArrayFoto3 = ArrayFotoX;
                            OnPropertyChanged(nameof(ImageSource3));
                            break;
                        default:
                            break;
                    }
                    //_imageSource.WidthRequest = 100;
                    //_imageSource.HeightRequest = 100;
                    //this.FrameFoto1.Padding = 0;
                }
                if (Accion == 3 && file != null)
                {
                    file = await CrossMedia.Current.PickPhotoAsync();

                    _imageSourceX = ImageSource.FromStream(() =>
                    {
                        var stream = file.GetStream();
                        ArrayFotoX = ReadImage(file.GetStream());
                        file.Dispose();
                        return stream;
                    }
                    );
                    switch (imageControl)
                    {
                        case "0":
                            _imageSource = _imageSourceX;
                            ArrayFoto = ArrayFotoX;
                            OnPropertyChanged(nameof(ImageSource));
                            break;
                        case "1":
                            _imageSource1 = _imageSourceX;
                            ArrayFoto1 = ArrayFotoX;
                            OnPropertyChanged(nameof(ImageSource1));
                            break;
                        case "2":
                            _imageSource2 = _imageSourceX;
                            ArrayFoto2 = ArrayFotoX;
                            OnPropertyChanged(nameof(ImageSource2));
                            break;
                        case "3":
                            _imageSource3 = _imageSourceX;
                            ArrayFoto3 = ArrayFotoX;
                            OnPropertyChanged(nameof(ImageSource3));
                            break;
                        default:
                            break;
                    }
                }*/
                if (file == null)
                {
                    //await App.Current.MainPage.DisplayAlert("Camara", "No realizo nada con la camara", "Cerrar");
                    return;
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Cerrar");
            }
        }
        public byte[] ReadImage(Stream Imput)
        {
            BinaryReader reader = new BinaryReader(Imput);
            byte[] imgByte = reader.ReadBytes((int)Imput.Length);

            return imgByte;
        }
        public ImageSource CreateImage(byte[] input)
        {
            Stream streamr;
            Image image = new Image();
            streamr = new MemoryStream(input);
            return image.Source = ImageSource.FromStream(() =>
            {
                return streamr;
            });
        }

        public async void CargaLista()
        {
            try
            {
                var v_data = BarcodeEntry.ToString().Split('-');

                if (v_data.Count() == 4)
                {
                    if (string.IsNullOrEmpty(v_data[3])) { return; }
                    int v_idBodega = int.Parse(v_data[0]);
                    int v_idBloque = int.Parse(v_data[1]);
                    int v_idFila = int.Parse(v_data[2]);
                    int v_idAltura = int.Parse(v_data[3]);

                    ///////////////////////////////////
                    //   llenar combo de ubicaciones
                    ///////////////////////////////////

                    DatosApi datos = new DatosApi();
                    var dd = await datos.GetListaProfundidad( v_idBodega, v_idBloque, v_idFila, v_idAltura);
                    ListProfundidad = dd;
                    SelectedNov = ListProfundidad?.FirstOrDefault();
                    OnPropertyChanged(nameof(ListProfundidad));
                }
                else
                {
                    ListProfundidad = null;
                    SelectedNov = null;
                    OnPropertyChanged(nameof(ListProfundidad));
                }
            }
            catch
            {
                ListProfundidad = null;
                OnPropertyChanged(nameof(ListProfundidad));
            }
        }
    }
}
