using Acr.UserDialogs;
using ApiDatos;
using ApiModels.AppModels;
using ApiModels.Parametros;
using BRBKApp.DA;
using BRBKApp.Models;
using BRBKApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BRBKApp.ViewModels
{
    public class CediTarjaConsultaDetailViewModel : BaseViewModel, INotifyPropertyChanged
    {
        private string _numeroTarja;
        private AppModelCediTarjaDetalle _detalle;
        private string _detalleTarjaIdInput;
        private bool _showCodeInput;
        private bool _showDetails;
        private bool _opcionEscaneo;
        private readonly PrintService _printService = new PrintService();
        private ObservableCollection<CediBloque> _bloques = new ObservableCollection<CediBloque>();
        private CediBloque _selectedBloque;
        private int _numeroBloque;
        private CediTarjaMensaje _entry;


        public bool EsEditable
        {
            get => _esEditable;
            set => SetProperty(ref _esEditable, value);
        }
        private bool _esEditable;


        public bool MostrarBotonesGuardarImprimir
        {
            get => _mostrarBotonesGuardarImprimir;
            set => SetProperty(ref _mostrarBotonesGuardarImprimir, value);
        }
        private bool _mostrarBotonesGuardarImprimir;

        private string _origen;
        public string Origen
        {
            get => _origen;
            set
            {
                _origen = value;

                if (_origen == "Novedad")
                {
                    EsOrigenDespacho = false;
                    ShowCodeInput = true;
                    ShowDetails = false;
                    OpcionEscaneo = true;
                    MostrarBotonNovedad = true;
                    EsEditable = false; // DESHABILITA los Entry
                    MostrarBotonesGuardarImprimir = false; //

                }
                else if (_origen == "Despacho")
                {
                    EsOrigenDespacho = true;
                    MostrarBotonNovedad = false;
                    EsEditable = true;
                    MostrarBotonesGuardarImprimir = true; // ✅ Mostrar
                }
                else
                {
                    EsOrigenDespacho = false;
                    MostrarBotonNovedad = false;
                    EsEditable = true;
                    MostrarBotonesGuardarImprimir = true; // ✅ Mostrar
                }

                OnPropertyChanged(nameof(EsOrigenDespacho));
                OnPropertyChanged(nameof(ShowCodeInput));
                OnPropertyChanged(nameof(ShowDetails));
                OnPropertyChanged(nameof(OpcionEscaneo));
                OnPropertyChanged(nameof(MostrarBotonNovedad));
                OnPropertyChanged(nameof(EsEditable));
                OnPropertyChanged(nameof(MostrarBotonesGuardarImprimir));
            }

        }

        private bool _puedeDespachar = true;
        public bool PuedeDespachar
        {
            get => _puedeDespachar;
            set => SetProperty(ref _puedeDespachar, value);
        }

        private bool _mostrarBotonNovedad;
        public bool MostrarBotonNovedad
        {
            get => _mostrarBotonNovedad;
            set => SetProperty(ref _mostrarBotonNovedad, value);
        }

        public bool EsOrigenDespacho { get; set; }

        public bool ShowCodeInput
        {
            get => _showCodeInput;
            set => SetProperty(ref _showCodeInput, value);
        }

        public bool ShowDetails
        {
            get => _showDetails;
            set => SetProperty(ref _showDetails, value);
        }

        public bool OpcionEscaneo
        {
            get => _opcionEscaneo;
            set
            {
                if (_opcionEscaneo != value)
                {
                    _opcionEscaneo = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsScanButtonVisible));
                }
            }
        }

        public bool IsScanButtonVisible => !OpcionEscaneo;

        public string NumeroTarja
        {
            get => !_showCodeInput ? $"Consultando detalle para tarja: {_numeroTarja}" : "Ingrese o escanee el código de tarja";
            set => SetProperty(ref _numeroTarja, value);
        }

        public AppModelCediTarjaDetalle Detalle
        {
            get => _detalle;
            set => SetProperty(ref _detalle, value);
        }

        public string DetalleTarjaIdInput
        {
            get => _detalleTarjaIdInput;
            set
            {
                _detalleTarjaIdInput = value;
                OnPropertyChanged();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    string firstNumber = ExtractFirstNumber(value);
                    if (!string.IsNullOrWhiteSpace(firstNumber) && long.TryParse(firstNumber, out long detalleTarjaId))
                    {
                        _detalleTarjaIdInput = firstNumber;
                        OnPropertyChanged(nameof(DetalleTarjaIdInput));
                    }
                }
            }
        }

        private string ExtractFirstNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            Match match = Regex.Match(input, @"\b\d+\b");
            return match.Success ? match.Value : string.Empty;
        }

        public ObservableCollection<CediBloque> Bloques
        {
            get => _bloques;
            set => SetProperty(ref _bloques, value);
        }

        public CediBloque SelectedBloque
        {
            get => _selectedBloque;
            set => SetProperty(ref _selectedBloque, value);
        }

        public int NumeroBloque
        {
            get => _numeroBloque;
            set => SetProperty(ref _numeroBloque, value);
        }

        public Command CloseCommand { get; }
        public Command SearchByCodeCommand { get; }
        public Command ScanQRCommand { get; }
        public Command SaveCommand { get; }
        private ImageSource _imageSource0;
        public ImageSource ImageSource0
        {
            get => _imageSource0;
            set => SetProperty(ref _imageSource0, value);
        }

        private ImageSource _imageSource1;
        public ImageSource ImageSource1
        {
            get => _imageSource1;
            set => SetProperty(ref _imageSource1, value);
        }

        private ImageSource _imageSource2;
        public ImageSource ImageSource2
        {
            get => _imageSource2;
            set => SetProperty(ref _imageSource2, value);
        }

        private ImageSource _imageSource3;
        public ImageSource ImageSource3
        {
            get => _imageSource3;
            set => SetProperty(ref _imageSource3, value);
        }

        private ImageSource _imageSource4;
        public ImageSource ImageSource4
        {
            get => _imageSource4;
            set => SetProperty(ref _imageSource4, value);
        }

        private ImageSource _imageSource5;
        public ImageSource ImageSource5
        {
            get => _imageSource5;
            set => SetProperty(ref _imageSource5, value);
        }

        private ImageSource _imageSource6;
        public ImageSource ImageSource6
        {
            get => _imageSource6;
            set => SetProperty(ref _imageSource6, value);
        }

        private ImageSource _imageSource7;
        public ImageSource ImageSource7
        {
            get => _imageSource7;
            set => SetProperty(ref _imageSource7, value);
        }

        public CediTarjaConsultaDetailViewModel(CediTarjaMensaje entry = null)
        {
            Title = "Consultar detalle Tarja";
            CloseCommand = new Command(OnCloseClicked);
            SearchByCodeCommand = new Command(async () => await SearchByCode());
            ScanQRCommand = new Command(OnScanQR);
            SaveCommand = new Command(async () => await SaveDetail());
            Bloques = new ObservableCollection<CediBloque>();
            _entry = entry;
            MostrarBotonNovedad = false;

            _ = InitializeAsync();

            MessagingCenter.Subscribe<CediAgregarEvidenciaViewModel>(this, "EvidenciaGrabada", (sender) =>
            {
                PuedeDespachar = false;
            });
        }

        private async Task InitializeAsync()
        {
            await LoadBloquesAsync();

            Device.BeginInvokeOnMainThread(() =>
            {
                if (_entry != null)
                {
                    if (!string.IsNullOrEmpty(_entry.Origen))
                    {
                        Origen = _entry.Origen;
                    }
                    else
                    {
                        Origen = string.Empty;
                    }

                    if (_entry.DetalleTarjaID > 0 && _entry.Origen != "Novedad")
                    {
                        NumeroTarja = _entry.TarjaId.ToString();
                        ShowCodeInput = false;
                        ShowDetails = true;
                        OpcionEscaneo = false;
                    }
                    else if (_entry.Origen != "Novedad")
                    {
                        ShowCodeInput = true;
                        ShowDetails = false;
                        OpcionEscaneo = true;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(_origen))
                    {
                        EsOrigenDespacho = false;
                        ShowCodeInput = true;
                        ShowDetails = false;
                        OpcionEscaneo = true;
                        MostrarBotonNovedad = false;
                    }
                }

                OnPropertyChanged(nameof(ShowCodeInput));
                OnPropertyChanged(nameof(ShowDetails));
                OnPropertyChanged(nameof(OpcionEscaneo));
                OnPropertyChanged(nameof(EsOrigenDespacho));
            });

            if (_entry != null && _entry.DetalleTarjaID > 0)
            {
                await LoadDetalleAsync(_entry.DetalleTarjaID);
            }
        }

        private async Task LoadBloquesAsync()
        {
            try
            {
                var datos = new Datos();
                var response = await datos.GetCediBloquesAsync();
                if (response.Resultado.Respuesta)
                {
                    Bloques.Clear();
                    if (response.Respuesta != null)
                    {
                        foreach (var bloque in response.Respuesta)
                        {
                            if (bloque != null)
                            {
                                Bloques.Add(bloque);
                            }
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
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al cargar bloques: {ex.Message}", "OK");
            }
        }

        private async Task LoadDetalleAsync(long detalleTarjaId)
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Cargando...");
                var datos = new Datos();

                await LoadBloquesAsync();

                var response = await datos.GetCediTarjaDetailByIdAsync(detalleTarjaId);

                if (response.Resultado.Respuesta)
                {
                    Detalle = response.Respuesta;
                    NumeroBloque = Detalle.NumeroBloque;
                    SelectedBloque = Bloques.FirstOrDefault(b => b.Id == Detalle.Id);
                    if (Detalle.Fotos != null && Detalle.Fotos.Any())
                    {
                        await LoadImagesAsync();
                    }
                }
                else
                {
                    Detalle = null;
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

        private async Task LoadImagesAsync()
        {
            try
            {
                var datos = new Datos();
                if (Detalle?.Fotos != null)
                {
                    for (int i = 0; i < Detalle.Fotos.Count && i < 8; i++)
                    {
                        var foto = Detalle.Fotos[i];
                        if (!string.IsNullOrEmpty(foto.FotosVehiculo))
                        {
                            var imageBytes = await datos.GetImageAsync(foto.FotosVehiculo);
                            if (imageBytes != null)
                            {
                                var imageSource = ImageSource.FromStream(() => new System.IO.MemoryStream(imageBytes));
                                switch (i)
                                {
                                    case 0: ImageSource0 = imageSource; break;
                                    case 1: ImageSource1 = imageSource; break;
                                    case 2: ImageSource2 = imageSource; break;
                                    case 3: ImageSource3 = imageSource; break;
                                    case 4: ImageSource4 = imageSource; break;
                                    case 5: ImageSource5 = imageSource; break;
                                    case 6: ImageSource6 = imageSource; break;
                                    case 7: ImageSource7 = imageSource; break;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al cargar imágenes: {ex.Message}", "OK");
            }
        }

        private async Task SearchByCode()
        {
            if (string.IsNullOrWhiteSpace(DetalleTarjaIdInput) || !long.TryParse(DetalleTarjaIdInput, out long detalleTarjaId))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Por favor, ingrese un código válido", "OK");
                return;
            }
            try
            {
                UserDialogs.Instance.ShowLoading("Cargando...");
                await LoadDetalleAsync(detalleTarjaId);
                if (Detalle != null)
                {
                    ShowCodeInput = false;
                    ShowDetails = true;
                }
                else
                {
                    ShowCodeInput = true;
                    ShowDetails = false;
                    await Application.Current.MainPage.DisplayAlert("Alerta", "No se encontraron datos para el código ingresado", "OK");
                }
            }
            catch (Exception ex)
            {
                ShowCodeInput = true;
                ShowDetails = false;
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al buscar: {ex.Message}", "OK");
            }
            finally
            {
                UserDialogs.Instance.HideLoading();
            }
        }

        private void OnScanQR()
        {
            Detalle = null;
            DetalleTarjaIdInput = string.Empty;
            NumeroTarja = null;
            ShowCodeInput = true;
            ShowDetails = false;
            OpcionEscaneo = true;

            Device.BeginInvokeOnMainThread(() =>
            {
                var page = Application.Current.MainPage as CediTarjaConsultaDetail;
                page?.FindByName<Entry>("DetalleTarjaIdInput")?.Focus();
            });
        }

        private async Task SaveDetail()
        {
            if (Detalle == null || SelectedBloque == null || SelectedBloque.Id <= 0 || NumeroBloque <= 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error",
                    "El Bloque y el Número de Bloque deben ser válidos.", "OK");
                return;
            }

            try
            {
                UserDialogs.Instance.ShowLoading("Guardando...");
                var datos = new Datos();
                var request = new ParametroVHSTarjaDetalleUpdate
                {
                    DetalleTarjaID = Detalle.DetalleTarjaID,
                    DocumentoTransporte = Detalle.DocumentoTransporte,
                    PackingList = Detalle.PackingList,
                    VIN = Detalle.VIN,
                    NumeroMotor = Detalle.NumeroMotor,
                    Observaciones = Detalle.Observaciones,
                    BloqueId = SelectedBloque.Id,
                    NumeroBloque = NumeroBloque
                };

                var response = await datos.UpdateCediTarjaDetailAsync(request);
                if (response.Resultado.Respuesta)
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", "Actualización exitosa", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "No se pudo actualizar", "OK");
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

        private async void OnCloseClicked(object parameter)
        {
            if (parameter is Page page)
            {
                MessagingCenter.Send(this, "ActualizarDespacho");

                if (page.Navigation.ModalStack.Count > 0)
                    await page.Navigation.PopModalAsync(true);
                else if (page.Navigation.NavigationStack.Count > 1)
                    await page.Navigation.PopAsync(true);
            }
        }

        public async Task PrintTarja()
        {
            if (Detalle == null) return;

            long detalleTarjaId = Detalle.DetalleTarjaID;
            string currentDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            string zpl = GenerateZPLForTarja(
                detalleTarjaId,
                currentDate,
                Detalle.TipoCargaDescripcion,
                Detalle.InformacionVehiculo,
                Detalle.DocumentoTransporte,
                Detalle.PackingList,
                Detalle.VIN,
                Detalle.NumeroMotor
            );

            _printService.Print(zpl);
        }

        private string GenerateZPLForTarja(
            long detalleTarjaId,
            string date,
            string tipoCargaDescripcion,
            string informacionVehiculo,
            string documentoTransporte,
            string packingList,
            string vin,
            string numeroMotor)
        {
            string v_ZPL = @"^XA^PW800^LL1218";
            v_ZPL += " ^FO150,50^BQN,2,6";
            v_ZPL += " " + string.Format(
                "^FDQA,{0};DATE:{1};TC:{2};IV:{3};DT:{4};PL:{5};V:{6};NM:{7}^FS",
                detalleTarjaId,
                date.Substring(0, Math.Min(50, date.Length)),
                tipoCargaDescripcion ?? "",
                informacionVehiculo ?? "",
                documentoTransporte ?? "",
                packingList ?? "",
                vin ?? "",
                numeroMotor ?? ""
            );
            v_ZPL += " ^CF0,18";
            v_ZPL += $" ^FO150,520^FDID:^FS ^FO250,520^FD{detalleTarjaId}^FS";
            v_ZPL += $" ^FO150,550^FDFECHA:^FS ^FO250,550^FD{date}^FS";
            v_ZPL += " ^XZ";
            return v_ZPL;
        }

        public ICommand DespacharCommand => new Command(async () => await Despachar());

        private async Task Despachar()
        {
            if (_entry != null && _entry.VehiculoDespachadoID > 0)
            {
                await Application.Current.MainPage.Navigation.PushModalAsync(new CediAgregarEvidenciaEntrega(_entry.VehiculoDespachadoID));
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se encontró el ID del vehículo despachado.", "OK");
            }
        }

        public ICommand NovedadCommand => new Command(async () => await AgregarNovedad());

       
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async Task AgregarNovedad()
        {
            long detalleId = 0;

            // Si ya cargaste un detalle:
            if (Detalle != null && Detalle.DetalleTarjaID > 0)
            {
                detalleId = Detalle.DetalleTarjaID;
            }
            else if (!string.IsNullOrWhiteSpace(DetalleTarjaIdInput) && long.TryParse(DetalleTarjaIdInput, out long parsedId))
            {
                detalleId = parsedId;
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Debe ingresar o cargar un código de tarja válido.", "OK");
                return;
            }

            var model = new AppModelNovedadDetalleTarja
            {
                DetalleTarjaID = detalleId,
                Descripcion = Detalle?.Observaciones // Puedes pasar vacío si no hay detalle cargado
            };

            await Application.Current.MainPage.Navigation.PushModalAsync(new CediNovedadDetalleTarjaPage(model));
        }

    }
}
