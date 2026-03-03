using ApiDatos;
using ApiModels.AppModels;
using ApiModels.Parametros;
using BRBKApp.Models;
using BRBKApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ViewModel;
using AppModelVHSTarjaDetalle = BRBKApp.Models.AppModelVHSTarjaDetalle;

namespace BRBKApp.DA
{
    public class ServiceVHS
    {
        /// <summary>
        /// Obtiene las ordenes de trabajo que tienen estado pendiente
        /// </summary>
        /// <returns>Lista de ordenes de trabajo</returns>
        public static async Task<ObservableCollection<VHSOrdenTrabajo>> GetOrdenesPendientes()
        {
            ObservableCollection<VHSOrdenTrabajo> datamodelvalues = new ObservableCollection<VHSOrdenTrabajo>();
            Datos datos = new Datos();
            
            try
            {
                var response = await datos.GetOrdenesTrabajoAsync().ConfigureAwait(false);
                datamodelvalues = new ObservableCollection<VHSOrdenTrabajo>(response.Respuesta);
            }
            catch (Exception ex)
            {
                
            }
            return datamodelvalues;
        }

        public static async Task<ObservableCollection<VHSTarjaMensaje>> GetTarjaList()
        {
            ObservableCollection<VHSTarjaMensaje> collection = new ObservableCollection<VHSTarjaMensaje>();
            Datos datos = new Datos();
            try
            {
                var response = await datos.GetTarjaPendienteAsync(false, -1);
                collection = new ObservableCollection<VHSTarjaMensaje>(response.Respuesta);
            }
            catch (Exception ex)
            {

            }
            return collection;
        }

