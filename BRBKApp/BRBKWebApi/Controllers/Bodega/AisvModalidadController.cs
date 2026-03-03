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

namespace MiWebApi.Controllers.Bodega
{
    public class AisvModalidadController : ApiController
    {
        private static Int64? lm = -3;
        private string OnError;
        
        [HttpGet]
        [Route("api/VBS_lista_modalidades")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Catalogo_Modalidad>> Lista_Modalidades()
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<BAN_Catalogo_Modalidad> ObjTarea = new List<BAN_Catalogo_Modalidad>();
            RespuestaViewModel<List<BAN_Catalogo_Modalidad>> respuesta = new RespuestaViewModel<List<BAN_Catalogo_Modalidad>>();

            try
            {
                ObjTarea = BRBKWebApiData.BAN_Catalogo_ModalidadDA.ConsultarLista(out OnError);

                if (ObjTarea == null)
                {
                    Mensaje.Add("No existe información de Tareas Manuales");
                    Valido = false;
                }
                else
                {
                    if (ObjTarea.Count == 0)
                    {
                        Mensaje.Add("No existen registros de tareas manuales que mostrar");
                        Valido = false;
                    }
                    else
                    {
                        Mensaje.Add(string.Format("Total de registros:{0}", ObjTarea.Count));
                        Valido = true;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de modalidades";
                respuestaVie.TotalRowsCount = ObjTarea != null && ObjTarea?.Count > 0 ? ObjTarea.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new List<BAN_Catalogo_Modalidad>();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_Modalidades), "api/VBS_lista_modalidades", false, null, null, ex.StackTrace, ex);

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

        [HttpGet]
        [Route("api/VBS_lista_modalidadesEmbarque")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<BAN_Catalogo_Modalidad>> Lista_ModalidadesEmbarque()
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<BAN_Catalogo_Modalidad> ObjTarea = new List<BAN_Catalogo_Modalidad>();
            RespuestaViewModel<List<BAN_Catalogo_Modalidad>> respuesta = new RespuestaViewModel<List<BAN_Catalogo_Modalidad>>();

            try
            {
                ObjTarea = BRBKWebApiData.BAN_Catalogo_ModalidadDA.ConsultarListaModalidadEmbarque(out OnError);

                if (ObjTarea == null)
                {
                    Mensaje.Add("No existe información de Tareas Manuales");
                    Valido = false;
                }
                else
                {
                    if (ObjTarea.Count == 0)
                    {
                        Mensaje.Add("No existen registros de tareas manuales que mostrar");
                        Valido = false;
                    }
                    else
                    {
                        Mensaje.Add(string.Format("Total de registros:{0}", ObjTarea.Count));
                        Valido = true;
                    }
                }

                respuestaVie.Mensajes = Mensaje != null && Mensaje.Count > 0 ? Mensaje : new List<string>();
                respuestaVie.Respuesta = Valido;
                respuestaVie.TipoMensaje = Valido == true ? TipoMensaje.Exito : TipoMensaje.Error;
                respuestaVie.Titulo = "Listado de modalidades";
                respuestaVie.TotalRowsCount = ObjTarea != null && ObjTarea?.Count > 0 ? ObjTarea.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new List<BAN_Catalogo_Modalidad>();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_Modalidades), "api/VBS_lista_modalidades", false, null, null, ex.StackTrace, ex);

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
