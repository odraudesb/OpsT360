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
    public class VBSOrderDispatchNewOrderViewModel : BaseViewModel
    {
        #region Declaraciones

        public string idWork;
        private string itemId;
        private BAN_Stowage_Movimiento _selectedItem;

        public string _lugar;
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
        private Combo _selectedNov;

        public BAN_Stowage_Movimiento MyAgenda { get; private set; }
        public Command ReportAlertCommand { get; }
        public Command SaveChangesCommand { get; }
        public Command CancelCommand { get; }
        public Command TapCommand { get; }
        public List<TasksBD> Listregistrado { get; private set; }

        public string Comentary
        {
            get => comentary;
            set => SetProperty(ref comentary, value);
        }
        public string LugarEntry
        {
            get => _lugar;
            set
            {
                _lugar = value;
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

        public Combo SelectedNov
        {
            get { return _selectedNov; }
            set { _selectedNov = value; OnPropertyChanged(); }
        }

        public List<Combo> ListModalidad { get; private set; }


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
        
        public VBSOrderDispatchNewOrderViewModel()
        {
            Title = "New Order Dispatch";
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
                    var v_data = ItemId.Split(',');
                    string v_idNave = v_data[0];
                    string v_idExportador = v_data[1];
                    string v_idBloque = v_data[2];
                    string v_booking = v_data[3];

                    //OBTIENE DATA DE ORDENES
                    BAN_Stowage_Movimiento oDet;
                    int _arrastre;

                    var oLista = await datos.GetListaOrdenes(v_idNave, int.Parse(v_idExportador), int.Parse(v_idBloque), v_booking);
                    oDet = datos.oDetMovimiento;
                    _arrastre = datos.arrastre;

                    //############################
                    MyAgenda = await datos.GetMovimientoParaDespacho(v_idNave, v_idExportador, v_idBloque, v_booking).ConfigureAwait(true);
                    OnPropertyChanged(nameof(MyAgenda));

                    LoadItemId(MyAgenda, _arrastre);
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
        public async void LoadItemId(BAN_Stowage_Movimiento oRecepcion, int _arrastre)
        {
            try
            {
                await CargaDetalle(oRecepcion, _arrastre);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private async Task CargaDetalle(BAN_Stowage_Movimiento oRec, int _arrastre)
        {
            if (oRec != null)
            {
                ///////////////////////////////////
                //   carga datos en pantalla
                ///////////////////////////////////
                _selectedItem = oRec;
                int _total = oRec.palets;
                oRec.palets = oRec.palets - _arrastre;

                container = "Exporter: " + oRec.oExportador?.nombre;
                taskslabel = "Location: " + oRec.oBloque?.oBodega?.nombre;
                temperature = "Block: " + oRec.oBloque?.nombre;
                //types = "Pending Pallets: [" + oRec.palets + "]    Pending Boxes: [" + oRec.cantidad + "]";
                types = string.Format("Total Pallets: [{0}]   Drag: [{1}]   Pending: [{2}]", _total,  _arrastre, oRec.palets.ToString());
                comentary = "Booking: " + oRec.booking;
                idWork = ItemId;
            }
            else
            {
                container = "BL: MRN-MSN-HSN";
                temperature = "Cantidad = 0";
                types = "Arrastre: 0";
                taskslabel = "Cliente: XXXXXXXXXXX XXXXXXXXX XXXXXXXXXXX";
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
                    if (int.Parse(QtyEntry) > MyAgenda.palets)
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Check QTY Pallets", "Close");
                        flags = true;
                    }
                    if (QtyEntry == null || QtyEntry == "" || QtyEntry == "0")
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please enter QTY", "Close");
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
                            UserDialogs.Instance.ShowLoading("Sending...");
                            DatosApi datos = new DatosApi();

                            double tmp = Convert.ToDouble(QtyEntry);
                            QtyEntry = tmp.ToString("0.##");

                            BAN_Stowage_OrdenDespacho oOrdenDespacho = new BAN_Stowage_OrdenDespacho();
                            oOrdenDespacho.idOrdenDespacho = null;
                            oOrdenDespacho.idNave = MyAgenda.idNave;
                            oOrdenDespacho.idExportador = MyAgenda.idExportador;
                            oOrdenDespacho.idBodega = int.Parse(MyAgenda.oBloque.idBodega.ToString());
                            oOrdenDespacho.idBloque = int.Parse(MyAgenda.oBloque.id.ToString());
                            oOrdenDespacho.cantidadPalets = int.Parse(QtyEntry.ToString());
                            oOrdenDespacho.cantidadBox = 1;
                            oOrdenDespacho.arrastre = 0;
                            oOrdenDespacho.pendiente = int.Parse(QtyEntry.ToString());
                            oOrdenDespacho.estado = "NUE";
                            oOrdenDespacho.usuarioCrea = userName;
                            oOrdenDespacho.booking = MyAgenda.booking;

                            ApiModels.AppModels.Base msg = await datos.RegistraOrdenDespacho(oOrdenDespacho).ConfigureAwait(true);
                            await App.Current.MainPage.DisplayAlert("Respuesta", msg.messages, "Close");
                            if (msg.response == true)
                            {
                                IsBusy = false;
                                await Shell.Current.GoToAsync("////VBSOrderDispatchInboxPage/VBSOrderDispatchDetailsPage");
                            }
                        }
                    }
                }
                IsBusy = false;
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
        
        private async void OnReportAlertClicked(object sender)
        {
            if (_selectedItem == null)
                return;

            await Shell.Current.GoToAsync("/TaskDetails/AlertReportPage");
            //await Shell.Current.Navigation.PushAsync(new AlertReportPage());

        }
        private async void OnCancelClicked()
        {
            await Shell.Current.GoToAsync("////VBSOrderDispatchInboxPage/VBSOrderDispatchDetailsPage");
        }
    }
}
