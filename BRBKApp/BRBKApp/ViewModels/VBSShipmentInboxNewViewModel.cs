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
    public class VBSShipmentInboxNewViewModel : BaseViewModel
    {
//        public string idWork;
        private string itemId;
        private BAN_Stowage_Plan_Aisv _selectedItem;
        private string _barcode;
        public string iduser;
        private Combo _selectedNave;
        private Combo _selectedExportador;
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

        #region Propiedades
        public BAN_Stowage_Movimiento MyAgenda { get; private set; }
        public Command ReportAlertCommand { get; }
        public Command SaveChangesCommand { get; }
        public Command CancelCommand { get; }
        public Command TapCommand { get; }
        public List<TasksBD> Listregistrado { get; private set; }

        public string BarcodeEntry
        {
            get => _barcode;
            set
            {
                _barcode = value;
                OnPropertyChanged();
            }
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
                if (_selectedExportador != value)
                {
                    _selectedExportador = value;
                    OnPropertyChanged();
                }
            }
        }

        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set {_imageSource = value; SetProperty(ref _imageSource, value);}
        }
        public ImageSource ImageSource1
        {
            get { return _imageSource1; }
            set { _imageSource1 = value; SetProperty(ref _imageSource1, value); }
        }
        public ImageSource ImageSource2
        {
            get { return _imageSource2; }
            set { _imageSource2 = value; SetProperty(ref _imageSource2, value); }
        }
        public ImageSource ImageSource3
        {
            get { return _imageSource3; }
            set { _imageSource3 = value; SetProperty(ref _imageSource3, value); }
        }
        #endregion
        
        public VBSShipmentInboxNewViewModel()
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
        public async void LoadItemId(string idSeleccioando)
        {
            try
            {
                await CargaDetalle(idSeleccioando);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private async Task CargaDetalle(string idSeleccioando)
        {
            if (idSeleccioando != null)
            {
                var v_data = idSeleccioando.ToString().Split('-');
                ///////////////////////////////////
                //   llenar combo de naves y export
                ///////////////////////////////////
                DatosApi datos = new DatosApi();
                var dd = await datos.GetListaNavesVBS();
                ListNave = dd.Where(p => p.Valor == v_data[0].ToString()).ToList();

                var lstExpo = await datos.GetListaExportadoresVBS();
                ListExportador = lstExpo.Where(p=> p.Valor == v_data[1].ToString()).ToList();

                OnPropertyChanged(nameof(ListNave));
                OnPropertyChanged(nameof(ListExportador));

                SelectedNave = ListNave?.FirstOrDefault();
                SelectedExportador = ListExportador?.FirstOrDefault();

                
                ///////////////////////////////////
                //   carga datos en pantalla
                ///////////////////////////////////
                //_selectedItem = oRec.oStowage_Plan_Aisv;

                //container = "AISV: " + _selectedItem.aisv;
                //taskslabel = "Exporter: " + _selectedItem.oStowage_Plan_Det?.oExportador?.nombre;
                //temperature = "Commodity: " + _selectedItem.oStowage_Plan_Det?.oCargo?.nombre + " " + _selectedItem.oStowage_Plan_Det?.oMarca?.nombre;
                //types = "Qty: " + _selectedItem.box + "     Drag: " + _selectedItem.arrastre.ToString() + "    Pending Qty: " + _selectedItem.pendiente.ToString();
                //comentary = "Planned Location: " + string.Format("{0} | Bloque: {1}", _selectedItem.oStowage_Plan_Det?.oBodega?.nombre, _selectedItem.oStowage_Plan_Det?.oBloque?.nombre);
                //idWork = oRec.idMovimiento.ToString();

                //OnPropertyChanged(nameof(Container));
                //OnPropertyChanged(nameof(Temperature));
                //OnPropertyChanged(nameof(Types));
                //OnPropertyChanged(nameof(TasksLabel));
            }
           
            //OnPropertyChanged(nameof(Container));
            //OnPropertyChanged(nameof(Temperature));
            //OnPropertyChanged(nameof(Types));
            //OnPropertyChanged(nameof(TasksLabel));

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
                    if (string.IsNullOrEmpty(SelectedNave?.Valor))
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please enter nave", "Close");
                        flags = true;
                    }
                    if (string.IsNullOrEmpty(_selectedExportador?.Valor))
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please enter exporter", "Close");
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

                            BAN_Embarque_Cab oEntidad = new BAN_Embarque_Cab();
                            oEntidad.idEmbarqueCab = 0;
                            oEntidad.barcode = BarcodeEntry?.ToString();
                            oEntidad.idNave = SelectedNave.Valor;
                            oEntidad.nave = SelectedNave.Descripcion;
                            oEntidad.idExportador = SelectedExportador.Valor;
                            oEntidad.Exportador = SelectedExportador.Descripcion;
                            oEntidad.estado = "NUE";
                            oEntidad.usuarioCrea = userName;

                            ApiModels.AppModels.Base msg = await datos.RegistraEmbarqueInbox(oEntidad, ArrayFoto, ArrayFoto1, ArrayFoto2, ArrayFoto3).ConfigureAwait(true);
                            await App.Current.MainPage.DisplayAlert("Respuesta", msg.messages, "Close");
                            if (msg.response == true)
                            {
                                IsBusy = false;
                                await Shell.Current.GoToAsync("////VBSShipmentInboxPage");
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
            await Shell.Current.GoToAsync("////VBSShipmentInboxPage");
        }
    }
}
