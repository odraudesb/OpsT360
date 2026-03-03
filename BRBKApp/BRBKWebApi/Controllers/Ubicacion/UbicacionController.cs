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
    public class UbicacionController : ApiController
    {
        private static Int64? lm = -3;
        private string OnError;
        
        [HttpGet]
        [Route("api/lista_ubicaciones")]
        [ValidateModelAttribute]
        public RespuestaViewModel<List<ubicacion>> Lista_Tarea_Manuales()
        {
            List<string> Mensaje = new List<string>();
            bool Valido = true;
            ResultadoViewModel respuestaVie = new ResultadoViewModel();
            List<ubicacion> ObjTarea = new List<ubicacion>();
            RespuestaViewModel<List<ubicacion>> respuesta = new RespuestaViewModel<List<ubicacion>>();

            try
            {
                ObjTarea = BRBKWebApiData.ubicacionDA.listadoUbicacion(out OnError);

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
                respuestaVie.Titulo = "Listado Tareas Manuales";
                respuestaVie.TotalRowsCount = ObjTarea != null && ObjTarea?.Count > 0 ? ObjTarea.Count : 0;
                respuesta.Resultado = respuestaVie;
                respuesta.Respuesta = ObjTarea ?? new List<ubicacion>();

            }
            catch (Exception ex)
            {
                //registro log de errores
                lm = SqlConexion.Cls_Conexion.LogEvent<Exception>(this.User.Identity.Name, nameof(Lista_Tarea_Manuales), "api/lista_tarea_manuales", false, null, null, ex.StackTrace, ex);

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
