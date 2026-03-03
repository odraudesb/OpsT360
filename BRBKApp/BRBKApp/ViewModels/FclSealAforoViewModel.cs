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
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BRBKApp.ViewModels
{
    public class FclSealAforoViewModel : BaseViewModel
    {
        #region Declaraciones
        bool isRefreshing;
        const int RefreshDuration = 2;
        private ImageSource _btnIcon;
        private dataContainers _selectedItem;
        public bool _esActivo;
        private string txtNumContainer = null;
        private string _txtSealCGSA;
        private string _txtSeal1;
        private string _txtSeal2;
        private string _txtSeal3;
        //private string _txtSeal4;
        //private string txtPosition = null;
        //private string _dataContainers = null;

        public long gkey { get; set; }
        public string referencia { get; set; }
        public ImageSource BtnIcon
        {
            get { return _btnIcon; }
            set
            {
                _btnIcon = value;
                SetProperty(ref _btnIcon, value);
            }
        }
        public Command ConsultCommand { get; }
        public Command SaveChangesCommand { get; }
        public Command CancelCommand { get; }
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }
        public Command TapCommand { get; }
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
        public string TxtSealCGSA
        {
            get => _txtSealCGSA;
            set
            {
                _txtSealCGSA = value;
                OnPropertyChanged();
            }
        }

        public string TxtSeal1
        {
            get => _txtSeal1;
            set
            {
                _txtSeal1 = value;
                OnPropertyChanged();
            }
        }

        public string TxtSeal2
        {
            get => _txtSeal2;
            set
            {
                _txtSeal2 = value;
                OnPropertyChanged();
            }
        }

        public string TxtSeal3
        {
            get => _txtSeal3;
            set
            {
                _txtSeal3 = value;
                OnPropertyChanged();
            }
        }

        //public string TxtSeal4
        //{
        //    get => _txtSeal4;
        //    set
        //    {
        //        _txtSeal4 = value;
        //        OnPropertyChanged();
        //    }
        //}

        //public string TxtPosition
        //{
        //    get => txtPosition;
        //    set
        //    {
        //        txtPosition = value;
        //        OnPropertyChanged();
        //    }
        //}
        //public string DataContainers
        //{
        //    get => _dataContainers;
        //    set => SetProperty(ref _dataContainers, value);
        //}

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
        public FclSealAforoViewModel()
        {
            Title = "Seal Aforo";
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

            esActivo = true;
            TapCommand = new Command(OnTapped);
            SaveChangesCommand = new Command(async (o) => await OnSaveClicked(o));
            CancelCommand = new Command(OnCancelClicked);
            ConsultCommand = new Command(OnConsultClicked);
        }
       
       
        public void CleanPages()
        {
            IsBusy = false;
            esActivo = true;
            TxtNumContainer = "";
            TxtSealCGSA = "";
            TxtSeal1 = "";
            TxtSeal2 = "";
            TxtSeal3 = "";
            //TxtSeal4 = "";
            //TxtPosition = "";
            gkey = -1;
            referencia = string.Empty;
            //_dataContainers = string.Empty;

            OnPropertyChanged(nameof(TxtNumContainer));
            OnPropertyChanged(nameof(TxtSealCGSA));
            OnPropertyChanged(nameof(TxtSeal1));
            OnPropertyChanged(nameof(TxtSeal2));
            OnPropertyChanged(nameof(TxtSeal3));
            //OnPropertyChanged(nameof(TxtSeal4));
            OnPropertyChanged(nameof(esActivo));
            //OnPropertyChanged(nameof(TxtPosition));
            //OnPropertyChanged(nameof(DataContainers));

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
            esActivo = true;
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
            await GetAgenda(null, userName);
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


                    MyAgenda = await datos.GetDataConteinersImpo(txtNumContainer);
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
        private async Task CargaDetalle(ObservableCollection<dataContainers> oDet)
        {
            if (!esActivo) { return; }

            if (oDet != null)
            {
                esActivo = true;
                
                _selectedItem = oDet.FirstOrDefault();

                if (!string.IsNullOrEmpty(txtNumContainer) && _selectedItem.visit != null)
                {
                    //_dataContainers = string.Format("[PORT:{0} | WEIGHT:{1} | TYPE:{2} | °C:{3} | IMO:{4} | REF:{5}]", _selectedItem.puerto1, _selectedItem.peso, _selectedItem.tipo, _selectedItem.temperatura, _selectedItem.imo, _selectedItem.visit);
                    ////_dataContainers = _selectedItem.notas;

                    await App.Current.MainPage.DisplayAlert("Validation", "Successful", "OK");
                    esActivo = false;

                    gkey = _selectedItem.gkey;
                    referencia = _selectedItem.visit;
                }
                else
                {
                    esActivo = true;
                    if (!string.IsNullOrEmpty(txtNumContainer))
                    {
                        await App.Current.MainPage.DisplayAlert("Validation", "Unit not found :" + TxtNumContainer, "OK");
                    }
                }
            }
            else
            {
                ////_dataContainers = string.Empty;
            }
            ////OnPropertyChanged(nameof(DataContainers));
            OnPropertyChanged(nameof(esActivo));
        }

        public async void LoadItemId(ObservableCollection<dataContainers> oDet)
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
        public ObservableCollection<dataContainers> MyAgenda { get; private set; }

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
                    //if (string.IsNullOrEmpty(DataContainers))
                    //{
                    //    await App.Current.MainPage.DisplayAlert("Error", "Please search the Container", "Close");
                    //    flags = true;
                    //    return;
                    //}
                    if (string.IsNullOrEmpty(TxtNumContainer))
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please enter Container", "Close");
                        flags = true;
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtSealCGSA))
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please enter Seal CGSA", "Close");
                        flags = true;
                        return;
                    }

                    if (string.IsNullOrEmpty(TxtSeal1))
                    {
                        await App.Current.MainPage.DisplayAlert("Error", "Please enter Seal 1", "Close");
                        flags = true;
                        return;
                    }
                    if (string.IsNullOrEmpty(TxtSeal2))
                    {
                        TxtSeal2 = string.Empty;
                    }
                    if (string.IsNullOrEmpty(TxtSeal3))
                    {
                        TxtSeal3 = string.Empty;
                    }
                    //if (string.IsNullOrEmpty(TxtSeal4))
                    //{
                    //    TxtSeal4 = string.Empty;
                    //}

                    //if (ArrayFoto==null && ArrayFoto1 == null && ArrayFoto2 == null && ArrayFoto3 == null)
                    //{
                    //    await App.Current.MainPage.DisplayAlert("Error", "Enter at least one photo", "Close");
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
                            var v_IP = App.Current.Properties["UserIp"];

                            sealRegistroPreEmbarqueYaforo objSeal = new sealRegistroPreEmbarqueYaforo();
                            objSeal.container = TxtNumContainer.ToUpper();
                            objSeal.sello_CGSA = TxtSealCGSA.ToUpper();
                            objSeal.sello1 = TxtSeal1.ToUpper();
                            objSeal.sello2 = TxtSeal2.ToUpper();
                            objSeal.sello3 = TxtSeal3.ToUpper();
                            //objSeal.sello4 = TxtSeal4.ToUpper();
                            objSeal.ip = v_IP.ToString();
                            objSeal.usuarioCrea = userName.ToLower();

                            //objSeal.dataContainer = DataContainers;
                            //objSeal.referencia = referencia;
                            objSeal.gkey = gkey;
                            //objSeal.position = TxtPosition;
                            //objSeal.xmlN4Discharge = string.Empty;
                            //objSeal.respuestaN4Discharge = string.Empty;

                            ApiModels.AppModels.Base msg = await datos.SetSealAforo(objSeal, ArrayFoto, ArrayFoto1, ArrayFoto2, ArrayFoto3).ConfigureAwait(true);
                            await App.Current.MainPage.DisplayAlert("Respuesta", msg.messages, "Close");
                            CleanPages();
                            
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
