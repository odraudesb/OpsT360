using ApiDatos;
using ApiModels.AppModels;
using ApiModels.Parametros;
using BRBKApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ViewModel;

namespace BRBKApp.DA
{
    public class ServiceCedi
    {
        public static async Task<ObservableCollection<CediOrdenTrabajo>> GetOrdenesPendientes()
        {
            ObservableCollection<CediOrdenTrabajo> collection = new ObservableCollection<CediOrdenTrabajo>();
            Datos datos = new Datos();
            try
            {
                var response = await datos.GetCediOrdenesTrabajoAsync();
                collection = new ObservableCollection<CediOrdenTrabajo>(response.Respuesta);
            }
            catch (Exception)
            {
            }
            return collection;
        }

        public static async Task<ObservableCollection<CediTarjaMensaje>> GetTarjaList()
        {
            ObservableCollection<CediTarjaMensaje> collection = new ObservableCollection<CediTarjaMensaje>();
            Datos datos = new Datos();
            try
            {
                var response = await datos.GetCediTarjaPendienteAsync(false, -1);
                collection = new ObservableCollection<CediTarjaMensaje>(response.Respuesta);
            }
            catch (Exception)
            {
            }
            return collection;
        }

        public static async Task<RespuestaViewModel<CediTarjaModel>> RegistraTarja(ParametroVHSCreaTarja parametro)
        {
            Datos datos = new Datos();
            return await datos.RegistrarCediTarjaAsync(parametro);
        }

        public static async Task<RespuestaViewModel<CediTarjaMensaje>> GetTarjaDetail(int idOrdenTrabajo)
        {
            RespuestaViewModel<CediTarjaMensaje> result = new RespuestaViewModel<CediTarjaMensaje>();
            Datos datos = new Datos();
            try
            {
                var response = await datos.GetCediTarjaDetailAsync(idOrdenTrabajo);
                result.Respuesta = response.Respuesta.FirstOrDefault();
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static async Task<RespuestaViewModel<CediTarjaDetalleMensaje>> AddTarjaDetail(ParametroVHSTarjaDetalleAdd parametro)
        {
            Datos datos = new Datos();
            return await datos.AddCediTarjaDetailAsync(parametro);
        }

        public static async Task<RespuestaViewModel<AppModelCediTarjaDetalle>> GetTarjaDetailById(long detalleTarjaId)
        {
            Datos datos = new Datos();
            return await datos.GetCediTarjaDetailByIdAsync(detalleTarjaId);
        }


        public static async Task<RespuestaViewModel<List<CediMensajeSimple>>> GetVehiculosDespacho(long paseId)
        {
            Datos datos = new Datos();
            return await datos.GetCediVehiculosDespachoAsync(paseId);
        }

        public static async Task<RespuestaViewModel<long>> RegistraEvidenciaEntrega(ParametroCediCrearEvidenciaEntrega parametro)
        {
            Datos datos = new Datos();
            RespuestaViewModel<long> response = new RespuestaViewModel<long>();
            try
            {
                response = await datos.RegistraCediEvidenciaEntregaAsync(parametro);
            }
            catch (Exception ex)
            {
                response.Resultado = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { ex.Message }
                };
            }
            return response;
        }

        public static async Task<RespuestaViewModel<int>> RegistrarCediNovedadDetalleTarja(ParametroRegistrarNovedadDetalleTarja request)
        {
            RespuestaViewModel<int> returnValue = new RespuestaViewModel<int>();
            Datos datos = new Datos();
            try
            {
                returnValue = await datos.RegistrarCediNovedadDetalleTarjaAsync(request);
            }
            catch (Exception ex)
            {
                returnValue.Resultado = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { $"Error al registrar: {ex.Message}" }
                };
            }
            return returnValue;
        }

    }
}
