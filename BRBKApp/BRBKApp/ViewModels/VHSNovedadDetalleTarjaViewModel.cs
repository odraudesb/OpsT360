using Acr.UserDialogs;
using ApiModels.Parametros;
using BRBKApp.DA;
using Plugin.Media.Abstractions;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.CommunityToolkit.Extensions;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using ApiModels.AppModels;
using ApiDatos;

namespace BRBKApp.ViewModels
{
    public class VHSNovedadDetalleTarjaViewModel : BaseViewModel
    {


        public ObservableCollection<TipoNovedadModel> TiposNovedad { get; set; }
        private TipoNovedadModel _selectedTipoNovedad;
        public TipoNovedadModel SelectedTipoNovedad
        {
            get => _selectedTipoNovedad;
            set => SetProperty(ref _selectedTipoNovedad, value);
        }


        private string descripcionDetalle;
        public string DescripcionDetalle
        {
            get => descripcionDetalle;
            set => SetProperty(ref descripcionDetalle, value);
        }

        public long DetalleTarjaID { get; set; }
        public string NumeroTarja { get; set; }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }
        public Command TapCommand { get; }

        #region Fotos
        public byte[] ArrayFoto0;
        public byte[] ArrayFoto1;
        public byte[] ArrayFoto2;
        public byte[] ArrayFoto3;
        public byte[] ArrayFoto4;
        public byte[] ArrayFoto5;
        public byte[] ArrayFoto6;
        public byte[] ArrayFoto7;

        private ImageSource _imageSource0;
        public ImageSource ImageSource0 { get => _imageSource0; set => SetProperty(ref _imageSource0, value); }
        private ImageSource _imageSource1;
        public ImageSource ImageSource1 { get => _imageSource1; set => SetProperty(ref _imageSource1, value); }
        private ImageSource _imageSource2;
        public ImageSource ImageSource2 { get => _imageSource2; set => SetProperty(ref _imageSource2, value); }
        private ImageSource _imageSource3;
        public ImageSource ImageSource3 { get => _imageSource3; set => SetProperty(ref _imageSource3, value); }
        private ImageSource _imageSource4;
        public ImageSource ImageSource4 { get => _imageSource4; set => SetProperty(ref _imageSource4, value); }
        private ImageSource _imageSource5;
        public ImageSource ImageSource5 { get => _imageSource5; set => SetProperty(ref _imageSource5, value); }
        private ImageSource _imageSource6;
        public ImageSource ImageSource6 { get => _imageSource6; set => SetProperty(ref _imageSource6, value); }
        private ImageSource _imageSource7;
        public ImageSource ImageSource7 { get => _imageSource7; set => SetProperty(ref _imageSource7, value); }
        #endregion

        public VHSNovedadDetalleTarjaViewModel(Models.AppModelNovedadDetalleTarja detalle)
        {
            DetalleTarjaID = detalle.DetalleTarjaID;
            NumeroTarja = $"Novedad para DetalleTarja: {detalle.DetalleTarjaID}";
            //DescripcionDetalle = detalle.Descripcion;

            SaveCommand = new Command(OnSaveClicked);
            CancelCommand = new Command(OnCancelClicked);
            TapCommand = new Command(OnTapped);

            InitImages();

            TiposNovedad = new ObservableCollection<TipoNovedadModel>();

            // Ejecutar la carga del combo en background sin bloquear el constructor
            Device.BeginInvokeOnMainThread(async () =>
            {
                await LoadTiposNovedadAsync();
            });
        }


        private void InitImages()
        {
            ImageSource0 = ImageSource.FromFile("icon.png");
            ImageSource1 = ImageSource.FromFile("icon.png");
            ImageSource2 = ImageSource.FromFile("icon.png");
            ImageSource3 = ImageSource.FromFile("icon.png");
            ImageSource4 = ImageSource.FromFile("icon.png");
            ImageSource5 = ImageSource.FromFile("icon.png");
            ImageSource6 = ImageSource.FromFile("icon.png");
            ImageSource7 = ImageSource.FromFile("icon.png");
        }

