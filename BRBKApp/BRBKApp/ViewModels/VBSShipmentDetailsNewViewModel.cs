using ApiModels.AppModels;
using Plugin.Media;
using Plugin.Media.Abstractions;
using BRBKApp.DA;
using BRBKApp.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Acr.UserDialogs;
using System.Threading.Tasks;
using System.Linq;

namespace BRBKApp.ViewModels
{
    public class VBSShipmentDetailsNewViewModel : BaseViewModel
    {
        #region Declaraciones

        public string idWork;
        private string itemId;
        private BAN_Embarque_Cab _selectedItem;

        public string _lugar;
        private string _codeBox;
        private string _qty;
        private string _ubicacion;
        private string _observacion;
        private string _estado;
        private string container;
        private string temperature;
        private string types;
        private string taskslabel;
        public string iduser;
        public string comentary;
        private Combo _selectedHold;
        private Combo _selectedDeck;
        private Combo _selectedBrands;
        private Combo _selectedEvents;
        private Combo _selectedOrigen;
        private Combo _selectedModalidad;

        public BAN_Stowage_Movimiento MyAgenda { get; private set; }
        public Command ReportAlertCommand { get; }
        public Command SaveChangesCommand { get; }
        public Command CancelCommand { get; }
        public Command TapCommand { get; }
        public Combo SelectedHold
        {
            get { return _selectedHold; }
            set { _selectedHold = value; OnPropertyChanged(); }
        }
        public List<Combo> ListHold { get; private set; }
        public Combo SelectedDeck
        {
            get { return _selectedDeck; }
            set { _selectedDeck = value; OnPropertyChanged(); }
        }
        public List<Combo> ListDeck { get; private set; }
        public Combo SelectedBrand
        {
            get { return _selectedBrands; }
            set { _selectedBrands = value; OnPropertyChanged(); }
        }
        public List<Combo> ListBrand { get; private set; }
        public Combo SelectedModalidad
        {
            get { return _selectedModalidad; }
            set { _selectedModalidad = value; OnPropertyChanged(); }
        }
        public List<Combo> ListModalidad { get; private set; }
        public Combo SelectedEvents
        {
            get { return _selectedEvents; }
            set { _selectedEvents = value; OnPropertyChanged(); }
        }
        public List<Combo> ListEvents { get; private set; }

        public Combo SelectedOrigen
        {
            get { return _selectedOrigen; }
            set { _selectedOrigen = value; OnPropertyChanged(); }
        }
        public List<Combo> ListOrigen { get; private set; }

        public string LugarEntry
        {
            get => _lugar;
            set
            {
                _lugar = value;
                OnPropertyChanged();
            }
        }

        public string CodeBoxEntry
        {
            get => _codeBox;
            set
            {
                _codeBox = value;
                OnPropertyChanged();
            }
        }

        public string QtyEntry
        {
            get => _qty;
            set
            {
                _qty = value;
                OnPropertyChanged();
            }
        }

        public string UbicacionEntry
        {
            get => _ubicacion;
            set
            {
                _ubicacion = value;
                OnPropertyChanged();
            }
        }

        public string ObservacionEntry
        {
            get => _observacion;
            set
            {
                _observacion = value;
                OnPropertyChanged();
            }
        }
  
        public string EstadoEntry
        {
            get => _estado;
            set
            {
                _estado = value;
                OnPropertyChanged();
            }
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
        
        public string TasksLabel
        {
            get => taskslabel;
            set => SetProperty(ref taskslabel, value);
        }

        public string Comentary
        {
            get => comentary;
            set => SetProperty(ref comentary, value);
        }

        public string ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;

            }
        }


        public byte[] ArrayFotoX;  //Almacenar Foto
        public byte[] ArrayFoto;  //Almacenar Foto
        public byte[] ArrayFoto1;  //Almacenar Foto
        public byte[] ArrayFoto2;  //Almacenar Foto
        public byte[] ArrayFoto3;  //Almacenar Foto

