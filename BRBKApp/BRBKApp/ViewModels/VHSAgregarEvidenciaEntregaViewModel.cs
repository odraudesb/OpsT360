using Acr.UserDialogs;
using ApiModels.Parametros;
using ApiModels.AppModels;
using BRBKApp.DA;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BRBKApp.ViewModels
{
    public class VHSAgregarEvidenciaEntregaViewModel : BaseViewModel
    {
        private readonly long _vehiculoDespachadoId;

        public string Observacion { get; set; }

        public Command TapCommand { get; }
        public Command GrabarCommand { get; }
        public Command CerrarCommand { get; }

        // Fotos en bytes
        public byte[] ArrayFoto0;
        public byte[] ArrayFoto1;
        public byte[] ArrayFoto2;
        public byte[] ArrayFoto3;
        public byte[] ArrayFoto4;
        public byte[] ArrayFoto5;
        public byte[] ArrayFoto6;
        public byte[] ArrayFoto7;

        // ImageSources
        private ImageSource _imageSource0;
        private ImageSource _imageSource1;
        private ImageSource _imageSource2;
        private ImageSource _imageSource3;
        private ImageSource _imageSource4;
        private ImageSource _imageSource5;
        private ImageSource _imageSource6;
        private ImageSource _imageSource7;

        public ImageSource Image0 { get => _imageSource0; set { _imageSource0 = value; OnPropertyChanged(nameof(Image0)); } }
        public ImageSource Image1 { get => _imageSource1; set { _imageSource1 = value; OnPropertyChanged(nameof(Image1)); } }
        public ImageSource Image2 { get => _imageSource2; set { _imageSource2 = value; OnPropertyChanged(nameof(Image2)); } }
        public ImageSource Image3 { get => _imageSource3; set { _imageSource3 = value; OnPropertyChanged(nameof(Image3)); } }
        public ImageSource Image4 { get => _imageSource4; set { _imageSource4 = value; OnPropertyChanged(nameof(Image4)); } }
        public ImageSource Image5 { get => _imageSource5; set { _imageSource5 = value; OnPropertyChanged(nameof(Image5)); } }
        public ImageSource Image6 { get => _imageSource6; set { _imageSource6 = value; OnPropertyChanged(nameof(Image6)); } }
        public ImageSource Image7 { get => _imageSource7; set { _imageSource7 = value; OnPropertyChanged(nameof(Image7)); } }

        public VHSAgregarEvidenciaEntregaViewModel(long vehiculoDespachadoId)
        {
            _vehiculoDespachadoId = vehiculoDespachadoId;

            TapCommand = new Command(OnTapped);
            GrabarCommand = new Command(async () => await Grabar());
            CerrarCommand = new Command(async () => await Cerrar());

            InicializarImagenes();
        }

        private void InicializarImagenes()
        {
            var icon = ImageSource.FromFile("icon.png");
            Image0 = Image1 = Image2 = Image3 = Image4 = Image5 = Image6 = Image7 = icon;
        }

        private async void OnTapped(object s)
        {
            string action = await Application.Current.MainPage.DisplayActionSheet(
                "Opciones", "Cancelar", null, "Tomar Foto", "Eliminar Foto");

            if (action == "Tomar Foto")
            {
                await Camara(1, s);
            }
            else if (action == "Subir Foto")
            {
                await Camara(2, s);
            }
            else if (action == "Eliminar Foto")
            {
                EliminarFoto(s.ToString());
            }
        }

        private void EliminarFoto(string idx)
        {
            var icon = ImageSource.FromFile("icon.png");
            switch (idx)
            {
                case "0": Image0 = icon; ArrayFoto0 = null; break;
                case "1": Image1 = icon; ArrayFoto1 = null; break;
                case "2": Image2 = icon; ArrayFoto2 = null; break;
                case "3": Image3 = icon; ArrayFoto3 = null; break;
                case "4": Image4 = icon; ArrayFoto4 = null; break;
                case "5": Image5 = icon; ArrayFoto5 = null; break;
                case "6": Image6 = icon; ArrayFoto6 = null; break;
                case "7": Image7 = icon; ArrayFoto7 = null; break;
            }
        }

        private async Task Camara(int accion, object imageControl)
        {
            try
            {
                await CrossMedia.Current.Initialize();
                MediaFile file = null;

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await Application.Current.MainPage.DisplayAlert("Cámara no disponible", "Verifique su dispositivo", "Cerrar");
                    return;
                }

                if (accion == 1) // Tomar Foto
                {
                    //file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    //{
                    //    PhotoSize = PhotoSize.Custom,
                    //    CustomPhotoSize = 60
                    //});

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
                else if (accion == 2) // Subir Foto
                {
                    file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.Custom,
                        CustomPhotoSize = 60
                    });
                }

                if (file == null) return;

                byte[] arrayFoto;
                using (var ms = new MemoryStream())
                {
                    file.GetStream().CopyTo(ms);
                    arrayFoto = ms.ToArray();
                }

                ImageSource fotoSource = ImageSource.FromStream(() => new MemoryStream(arrayFoto));
                string idx = imageControl.ToString();

                switch (idx)
                {
                    case "0": Image0 = fotoSource; ArrayFoto0 = arrayFoto; break;
                    case "1": Image1 = fotoSource; ArrayFoto1 = arrayFoto; break;
                    case "2": Image2 = fotoSource; ArrayFoto2 = arrayFoto; break;
                    case "3": Image3 = fotoSource; ArrayFoto3 = arrayFoto; break;
                    case "4": Image4 = fotoSource; ArrayFoto4 = arrayFoto; break;
                    case "5": Image5 = fotoSource; ArrayFoto5 = arrayFoto; break;
                    case "6": Image6 = fotoSource; ArrayFoto6 = arrayFoto; break;
                    case "7": Image7 = fotoSource; ArrayFoto7 = arrayFoto; break;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Cerrar");
            }
        }

        private async Task Grabar()
        {
            if (string.IsNullOrWhiteSpace(Observacion))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Debe ingresar una observación.", "OK");
                return;
            }

            UserDialogs.Instance.ShowLoading("Enviando...");

            try
            {
                string usuario = App.Current.Properties["Username"]?.ToString() ?? "sistema";

                var request = new ParametroVHSCrearEvidenciaEntrega
                {
                    VehiculoDespachadoID = _vehiculoDespachadoId,
                    Observacion = this.Observacion,
                    Usuario = usuario,
                    Fotos = new List<VHSEvidenciaEntregaFoto>()
                };

                var fotos = new[]
                {
                    ArrayFoto0, ArrayFoto1, ArrayFoto2, ArrayFoto3,
                    ArrayFoto4, ArrayFoto5, ArrayFoto6, ArrayFoto7
                };

                foreach (var foto in fotos)
                {
                    if (foto != null && foto.Length > 0)
                    {
                        request.Fotos.Add(new VHSEvidenciaEntregaFoto { ArrayFoto = foto });
                    }
                }



                var response = await ServiceVHS.RegistraEvidenciaEntrega(request);

                if (response?.Resultado?.Respuesta == true)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", "Evidencia registrada correctamente.", "OK");
                    
                    MessagingCenter.Send(this, "EvidenciaGrabada");

                    await Cerrar();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        string.Join("\n", response?.Resultado?.Mensajes ?? new List<string> { "Error desconocido" }),
                        "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private async Task Cerrar()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }
    }
}