        private async void OnSaveClicked()
        {

            if (SelectedTipoNovedad == null)
            {
                await Application.Current.MainPage.DisplayAlert("Validación", "Seleccione el tipo de novedad.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(DescripcionDetalle))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Ingrese la descripción.", "OK");
                return;
            }

            bool confirm = await Application.Current.MainPage.DisplayAlert("Confirmar", "¿Desea registrar la novedad?", "Sí", "Cancelar");
            if (!confirm)
                return;

            UserDialogs.Instance.ShowLoading("Registrando...");

            try
            {
                var usuario = App.Current.Properties["Username"].ToString();

                var request = new ParametroRegistrarNovedadDetalleTarja
                {
                    DetalleTarjaID = this.DetalleTarjaID,
                    Descripcion = this.DescripcionDetalle,
                    TipoNovedadID = SelectedTipoNovedad.TipoNovedadID, // 
                    Usuario = usuario,
                    Fotos = new List<ParametroRegistrarNovedadDetalleTarjaFoto>
                {
                    new ParametroRegistrarNovedadDetalleTarjaFoto { ArrayFoto = this.ArrayFoto0, Orden = 1 },
                    new ParametroRegistrarNovedadDetalleTarjaFoto { ArrayFoto = this.ArrayFoto1, Orden = 2 },
                    new ParametroRegistrarNovedadDetalleTarjaFoto { ArrayFoto = this.ArrayFoto2, Orden = 3 },
                    new ParametroRegistrarNovedadDetalleTarjaFoto { ArrayFoto = this.ArrayFoto3, Orden = 4 },
                    new ParametroRegistrarNovedadDetalleTarjaFoto { ArrayFoto = this.ArrayFoto4, Orden = 5 },
                    new ParametroRegistrarNovedadDetalleTarjaFoto { ArrayFoto = this.ArrayFoto5, Orden = 6 },
                    new ParametroRegistrarNovedadDetalleTarjaFoto { ArrayFoto = this.ArrayFoto6, Orden = 7 },
                    new ParametroRegistrarNovedadDetalleTarjaFoto { ArrayFoto = this.ArrayFoto7, Orden = 8 }
                }
                };

                string jsonRequest = JsonConvert.SerializeObject(request, Formatting.Indented);



                var response = await ServiceVHS.RegistrarNovedadDetalleTarja(request);

                if (response.Resultado.Respuesta)
                {
                

                    await Application.Current.MainPage.DisplayAlert("Éxito", "Novedad registrada correctamente.", "OK");

                    await Application.Current.MainPage.Navigation.PopModalAsync();
                }

                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", string.Join("\n", response.Resultado.Mensajes), "OK");
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

        private async void OnCancelClicked()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }

        private async void OnTapped(object parameter)
        {
            string index = parameter?.ToString();
            var action = await Application.Current.MainPage.DisplayActionSheet("Opciones", "Cancelar", null, "Tomar Foto", "Eliminar Foto");
            if (action == "Tomar Foto")
                await TakePhoto(index);
            if (action == "Eliminar Foto")
                RemovePhoto(index);
        }

        private async Task TakePhoto(string index)
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

                // Tomar la foto con reducción de tamaño
                file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    SaveToAlbum = false, // No guardes en galería si no lo necesitas
                    PhotoSize = PhotoSize.Custom,
                    CustomPhotoSize = 60, // Porcentaje de reducción (ajústalo a tu necesidad)
                    CompressionQuality = 60 // Comprime la imagen (0-100)
                });

                if (file == null)
                    return;

                byte[] arrayFoto;
                using (var ms = new MemoryStream())
                {
                    file.GetStream().CopyTo(ms);
                    arrayFoto = ms.ToArray();
                }

                ImageSource imageSource = ImageSource.FromStream(() => new MemoryStream(arrayFoto));

                switch (index)
                {
                    case "0": ImageSource0 = imageSource; ArrayFoto0 = arrayFoto; break;
                    case "1": ImageSource1 = imageSource; ArrayFoto1 = arrayFoto; break;
                    case "2": ImageSource2 = imageSource; ArrayFoto2 = arrayFoto; break;
                    case "3": ImageSource3 = imageSource; ArrayFoto3 = arrayFoto; break;
                    case "4": ImageSource4 = imageSource; ArrayFoto4 = arrayFoto; break;
                    case "5": ImageSource5 = imageSource; ArrayFoto5 = arrayFoto; break;
                    case "6": ImageSource6 = imageSource; ArrayFoto6 = arrayFoto; break;
                    case "7": ImageSource7 = imageSource; ArrayFoto7 = arrayFoto; break;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Cerrar");
            }
        }


        private void RemovePhoto(string index)
        {
            switch (index)
            {
                case "0": ImageSource0 = ImageSource.FromFile("icon.png"); ArrayFoto0 = null; break;
                case "1": ImageSource1 = ImageSource.FromFile("icon.png"); ArrayFoto1 = null; break;
                case "2": ImageSource2 = ImageSource.FromFile("icon.png"); ArrayFoto2 = null; break;
                case "3": ImageSource3 = ImageSource.FromFile("icon.png"); ArrayFoto3 = null; break;
                case "4": ImageSource4 = ImageSource.FromFile("icon.png"); ArrayFoto4 = null; break;
                case "5": ImageSource5 = ImageSource.FromFile("icon.png"); ArrayFoto5 = null; break;
                case "6": ImageSource6 = ImageSource.FromFile("icon.png"); ArrayFoto6 = null; break;
                case "7": ImageSource7 = ImageSource.FromFile("icon.png"); ArrayFoto7 = null; break;
            }
        }

        private byte[] ReadImage(Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private async Task LoadTiposNovedadAsync()
        {
            try
            {
                var datos = new Datos();
                var response = await datos.GetTiposNovedadAsync();
                if (response.Resultado.Respuesta)
                {
                    TiposNovedad.Clear();
                    if (response.Respuesta != null)
                    {
                        foreach (var tipo in response.Respuesta)
                        {
                            TiposNovedad.Add(tipo);
                        }
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", string.Join("\n", response.Resultado.Mensajes), "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al cargar tipos de novedad: {ex.Message}", "OK");
            }
        }


    }

}