        private ImageSource _imageSourceX;
        private ImageSource _imageSource;
        private ImageSource _imageSource1;
        private ImageSource _imageSource2;
        private ImageSource _imageSource3;
        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set
            {
                _imageSource = value;
                SetProperty(ref _imageSource, value);
            }
        }
        public ImageSource ImageSource1
        {
            get { return _imageSource1; }
            set
            {
                _imageSource1 = value;
                SetProperty(ref _imageSource1, value);
            }
        }
        public ImageSource ImageSource2
        {
            get { return _imageSource2; }
            set
            {
                _imageSource2 = value;
                SetProperty(ref _imageSource2, value);
            }
        }
        public ImageSource ImageSource3
        {
            get { return _imageSource3; }
            set
            {
                _imageSource3 = value;
                SetProperty(ref _imageSource3, value);
            }
        }
        #endregion
        
        public VBSShipmentDetailsNewViewModel()
        {
            Title = "New Shipment";
            var ids = App.Current.Properties["UserId"];
            iduser = ids.ToString();

            _imageSource = ImageSource.FromFile("icon.png");
            _imageSource1 = ImageSource.FromFile("icon.png");
            _imageSource2 = ImageSource.FromFile("icon.png");
            _imageSource3 = ImageSource.FromFile("icon.png");
            OnPropertyChanged(nameof(ImageSource));
            OnPropertyChanged(nameof(ImageSource1));
            OnPropertyChanged(nameof(ImageSource2));
            OnPropertyChanged(nameof(ImageSource3));

            TapCommand = new Command(OnTapped);
            ReportAlertCommand = new Command(OnReportAlertClicked);
            SaveChangesCommand = new Command(async (o) => await OnSaveClicked(o));
            CancelCommand = new Command(OnCancelClicked);
            ItemId = SecureStorage.GetAsync("WorkId").Result;
            GetRecepcion(ItemId);
        }
        private async void GetRecepcion(string idSeleccioando)
        {
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
                    var ids = App.Current.Properties["Username"];
                    string userName = ids.ToString();
                    //MyAgenda = await datos.GetRecepcionAisv(long.Parse(idSeleccioando), userName).ConfigureAwait(true);
                    //OnPropertyChanged(nameof(MyAgenda));

                    LoadItemId(idSeleccioando);
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
        public async void LoadItemId(string idCabecera)
        {
            try
            {
                await CargaDetalle(long.Parse(idCabecera));
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private async Task CargaDetalle(long idCabecera)
        {
            if (idCabecera > 0)
            {
                ///////////////////////////////////
                //   llenar combo generales
                ///////////////////////////////////
                DatosApi datos = new DatosApi();

                var lstHold = await datos.GetListaHoldVBS();
                ListHold = lstHold;
                SelectedHold = ListHold?.FirstOrDefault();
                OnPropertyChanged(nameof(ListHold));

                var lstDeck = await datos.GetListaDecksVBS();
                ListDeck = lstDeck;
                SelectedDeck = ListDeck?.FirstOrDefault();
                OnPropertyChanged(nameof(ListDeck));

                var lstTipoMovimiento = await datos.GetListaTipoMovimientosVBS();
                ListEvents = lstTipoMovimiento;
                SelectedEvents = ListEvents?.FirstOrDefault();
                OnPropertyChanged(nameof(ListEvents));

                var lstOrigen = await datos.GetListaOrigenVBS();
                ListOrigen = lstOrigen;
                SelectedOrigen= ListOrigen?.FirstOrDefault();
                OnPropertyChanged(nameof(ListOrigen));

                var dd = await datos.GetListaModalidadEmbarque();
                ListModalidad = dd;
                SelectedModalidad = ListModalidad?.FirstOrDefault();
                OnPropertyChanged(nameof(ListModalidad));



                ///////////////////////////////////
                //   carga datos en pantalla
                ///////////////////////////////////
                var oCabecera = await datos.GetEmbarqueCab(idCabecera);

                _selectedItem = oCabecera;
                //int vSaldo = (int)oDet.box; //- arrastre;
                //vSaldo = vSaldo - arrastre;

                if (oCabecera?.idEmbarqueCab is null)
                {
                    container = "CARGA EN PRE EMBARQUE";
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

                    //await Shell.Current.GoToAsync("////VBSPreDispatchInboxPage");
                    return;
                }
                else
                {
                    container = "Order Number : " + oCabecera.idEmbarqueCab?.ToString("D8");
                    comentary = "RUC Exporter: " + oCabecera.idExportador;
                    taskslabel = "Exporter: " + oCabecera.Exportador;
                    temperature = "Reference: " + oCabecera.idNave;
                    types = "Ship name: " + oCabecera.nave;
                    idWork = ItemId;

                    var lstBrands = await datos.GetListaBrandsVBS(oCabecera.idExportador);
                    ListBrand = lstBrands;
                    SelectedBrand = ListBrand?.FirstOrDefault();
                    OnPropertyChanged(nameof(ListBrand));
                }
            }
            else
            {
                container = "CARGA EN PRE EMBARQUE";
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
                if (Accion != 3 && file != null)
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
                }
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
        private async Task OnSaveClicked(object obj)
        {
            try
            {
                var _sUser = App.Current.Properties["SuperUser"];
                bool superUser = bool.Parse(_sUser.ToString());

                var ids = App.Current.Properties["Username"];
                string userName = ids.ToString();

                bool DeACuerdo;
                bool flags = false;
                DeACuerdo = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Desea registrar?", "De Acuerdo", "Cancelar");
                IsBusy = true;
                if (DeACuerdo)
                {
                    if (string.IsNullOrEmpty(CodeBoxEntry?.ToString()))
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please enter Box Code", "Close");
                        flags = true;
                    }

                    if (string.IsNullOrEmpty(SelectedBrand?.Valor))
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please enter Brands", "Close");
                        flags = true;
                    }

                    if (QtyEntry == null || QtyEntry == "" || QtyEntry == "0")
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please enter QTY", "Close");
                        flags = true;
                    }

                    string v_tipoMov = "1";
                    string v_tipo = "ING";
                    if (!string.IsNullOrEmpty(SelectedEvents?.Valor))
                    {
                        v_tipoMov = SelectedEvents?.Valor;
                        v_tipo = "EGR";
                        if (string.IsNullOrEmpty(ObservacionEntry))
                        {
                            await App.Current.MainPage.DisplayAlert("Error", "Please enter Note", "Close");
                            flags = true;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(SelectedHold?.Valor))
                        {
                            await App.Current.MainPage.DisplayAlert("Error", "Please enter Hold", "Close");
                            flags = true;
                        }
                        if (string.IsNullOrEmpty(SelectedDeck?.Valor))
                        {
                            await App.Current.MainPage.DisplayAlert("Error", "Please enter Deck", "Close");
                            flags = true;
                        }
                        if (string.IsNullOrEmpty(SelectedModalidad?.Valor))
                        {
                            await App.Current.MainPage.DisplayAlert("Error", "Please enter Modality", "Close");
                            flags = true;
                        }
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
                            UserDialogs.Instance.ShowLoading("Sending...");
                            DatosApi datos = new DatosApi();

                            double tmp = Convert.ToDouble(QtyEntry);
                            QtyEntry = tmp.ToString("0.##");

                            BAN_Embarque_Movimiento oRecepcion = new BAN_Embarque_Movimiento();
                            oRecepcion.idEmbarqueMovimiento = 0;
                            oRecepcion.idEmbarqueCab = long.Parse(_selectedItem.idEmbarqueCab.ToString());
                            oRecepcion.codigoCaja = CodeBoxEntry?.ToString();
                            oRecepcion.idHold = int.Parse(string.IsNullOrEmpty(SelectedHold?.Valor)? "0": SelectedHold?.Valor);
                            oRecepcion.idPiso = int.Parse(string.IsNullOrEmpty(SelectedDeck?.Valor) ? "0" : SelectedDeck?.Valor); 
                            oRecepcion.idMarca = int.Parse(string.IsNullOrEmpty(SelectedBrand?.Valor) ? "0" : SelectedBrand?.Valor);
                            oRecepcion.idModalidad = int.Parse(string.IsNullOrEmpty(SelectedModalidad?.Valor) ? "0" : SelectedModalidad?.Valor);
                            oRecepcion.idtipoMovimiento = int.Parse(v_tipoMov);
                            oRecepcion.box = int.Parse(QtyEntry.ToString());
                            oRecepcion.tipo = v_tipo;
                            oRecepcion.estado = "NUE";
                            oRecepcion.comentario = ObservacionEntry?.ToString();
                            oRecepcion.usuarioCrea = userName;

                            ApiModels.AppModels.Base msg = await datos.RegistraMovmientoEmbarqueVBS(oRecepcion, ArrayFoto, ArrayFoto1, ArrayFoto2, ArrayFoto3).ConfigureAwait(true);
                            await App.Current.MainPage.DisplayAlert("Respuesta", msg.messages, "Close");
                            if (msg.response == true)
                            {
                                IsBusy = false;
                                await Shell.Current.GoToAsync("////VBSShipmentInboxPage/VBSShipmentDetailsPage");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Close");
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
            if (action == "Delete Photo")
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
            }
        }
        private async void OnReportAlertClicked(object sender)
        {
            if (_selectedItem == null)
                return;

            await Shell.Current.GoToAsync("/TaskDetails/AlertReportPage");
            //await Shell.Current.Navigation.PushAsync(new AlertReportPage());

        }
        private async void OnCancelClicked()
        {
            await Shell.Current.GoToAsync("////VBSShipmentInboxPage/VBSShipmentDetailsPage");
        }
    }
}