        public static async Task<RespuestaViewModel<VHSTarjaModel>> RegistraTarja(ParametroVHSCreaTarja parametroVHSCreaTarja)
        {
            VHSTarjaModel _rec;
            Datos datos = new Datos();
            RespuestaViewModel<VHSTarjaModel> result = new RespuestaViewModel<VHSTarjaModel>();

            result = await datos.RegistrarTarjaAsync(parametroVHSCreaTarja).ConfigureAwait(false);
            
            return result;
        }
        public static async Task<RespuestaViewModel<VHSTarjaMensaje>> GetTarjaDetail(int idOrdenTrabajo)
        {
            RespuestaViewModel<VHSTarjaMensaje> returnValue = new RespuestaViewModel<VHSTarjaMensaje>();
            Datos datos = new Datos();
            try
            {
                var response = await datos.GetTarjaDetailAsync(idOrdenTrabajo);
                var entry = response.Respuesta.First();
                returnValue.Respuesta = entry;
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }
        public static async Task<RespuestaViewModel<VHSTarjaDetalleMensaje>> AddTarjaDetail(ParametroVHSTarjaDetalleAdd parametro)
        {
            RespuestaViewModel<VHSTarjaDetalleMensaje> returnValue = new RespuestaViewModel<VHSTarjaDetalleMensaje>();
            Datos datos = new Datos();
            try
            {
                var response = await datos.AddTarjaDetailAsync(parametro);
                returnValue = response;
            }
            catch (Exception ex)
            {

            }
            return returnValue;
        }

        public static async Task<RespuestaViewModel<ApiModels.AppModels.AppModelVHSTarjaDetalle>> GetTarjaDetailById(long detalleTarjaId)
        {
            RespuestaViewModel<ApiModels.AppModels.AppModelVHSTarjaDetalle> returnValue = new RespuestaViewModel<ApiModels.AppModels.AppModelVHSTarjaDetalle>();
            Datos datos = new Datos();
            try
            {
                var response = await datos.GetTarjaDetailByIdAsync(detalleTarjaId);
                returnValue = response; // Asignación directa si los tipos coinciden exactamente
            }
            catch (Exception ex)
            {
                returnValue.Resultado = new ResultadoViewModel
                {
                    Respuesta = false,
                    Titulo = "Error",
                    TipoMensaje = Enumerados.TipoMensaje.Error,
                    Mensajes = new List<string> { $"Error al consultar detalle: {ex.Message}" }
                };
            }
            return returnValue;
        }
        public static async Task<ObservableCollection<VHSDataModel<Contenedor>>> GetContenedores()
        {
            ObservableCollection<VHSDataModel<Contenedor>> resps = new ObservableCollection<VHSDataModel<Contenedor>>();
            // alguna llamada await a un api rest
            resps.Add(new VHSDataModel<Contenedor>()
            {
                Id = 1,
                ValoresEntidad = new List<VHSCampo>()
                {
                    new VHSCampo()
                    {
                        Id=1,
                        GrupoEntidad=1,
                        ColorEtiqueta = "OrangeRed",
                        TamanhoEtiqueta=12,
                        EtiquetaNegrita=false,
                        ColorContenido = "Black",
                        TamanhoContenido=16,
                        TextoNegrita=true,
                        Orden=1,
                        Etiqueta="Numero Contenedor",
                        Contenido="00022"
                    },
                    new VHSCampo()
                    {
                        Id=2,
                        GrupoEntidad=1,
                        ColorEtiqueta = "OrangeRed",
                        TamanhoEtiqueta=12,
                        EtiquetaNegrita=false,
                        ColorContenido = "Black",
                        TamanhoContenido=16,
                        TextoNegrita=true,
                        Orden=2,
                        Etiqueta="NombreNave",
                        Contenido="Liberty"
                    }
                },
                Botones = new List<VHSBoton>() 
                { 
                    new VHSBoton() 
                    {
                        Id=1,
                        ColorBoton = "",
                        ColorTexto="",
                        EsVisible=true,
                        GrupoEntidadId=1,
                        Texto = "Cerrar",
                        Nombre = "cerrar",
                        Orden = 1
                    }
                }
            });
            resps.Add(new VHSDataModel<Contenedor>()
            {
                Id = 2,
                ValoresEntidad = new List<VHSCampo>()
                {
                    new VHSCampo()
                    {
                        Id=3,
                        GrupoEntidad=2,
                        ColorEtiqueta = "OrangeRed",
                        TamanhoEtiqueta=12,
                        EtiquetaNegrita=false,
                        ColorContenido = "Black",
                        TamanhoContenido=16,
                        TextoNegrita=true,
                        Orden=1,
                        Etiqueta="Numero Contenedor",
                        Contenido="00024"
                    },
                    new VHSCampo()
                    {
                        Id=4,
                        GrupoEntidad=2,
                        ColorEtiqueta = "OrangeRed",
                        TamanhoEtiqueta=12,
                        EtiquetaNegrita=false,
                        ColorContenido = "Black",
                        TamanhoContenido=16,
                        TextoNegrita=true,
                        Orden=2,
                        Etiqueta="NombreNave",
                        Contenido="Pegasus"
                    }
                }
            });
            return resps;
        }

        public static async Task<RespuestaViewModel<List<VHSMensajeSimple>>> GetVehiculosDespacho(long paseId)
        {
            Datos datos = new Datos();
            return await datos.GetVehiculosDespachoAsync(paseId);
        }


        public static async Task<RespuestaViewModel<long>> RegistraEvidenciaEntrega(ParametroVHSCrearEvidenciaEntrega parametro)
        {
            Datos datos = new Datos();
            RespuestaViewModel<long> response = new RespuestaViewModel<long>();
            try
            {
                response = await datos.RegistraEvidenciaEntregaAsync(parametro);
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

        public static async Task<RespuestaViewModel<int>> RegistrarNovedadDetalleTarja(ParametroRegistrarNovedadDetalleTarja parametro)
        {
            RespuestaViewModel<int> returnValue = new RespuestaViewModel<int>();
            Datos datos = new Datos();
            try
            {
                returnValue = await datos.RegistrarNovedadDetalleTarjaAsync(parametro);
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
