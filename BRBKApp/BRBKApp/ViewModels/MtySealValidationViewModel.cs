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
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace BRBKApp.ViewModels
{
    public class MtySealValidationViewModel : BaseViewModel
    {
        #region Declaraciones

        bool isRefreshing;
        const int RefreshDuration = 2;
        public bool _esActivo;
        private ImageSource _btnIcon;
        private workPosition _selectedItem;
        private string txtNumContainer = null;
        private string _txtSeal;
        private string txtPosition = null;
        private string _usuario = null;
        private string _referece = null;
        private string _crane = null;
        private string _dataContainers = null;

        public long gkey{ get; set; }
        public bool bloqueo { get; set; }
        public bool impedimento { get; set; }
        public string impedimentod { get; set; }

        public ImageSource BtnIcon
        {
            get { return _btnIcon; }
            set
            {
                _btnIcon = value;
                SetProperty(ref _btnIcon, value);
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
        public string TxtNumContainer
        {
            get => txtNumContainer;
            set
            {
                txtNumContainer = value;
                OnPropertyChanged();
            }
        }
        public string TxtSeal
        {
            get => _txtSeal;
            set
            {
                _txtSeal = value;
                OnPropertyChanged();
            }
        }
        public string TxtPosition
        {
            get => txtPosition;
            set
            {
                txtPosition = value;
                OnPropertyChanged();
            }
        }

        public string Usuario
        {
            get => _usuario;
            set => SetProperty(ref _usuario, value);
        }
        public string Reference
        {
            get => _referece;
            set => SetProperty(ref _referece, value);
        }
        public string Crane
        {
            get => _crane;
            set => SetProperty(ref _crane, value);
        }
        public string DataContainers
        {
            get => _dataContainers;
            set => SetProperty(ref _dataContainers, value);
        }

        public Command ReportAlertCommand { get; }
        public Command ConsultCommand { get; }
        public Command SaveChangesCommand { get; }
        public Command CancelCommand { get; }
        public Command TapCommand { get; }
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }
        public async void LoadItemId(ObservableCollection<workPosition> oDet)
        {
            try
            {
                await CargaDetalle(oDet);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public ObservableCollection<workPosition> MyAgenda { get; private set; }
        public List<TasksBD> Listregistrado { get; private set; }

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

        public ICommand RefreshCommand => new Command(async () => await RefreshItemsAsync());
        public MtySealValidationViewModel()
        {
            Title = "Seal Validation";
             _btnIcon = ImageSource.FromFile("icon_search.png");
            
            _imageSource = ImageSource.FromFile("icon.png");
            _imageSource1 = ImageSource.FromFile("icon.png");
            _imageSource2 = ImageSource.FromFile("icon.png");
            _imageSource3 = ImageSource.FromFile("icon.png");
            OnPropertyChanged(nameof(ImageSource));
            OnPropertyChanged(nameof(ImageSource1));
            OnPropertyChanged(nameof(ImageSource2));
            OnPropertyChanged(nameof(ImageSource3));
            OnPropertyChanged(nameof(BtnIcon));

            TapCommand = new Command(OnTapped);
            SaveChangesCommand = new Command(async (o) => await OnSaveClicked(o));
            CancelCommand = new Command(OnCancelClicked);
            ConsultCommand = new Command(OnConsultClicked);
            esActivo = false;
        }
       
        public void CleanPages()
        {
            IsBusy = false;
            TxtNumContainer = "";
            TxtSeal = "";
            txtPosition = "";
            esActivo = false;
            gkey = -1;
            bloqueo = false;
            impedimento = false;
            impedimentod = string.Empty;
            _dataContainers = string.Empty;

            OnPropertyChanged(nameof(TxtNumContainer));
            OnPropertyChanged(nameof(TxtSeal));
            OnPropertyChanged(nameof(TxtPosition));
            OnPropertyChanged(nameof(esActivo));
            OnPropertyChanged(nameof(DataContainers));

            _imageSource = ImageSource.FromFile("icon.png");
            _imageSource1 = ImageSource.FromFile("icon.png");
            _imageSource2 = ImageSource.FromFile("icon.png");
            _imageSource3 = ImageSource.FromFile("icon.png");
            OnPropertyChanged(nameof(ImageSource));
            OnPropertyChanged(nameof(ImageSource1));
            OnPropertyChanged(nameof(ImageSource2));
            OnPropertyChanged(nameof(ImageSource3));

            ArrayFoto = null;
            ArrayFoto1 = null;
            ArrayFoto2 = null;
            ArrayFoto3 = null;
        }

        private async void OnConsultClicked(object obj)
        {
            esActivo = false;
            OnPropertyChanged(nameof(esActivo));
            IsRefreshing = true;
            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
           // await GetAgenda(TxtMRN);
            IsRefreshing = false;
            //esActivo = true; ;
        }
        async Task RefreshItemsAsync()
        {
            IsRefreshing = true;

            var ids = App.Current.Properties["Username"];
            string userName = ids.ToString();

            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
            await GetAgenda(null,userName);
            IsRefreshing = false;
        }

        private async Task GetAgenda(string idPosition, string _user)
        {
            if (string.IsNullOrEmpty(_user)) { return; }
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
                  

                    MyAgenda = await datos.GetPOW(idPosition, _user, txtNumContainer);
                    //OnPropertyChanged(nameof(MyAgenda));
                    LoadItemId(MyAgenda);
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
        private async Task CargaDetalle(ObservableCollection<workPosition> oDet)
        {
            if (esActivo) { return; }

            if (oDet != null)
            {
                esActivo = false;
                _selectedItem = oDet.FirstOrDefault();

                if (string.IsNullOrEmpty(_selectedItem.namePosition))
                {
                    _usuario = "<< NEED TO CONFIGURE THE POW >>"; 
                    _referece = "YOU MUST GO TO THE OPTIONS MENU REGISTER POW";
                    _crane = "PLEASE CHECK POW";
                }
                else
                {
                    _usuario = "USER: " + _selectedItem.usuarioCrea.ToUpper();
                    _referece = "VISIT: " + _selectedItem.idPosition + "  EXPIRED: " + _selectedItem.validoHasta.ToString();
                    _crane = "POW: " + _selectedItem.namePosition;
                }

                if (!string.IsNullOrEmpty(txtNumContainer) && _selectedItem.dataContenedor != null)
                {
                    //_dataContainers = string.Format("[PORT:{0} | WEIGHT:{1} | TYPE:{2} | °C:{3} | IMO:{4} | HOLD:{5}]", _selectedItem.dataContenedor.puerto1, _selectedItem.dataContenedor.peso, _selectedItem.dataContenedor.tipo, _selectedItem.dataContenedor.temperatura, _selectedItem.dataContenedor.imo, _selectedItem.dataContenedor.impedimentod);
                    _dataContainers = _selectedItem.dataContenedor.notas;

                    if (_selectedItem.dataContenedor.impedimento)
                    {
                        await App.Current.MainPage.DisplayAlert("Validation", "Impediments: " + _selectedItem.dataContenedor.impedimentod, "OK");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Validation", "Successful", "OK");
                        esActivo = true;

                        gkey= _selectedItem.dataContenedor.gkey;
                        bloqueo = _selectedItem.dataContenedor.bloqueo;
                        impedimento = _selectedItem.dataContenedor.impedimento;
                        impedimentod = _selectedItem.dataContenedor.impedimentod;
                    }
                }
                else
                {
                    esActivo = false;
                    if (!string.IsNullOrEmpty(txtNumContainer))
                    {
                        await App.Current.MainPage.DisplayAlert("Validation", "Unit not found :" + TxtNumContainer, "OK");
                    }
                }
            }
            else
            {
                _usuario = "need to configure the pow";
                //_referece = "Reference: XXXXX";
                //_crane = "Crane: XXXXX";
            }
            OnPropertyChanged(nameof(Usuario));
            OnPropertyChanged(nameof(Reference));
            OnPropertyChanged(nameof(Crane));
            OnPropertyChanged(nameof(DataContainers));
            OnPropertyChanged(nameof(esActivo));
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

                if (string.IsNullOrEmpty(_selectedItem.namePosition) && !string.IsNullOrEmpty(txtPosition))
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Please set the POW", "Close");
                    flags = true;
                    return;
                }

                DeACuerdo = await App.Current.MainPage.DisplayAlert("Confirm", "¿Are you sure?", "Yes", "Cancel");

                var ids = App.Current.Properties["Username"];
                string userName = ids.ToString();

                if (DeACuerdo)
                {
                    if (string.IsNullOrEmpty(TxtNumContainer))
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please enter Containers", "Close");
                        flags = true;
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtSeal))
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please enter Seal", "Close");
                        flags = true;
                        return;
                    }

                    //if (string.IsNullOrEmpty(TxtPosition))
                    //{
                    //    await App.Current.MainPage.DisplayAlert("Error", "Please enter Positions", "Close");
                    //    flags = true;
                    //    return;
                    //}

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
                            var idUser = App.Current.Properties["UserId"];
                            int id = Convert.ToInt32(idUser);

                            ApiModels.AppModels.Base msg = await datos.GetValidateSeal(TxtNumContainer.ToUpper(),TxtSeal.ToUpper(), txtPosition,gkey,bloqueo,impedimento,impedimentod, userName.ToLower(), ArrayFoto, ArrayFoto1, ArrayFoto2, ArrayFoto3).ConfigureAwait(true);
                            await App.Current.MainPage.DisplayAlert("Respuesta", msg.messages, "Close");
                            if (msg.response == true)
                            {
                                CleanPages();
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
