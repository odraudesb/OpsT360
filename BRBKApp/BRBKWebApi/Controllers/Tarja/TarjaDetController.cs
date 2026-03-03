using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static ViewModel.Enumerados;
using ViewModel;
using ReferencialVie = ViewModel;
using ApiModels.AppModels;
using BRBKWebApiData;

namespace MiWebApi.Controllers.Tarja
{
    public class TarjaDetController : ApiController
    {
        private static Int64? lm = -3;
        private string OnError;
        private Int64? IdGenerado = null;

        [HttpPost]
        [Route("api/lista_BL")]
        [ValidateModelAttribute]

        public RespuestaViewModel<List<tarjaDet>> Lista_BL([FromBody] ParametrosTarjaDet.ParametrosConsultaListaBL pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<tarjaDet> query = new List<tarjaDet>();
            RespuestaViewModel<List<tarjaDet>> respuesta = new RespuestaViewModel<List<tarjaDet>>();

            try
            {
                if(pObj.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    query = BRBKWebApiData.tarjaDetDA.GetTarjaDetXmrn(pObj.MRN, pObj.Lugar,  out OnError);

                    if (query == null)
                    {
                        Mensaje.Add(string.Format("No existe información de BL con los criterios ingresados, Error: {0}", OnError));
                        Valido = false;
                    }
                    else
                    {
                        if (query?.Count <= 0 && Valido)
                        {
                            Mensaje.Add(string.Format("No existe información de BL con los criterios ingresados, MRN {0}", pObj.MRN));
                            Valido = false;
                        }
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de BL por MRN";
                respuestaVie.TotalRowsCount = query != null && query?.Count > 0 ? query.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = query ?? new List<tarjaDet>();
            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_BL), "api/lista_BL", false, null, null, ex.StackTrace, ex);

                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("EXCEPCION NO CONTROLADA # {0}: {1}", lm, ex.Message.ToString()));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }

            return respuesta;

        }

        [HttpPost]
        [Route("api/consulta_bl")]
        [ValidateModelAttribute]
        public RespuestaViewModel<tarjaDet> GetTarjaDet([FromBody] ParametrosTarjaDet.ParametrosConsultaGetBL pObj)
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            tarjaDet oTarjaDet = new tarjaDet();
            RespuestaViewModel<tarjaDet> respuesta = new RespuestaViewModel<tarjaDet>();

            try
            {
                if (pObj.PreValidationsTransaction(out OnError) != 1)
                {
                    Mensaje.Add(string.Format("Error:{0}", OnError));
                    Valido = false;
                }
                else
                {
                    var Entity = tarjaDetDA.GetTarjaDet(long.Parse(pObj.idTarjaDet.ToString()));
                    if (Entity == null)
                    {
                        Mensaje.Add(string.Format("No existe información con el Id de Detalle:{0}", pObj.idTarjaDet));
                        Valido = false;
                    }
                    else
                    {
                        Mensaje.Add("Ok");
                        Valido = true;
                        IdGenerado = (long)Entity.idTarjaDet;

                        oTarjaDet = Entity;

                    }
                }


                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Recepción";
                respuestaVie.TotalRowsCount = IdGenerado != null && IdGenerado.Value > 0 ? IdGenerado.Value : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = oTarjaDet ?? new tarjaDet();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(GetTarjaDet), "api/consulta_bl", false, null, null, ex.StackTrace, ex);

                respuestaVie = new ResultadoViewModel();
                respuestaVie.Respuesta = false;
                respuestaVie.Titulo = "Error";
                respuestaVie.TipoMensaje = Enumerados.TipoMensaje.Error;
                List<String> mensaje = new List<string>();
                mensaje.Add(string.Format("EXCEPCION NO CONTROLADA # {0}: {1}", lm, ex.Message.ToString()));
                respuestaVie.Mensajes = mensaje;
                respuesta.Resultado = respuestaVie;
            }

            return respuesta;
        }


    }
}
