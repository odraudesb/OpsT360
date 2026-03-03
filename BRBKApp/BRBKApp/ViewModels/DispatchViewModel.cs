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
    public class DispatchViewModel : BaseViewModel
    {
        #region Declaraciones
        bool isRefreshing;
        public bool _esActivo;
        const int RefreshDuration = 2;
        public string idWork;
        public string _noteEntry;
        private tarjaDet _selectedItem;
        private ImageSource _btnIcon;
        private string _qty;
        private string container;
        private string temperature;
        private string types;
        private string taskslabel;
        public string iduser;
        public string comentary;
        private string txtPasePuertaNum = null;
        
        public Command ConsultCommand { get; }
        public ImageSource BtnIcon
        {
            get { return _btnIcon; }
            set
            {
                _btnIcon = value;
                SetProperty(ref _btnIcon, value);
            }
        }
        public string TxtNumPasePuerta
        {
            get => txtPasePuertaNum;
            set
            {
                txtPasePuertaNum = value;
                OnPropertyChanged();
            }
        }
        public pasePuerta MyAgenda { get; private set; }
        public Command ReportAlertCommand { get; }
        public Command SaveChangesCommand { get; }
        public Command CancelCommand { get; }
        public Command TapCommand { get; }
        public List<TasksBD> Listregistrado { get; private set; }

        public string NoteEntry
        {
            get => _noteEntry;
            set
            {
                _noteEntry = value;
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
        public DispatchViewModel()
        {
            Title = "Dispatch";
             _btnIcon = ImageSource.FromFile("icon_search.png");

            OnPropertyChanged(nameof(BtnIcon));

            _imageSource = ImageSource.FromFile("icon.png");
            _imageSource1 = ImageSource.FromFile("icon.png");
            _imageSource2 = ImageSource.FromFile("icon.png");
            _imageSource3 = ImageSource.FromFile("icon.png");
            OnPropertyChanged(nameof(ImageSource));
            OnPropertyChanged(nameof(ImageSource1));
            OnPropertyChanged(nameof(ImageSource2));
            OnPropertyChanged(nameof(ImageSource3));

            ConsultCommand = new Command(OnConsultClicked);
            TapCommand = new Command(OnTapped);
            SaveChangesCommand = new Command(async (o) => await OnSaveClicked(o));
            CancelCommand = new Command(OnCancelClicked);
            esActivo = true;
        }
        private async void OnConsultClicked(object obj)
        {
            esActivo = false;
            IsRefreshing = true;
            var vPase = TxtNumPasePuerta;
            CleanPages();
            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
            await GetAgenda(vPase);
            IsRefreshing = false;
            esActivo = true;
        }
        private async Task GetAgenda(string _numPase)
        {
            if (string.IsNullOrEmpty(_numPase)) { return; }
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
                    MyAgenda = await datos.GetEPass(_numPase);
                    OnPropertyChanged(nameof(MyAgenda));
                    if (MyAgenda.idTarjaDet != null)
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
        public async void LoadItemId(pasePuerta oPasePuerta)
        {
            try
            {
                await CargaDetalle(oPasePuerta);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private async Task CargaDetalle(pasePuerta oPass)
        {
            if (oPass.idTarjaDet != null)
            {
                ///////////////////////////////////
                //   carga datos en pantalla
                ///////////////////////////////////
                TxtNumPasePuerta = oPass.pase;
                _selectedItem = oPass.tarjaDet;
                container = "BL: " + _selectedItem.carga + " EPass: " + oPass.pase;
                temperature = "Truck licence: " + oPass.placa;
                types = "Driver: " + oPass.idchofer + " - " + oPass.chofer;
                taskslabel = "Cliente: " + _selectedItem.Consignatario;
                comentary = "";
                idWork = oPass.pase.ToString();
                QtyEntry = oPass.cantidad.ToString();
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
            OnPropertyChanged(nameof(TxtNumPasePuerta));
            OnPropertyChanged(nameof(QtyEntry));
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
            TxtNumPasePuerta = "";
            QtyEntry = "0";
            NoteEntry = "";
            ArrayFoto = null;
            ArrayFoto1 = null;
            ArrayFoto2 = null;
            ArrayFoto3 = null;
            ImageSource = null;
            ImageSource1 = null;
            ImageSource2 = null;
            ImageSource3 = null;
            _imageSource = ImageSource.FromFile("icon.png");
            _imageSource1 = ImageSource.FromFile("icon.png");
            _imageSource2 = ImageSource.FromFile("icon.png");
            _imageSource3 = ImageSource.FromFile("icon.png");
            OnPropertyChanged(nameof(ImageSource));
            OnPropertyChanged(nameof(ImageSource1));
            OnPropertyChanged(nameof(ImageSource2));
            OnPropertyChanged(nameof(ImageSource3));
            OnPropertyChanged(nameof(Container));
            OnPropertyChanged(nameof(Temperature));
            OnPropertyChanged(nameof(Types));
            OnPropertyChanged(nameof(TasksLabel));
            OnPropertyChanged(nameof(TxtNumPasePuerta));
            OnPropertyChanged(nameof(QtyEntry));
            OnPropertyChanged(nameof(NoteEntry));
        }
        private async void OnCancelClicked(object obj)
        {
            CleanPages();
        }

        private async Task OnSaveClicked(object obj)
        {
            try
            {
                bool DeACuerdo;
                bool flags = false;
                DeACuerdo = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Desea registrar?", "De Acuerdo", "Cancelar");

                var ids = App.Current.Properties["Username"];
                string userName = ids.ToString();

                if (DeACuerdo)
                {
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
                            if (MyAgenda != null)
                            {
                                UserDialogs.Instance.ShowLoading("Sending...");
                                DatosApi datos = new DatosApi();

                                despacho oDespacho = new despacho();
                                oDespacho.idTarjaDet = long.Parse(MyAgenda.idTarjaDet.ToString());
                                oDespacho.pase = idWork;
                                oDespacho.mrn = MyAgenda.mrn;
                                oDespacho.msn = MyAgenda.msn;
                                oDespacho.hsn = MyAgenda.hsn;
                                oDespacho.placa = MyAgenda.placa;
                                oDespacho.idchofer = MyAgenda.idchofer;
                                oDespacho.chofer = MyAgenda.chofer;
                                oDespacho.cantidad = decimal.Parse(QtyEntry.ToString());
                                oDespacho.observacion = NoteEntry.ToString();
                                oDespacho.estado = "NUE";
                                oDespacho.usuarioCrea = userName;
                                oDespacho.delivery = MyAgenda.delivery;
                                oDespacho.PRE_GATE_ID = MyAgenda.PRE_GATE_ID;

                                ApiModels.AppModels.Base msg = await datos.RegistraDespacho(oDespacho, ArrayFoto, ArrayFoto1, ArrayFoto2, ArrayFoto3).ConfigureAwait(true);
                                await App.Current.MainPage.DisplayAlert("Respuesta", msg.messages, "Close");
                                if (msg.response == true)
                                {
                                    CleanPages();
                                }
                            }
                            else
                            {
                                if (txtPasePuertaNum == "")
                                {
                                    await App.Current.MainPage.DisplayAlert("Respuesta", "Ingrese el número del pase de puerta, luego presione el botón de consulta", "Close");
                                }
                                else
                                {
                                    OnConsultClicked(null);
                                    await App.Current.MainPage.DisplayAlert("Respuesta", "La información del pase de puerta se ha refrescado, Intente nuevamente por favor", "Close");
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
    }
}
