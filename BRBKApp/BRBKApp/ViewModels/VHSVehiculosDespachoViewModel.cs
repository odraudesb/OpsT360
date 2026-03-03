using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using ViewModel;
using ApiModels.AppModels;
using BRBKApp.DA;
using BRBKApp.Views;
using System.Windows.Input;
using BRBKApp.ViewModels;

public class VHSVehiculosDespachoViewModel : BaseViewModel
{
    public string NumeroPasePuerta { get; set; }

    public ObservableCollection<VHSMensajeSimple> ListaVehiculos { get; set; }

    private bool _isListaVacia;
    public bool IsListaVacia
    {
        get => _isListaVacia;
        set => SetProperty(ref _isListaVacia, value);
    }

    public ICommand ConsultarCommand { get; }
    public ICommand ConsultarDetalleCommand { get; }

    public VHSVehiculosDespachoViewModel()
    {
        ListaVehiculos = new ObservableCollection<VHSMensajeSimple>();
        IsListaVacia = true;

        ConsultarCommand = new Command(async () => await Consultar());
        ConsultarDetalleCommand = new Command<VHSMensajeSimple>(async (item) => await ConsultarDetalle(item));

        MessagingCenter.Subscribe<VHSTarjaConsultaDetailViewModel>(this, "ActualizarDespacho", async (sender) =>
        {
            await Consultar(false);
        });



    }



  public async Task Consultar(bool mostrarMensajes = true)
    {
        if (string.IsNullOrWhiteSpace(NumeroPasePuerta))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Ingrese el número de pase.", "Ok");
            return;
        }

        if (!long.TryParse(NumeroPasePuerta, out long paseId))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "El número de pase debe ser numérico.", "Ok");
            return;
        }

        var response = await ServiceVHS.GetVehiculosDespacho(paseId);

        if (response?.Resultado?.Respuesta == true)
        {
            ListaVehiculos.Clear();
            foreach (var item in response.Respuesta)
                ListaVehiculos.Add(item);

            IsListaVacia = ListaVehiculos == null || ListaVehiculos.Count == 0;
        }
        else
        {
            ListaVehiculos.Clear();
            IsListaVacia = true;

            if (mostrarMensajes)
            {
                string mensaje = "No se encontraron datos.";

                if (response?.Resultado?.Mensajes?.Count > 0)
                {
                    var mensajeOriginal = string.Join("\n", response.Resultado.Mensajes).Trim();
                    if (mensajeOriginal.StartsWith("No existe información", StringComparison.OrdinalIgnoreCase))
                    {
                        mensaje = $"No existe información, Pase de puerta {NumeroPasePuerta}";
                    }
                    else
                    {
                        mensaje = mensajeOriginal;
                    }
                }

                await Application.Current.MainPage.DisplayAlert("Aviso", mensaje, "Ok");
            }
        }

    }


    private async Task ConsultarDetalle(VHSMensajeSimple item)
    {
        var tarjaMensaje = new VHSTarjaMensaje
        {
            TarjaId = 0,
            OrdenTrabajoID = 0,
            DetalleTarjaID = item.DetalleTarjaID,
            Origen = "Despacho",
            VehiculoDespachadoID = item.VehiculoDespachadoID // Pasas este dato
        };

        await Shell.Current.Navigation.PushModalAsync(new VHSTarjaConsultaDetail(tarjaMensaje));
    }

}
