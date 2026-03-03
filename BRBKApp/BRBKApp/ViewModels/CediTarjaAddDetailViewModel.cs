using Acr.UserDialogs;
using ApiModels.AppModels;
using ApiModels.Parametros;
using BRBKApp.DA;
using BRBKApp.Models;
using Plugin.Media.Abstractions;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.CommunityToolkit.Extensions;
using Newtonsoft.Json;

namespace BRBKApp.ViewModels
{
    public class CediTarjaAddDetailViewModel : BaseViewModel
    {
        private string numeroTarja;

        private string _descripcionProducto;


        public string DescripcionProducto
        {
            get => _descripcionProducto;
            set => SetProperty(ref _descripcionProducto, value);
        }


        public string NumeroTarja
        {
            get => string.Format("Agregando detalle para tarja: {0}", numeroTarja);
            set
            {
                numeroTarja = value;
                OnPropertyChanged(nameof(NumeroTarja));
            }
        }
        public Command AddCommand { get; }
        public Command CancelCommand { get; }
        private AppModelCediTarjaDetalle _currentEntry;
        public AppModelCediTarjaDetalle CurrentEntry
        {
            get => _currentEntry;
            set
            {
                _currentEntry = value;
                OnPropertyChanged(); // importante para notificar cambio en el binding
            }
        }
        public CediTarjaMensaje CediTarja { get; set; }
        public CediTarjaAddDetailViewModel(CediTarjaMensaje entry)
        {
            Title = "Agregar detalle Tarja";
            AddCommand = new Command(OnCrearClicked);
            CancelCommand = new Command(OnCancelClicked);
            if (CurrentEntry == null) CurrentEntry = new AppModelCediTarjaDetalle();
            CediTarja = entry;
            this.numeroTarja = entry.TarjaId.ToString();
            _descripcionProducto = entry.DescripcionProducto;
            IniciarPropiedadesFotos();
            TapCommand = new Command(OnTapped);
        }
        private async void OnCrearClicked()
        {
            bool segraba = await App.Current.MainPage.DisplayAlert("Confirmar", "¿Desea registrar?", "De Acuerdo", "Cancelar");
            if (!segraba)
            {
                return;
            }

            bool procesado = await Grabar();

            if (!procesado)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se puedo Grabar el detalle de la tarja", "OK");
                return;
            }

            await Application.Current.MainPage.Navigation.PopModalAsync(true);

            await Task.Delay(TimeSpan.FromSeconds(2));
            await Application.Current.MainPage.DisplayToastAsync(new ToastOptions()
            {
                BackgroundColor = Color.LightGreen,
                Duration = TimeSpan.FromMilliseconds(2800),
                MessageOptions = new MessageOptions()
                {
                    Message = $"Detalle Tarja agregado"
                }
            });
        }
        private async void OnCancelClicked()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync(true);
        }

        #region fotos

        private ImageSource _btnIcon;
        public ImageSource BtnIcon
        {
            get { return _btnIcon; }
            set
            {
                _btnIcon = value;
                SetProperty(ref _btnIcon, value);
            }
        }

        public byte[] ArrayFotoX;  // Almacenar Foto temporal
        public byte[] ArrayFoto0;  // Almacenar Foto 0
        public byte[] ArrayFoto1;  // Almacenar Foto 1
        public byte[] ArrayFoto2;  // Almacenar Foto 2
        public byte[] ArrayFoto3;  // Almacenar Foto 3
        public byte[] ArrayFoto4;  // Almacenar Foto 4
        public byte[] ArrayFoto5;  // Almacenar Foto 5
        public byte[] ArrayFoto6;  // Almacenar Foto 6
        public byte[] ArrayFoto7;  // Almacenar Foto 7

        private ImageSource _imageSourceX;
        private ImageSource _imageSource0;
        private ImageSource _imageSource1;
        private ImageSource _imageSource2;
        private ImageSource _imageSource3;
        private ImageSource _imageSource4;
        private ImageSource _imageSource5;
        private ImageSource _imageSource6;
        private ImageSource _imageSource7;

        public ImageSource ImageSource0
        {
            get { return _imageSource0; }
            set
            {
                _imageSource0 = value;
                SetProperty(ref _imageSource0, value);
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
        public ImageSource ImageSource4
        {
            get { return _imageSource4; }
            set
            {
                _imageSource4 = value;
                SetProperty(ref _imageSource4, value);
            }
        }
        public ImageSource ImageSource5
        {
            get { return _imageSource5; }
            set
            {
                _imageSource5 = value;
                SetProperty(ref _imageSource5, value);
            }
        }
        public ImageSource ImageSource6
        {
            get { return _imageSource6; }
            set
            {
                _imageSource6 = value;
                SetProperty(ref _imageSource6, value);
            }
        }
        public ImageSource ImageSource7
        {
            get { return _imageSource7; }
            set
            {
                _imageSource7 = value;
                SetProperty(ref _imageSource7, value);
            }
        }
        public Command TapCommand { get; }
        private void IniciarPropiedadesFotos()
        {
            _btnIcon = ImageSource.FromFile("icon_search.png");
            _imageSource0 = ImageSource.FromFile("icon.png");
            _imageSource1 = ImageSource.FromFile("icon.png");
            _imageSource2 = ImageSource.FromFile("icon.png");
            _imageSource3 = ImageSource.FromFile("icon.png");
            _imageSource4 = ImageSource.FromFile("icon.png");
            _imageSource5 = ImageSource.FromFile("icon.png");
            _imageSource6 = ImageSource.FromFile("icon.png");
            _imageSource7 = ImageSource.FromFile("icon.png");
            OnPropertyChanged(nameof(ImageSource0));
            OnPropertyChanged(nameof(ImageSource1));
            OnPropertyChanged(nameof(ImageSource2));
            OnPropertyChanged(nameof(ImageSource3));
            OnPropertyChanged(nameof(ImageSource4));
            OnPropertyChanged(nameof(ImageSource5));
            OnPropertyChanged(nameof(ImageSource6));
            OnPropertyChanged(nameof(ImageSource7));
            OnPropertyChanged(nameof(BtnIcon));
        }

        private async void OnTapped(object s)
        {
            string action = await App.Current.MainPage.DisplayActionSheet("¿What do you want to do?", "Cancel", null,
                "Take Photo", "Delete Photo");

            if (action == "Take Photo")
            {
                Camara(1, s);
            }

            if (action == "Delete Photo")
            {
                string ss = s.ToString();
                switch (ss)
                {
                    case "0":
                        _imageSource0 = ImageSource.FromFile("icon.png");
                        ArrayFoto0 = null;
                        OnPropertyChanged(nameof(ImageSource0));
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
                    case "4":
                        _imageSource4 = ImageSource.FromFile("icon.png");
                        ArrayFoto4 = null;
                        OnPropertyChanged(nameof(ImageSource4));
                        break;
                    case "5":
                        _imageSource5 = ImageSource.FromFile("icon.png");
                        ArrayFoto5 = null;
                        OnPropertyChanged(nameof(ImageSource5));
                        break;
                    case "6":
                        _imageSource6 = ImageSource.FromFile("icon.png");
                        ArrayFoto6 = null;
                        OnPropertyChanged(nameof(ImageSource6));
                        break;
                    case "7":
                        _imageSource7 = ImageSource.FromFile("icon.png");
                        ArrayFoto7 = null;
                        OnPropertyChanged(nameof(ImageSource7));
                        break;
                    default:
                        break;
                }
            }
        }

        private async Task Camara(int Accion, object imageControl)
        {
            try
            {
                MediaFile file = null;
                await CrossMedia.Current.Initialize();
                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsPickPhotoSupported)
                {
                    await App.Current.MainPage.DisplayAlert("Cámara no habilitada", "Revise su dispositivo", "Cerrar");
                    return;
                }

                if (Accion == 1) // TOMA FOTO
                {
                    file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        SaveToAlbum = false, // más rápido si no necesitas guardar en galería
                        Directory = "VHS",
                        Name = $"{Guid.NewGuid()}.jpg",
                        CompressionQuality = 75, // reduce tamaño sin perder mucha calidad
                        CustomPhotoSize = 50, // porcentaje (usa esto si `PhotoSize.Custom`)
                        PhotoSize = PhotoSize.Custom,
                        DefaultCamera = CameraDevice.Rear
                    });
                }
                else if (Accion == 2) // SUBE FOTO
                {
                    //PickMediaOptions c = new PickMediaOptions();
                    //c.PhotoSize = PhotoSize.Small;
                    PickMediaOptions c = new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.Custom,
                        CustomPhotoSize = 60 // Porcentaje (10% del tamaño original)
                    };

                    file = await CrossMedia.Current.PickPhotoAsync(c);
                }

                if (file != null)
                {
                    using (var stream = file.GetStream())
                    {
                        if (stream == null || stream.Length == 0)
                        {
                            Console.WriteLine("Stream is null or empty.");
                            return;
                        }

                        byte[] arrayFotoX = ReadImage(stream);
                        Console.WriteLine($"ReadImage: Array length = {arrayFotoX?.Length ?? 0}"); // Depuración

                        if (arrayFotoX != null && arrayFotoX.Length > 0)
                        {
                            ImageSource imageSourceX = ImageSource.FromStream(() =>
                            {
                                var memoryStream = new MemoryStream(arrayFotoX);
                                Console.WriteLine($"Creating ImageSource. Stream length: {memoryStream.Length}");
                                return memoryStream;
                            });

                            string index = imageControl.ToString();
                            Console.WriteLine($"Assigning image to index: {index}");
                            switch (index)
                            {
                                case "0":
                                    _imageSource0 = imageSourceX; // Asignación directa
                                    ArrayFoto0 = arrayFotoX;
                                    OnPropertyChanged(nameof(ImageSource0)); // Notificación manual
                                    Console.WriteLine("ImageSource0 assigned and notified");
                                    break;
                                case "1":
                                    _imageSource1 = imageSourceX;
                                    ArrayFoto1 = arrayFotoX;
                                    OnPropertyChanged(nameof(ImageSource1));
                                    Console.WriteLine("ImageSource1 assigned and notified");
                                    break;
                                case "2":
                                    _imageSource2 = imageSourceX;
                                    ArrayFoto2 = arrayFotoX;
                                    OnPropertyChanged(nameof(ImageSource2));
                                    Console.WriteLine("ImageSource2 assigned and notified");
                                    break;
                                case "3":
                                    _imageSource3 = imageSourceX;
                                    ArrayFoto3 = arrayFotoX;
                                    OnPropertyChanged(nameof(ImageSource3));
                                    Console.WriteLine("ImageSource3 assigned and notified");
                                    break;
                                case "4":
                                    _imageSource4 = imageSourceX;
                                    ArrayFoto4 = arrayFotoX;
                                    OnPropertyChanged(nameof(ImageSource4));
                                    Console.WriteLine("ImageSource4 assigned and notified");
                                    break;
                                case "5":
                                    _imageSource5 = imageSourceX;
                                    ArrayFoto5 = arrayFotoX;
                                    OnPropertyChanged(nameof(ImageSource5));
                                    Console.WriteLine("ImageSource5 assigned and notified");
                                    break;
                                case "6":
                                    _imageSource6 = imageSourceX;
                                    ArrayFoto6 = arrayFotoX;
                                    OnPropertyChanged(nameof(ImageSource6));
                                    Console.WriteLine("ImageSource6 assigned and notified");
                                    break;
                                case "7":
                                    _imageSource7 = imageSourceX;
                                    ArrayFoto7 = arrayFotoX;
                                    OnPropertyChanged(nameof(ImageSource7));
                                    Console.WriteLine("ImageSource7 assigned and notified");
                                    break;
                                default:
                                    Console.WriteLine($"Invalid index: {index}");
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("ArrayFotoX is null or empty.");
                        }
                    }
                    // file.Dispose() se maneja con using
                }
                else
                {
                    Console.WriteLine("No se seleccionó ninguna foto.");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Cerrar");
                Console.WriteLine($"Error en Camara: {ex.Message} - StackTrace: {ex.StackTrace}");
            }
        }
        public byte[] ReadImage(Stream input)
        {
            using (var memoryStream = new MemoryStream())
            {
                input.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        private async Task<bool> Grabar()
        {
            try
            {
                bool flags = false;

                var ids = App.Current.Properties["Username"];
                string userName = ids.ToString();
                if (flags == false)
                {
                    var current = Connectivity.NetworkAccess;
                    if (current == NetworkAccess.None)
                    {
                        await App.Current.MainPage.DisplayAlert("Internet Error", "Please verify your internet connection", "OK");
                        flags = true;
                        return false;
                    }
                    if (current == NetworkAccess.Internet)
                    {
                        UserDialogs.Instance.ShowLoading("Sending...");
                        ParametroVHSTarjaDetalleAdd request = new ParametroVHSTarjaDetalleAdd();
                        request.TarjaID = this.CediTarja.TarjaId;
                        request.NumeroMotor = CurrentEntry.NumeroMotor;
                        request.Observaciones = CurrentEntry.Observaciones;
                        request.DocumentoTransporte = CurrentEntry.DocumentoTransporte;
                        request.InformacionVehiculo = CurrentEntry.InformacionVehiculo;
                        request.PackingList = CurrentEntry.PackingList;
                        request.TipoCargaDescripcion = CurrentEntry.TipoCargaDescripcion;
                        request.UbicacionBodega = CurrentEntry.UbicacionBodega;
                        request.VIN = CurrentEntry.VIN;
                        request.Fotos = new List<ApiModels.AppModels.VHSTarjaDetalleFoto>();
                        request.Fotos.Add(new ApiModels.AppModels.VHSTarjaDetalleFoto() { ArrayFoto = this.ArrayFoto0, Orden = 1 });
                        request.Fotos.Add(new ApiModels.AppModels.VHSTarjaDetalleFoto() { ArrayFoto = this.ArrayFoto1, Orden = 2 });
                        request.Fotos.Add(new ApiModels.AppModels.VHSTarjaDetalleFoto() { ArrayFoto = this.ArrayFoto2, Orden = 3 });
                        request.Fotos.Add(new ApiModels.AppModels.VHSTarjaDetalleFoto() { ArrayFoto = this.ArrayFoto3, Orden = 4 });
                        request.Fotos.Add(new ApiModels.AppModels.VHSTarjaDetalleFoto() { ArrayFoto = this.ArrayFoto4, Orden = 5 });
                        request.Fotos.Add(new ApiModels.AppModels.VHSTarjaDetalleFoto() { ArrayFoto = this.ArrayFoto5, Orden = 6 });
                        request.Fotos.Add(new ApiModels.AppModels.VHSTarjaDetalleFoto() { ArrayFoto = this.ArrayFoto6, Orden = 7 });
                        request.Fotos.Add(new ApiModels.AppModels.VHSTarjaDetalleFoto() { ArrayFoto = this.ArrayFoto7, Orden = 8 });
                        request.Create_date = DateTime.Now;
                        request.Create_user = userName;
                        request.Status = true;


                        var jsonString = JsonConvert.SerializeObject(request, Formatting.Indented);

                        var response = await ServiceCedi.AddTarjaDetail(request);

                        return true;
                    }
                }
                return !flags;
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Cerrar");
                return false;
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        #endregion
    }
}